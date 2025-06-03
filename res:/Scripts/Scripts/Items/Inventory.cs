using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Node
{
    [Signal]
    public delegate void InventoryChangedEventHandler();
    
    [Signal]
    public delegate void ItemAddedEventHandler(ItemData item, int quantity);
    
    [Signal]
    public delegate void ItemRemovedEventHandler(ItemData item, int quantity);
    
    [Export]
    public int Size { get; private set; } = 24; // Default inventory size
    
    private List<InventorySlot> _slots;
    private InventorySlot _selectedSlot;
    
    // UI elements
    private GridContainer _gridContainer;
    private Panel _inventoryPanel;
    private Label _itemDescription;
    
    public override void _Ready()
    {
        // Make sure the inventory persists between scenes
        ProcessMode = ProcessModeEnum.Always;
        
        _slots = new List<InventorySlot>();
        for (int i = 0; i < Size; i++)
        {
            _slots.Add(new InventorySlot(i));
        }
        
        CreateUI();
        
        // Try to load saved inventory
        LoadInventory();
        
        // If no save file exists, add starting items
        if (_slots.All(s => s.IsEmpty))
        {
            AddStartingItems();
        }
        
        // Make sure the inventory panel starts hidden
        _inventoryPanel.Visible = false;
        
        // Connect to inventory changed signal
        InventoryChanged += OnInventoryChanged;
    }
    
    private void CreateUI()
    {
        // Main inventory panel
        _inventoryPanel = new Panel();
        _inventoryPanel.SetAnchorsPreset(Control.LayoutPreset.CenterRight);
        _inventoryPanel.Position = new Vector2(-250, -300);
        _inventoryPanel.Size = new Vector2(200, 400);
        
        // Make sure the panel is always on top
        _inventoryPanel.ZIndex = 100;
        
        AddChild(_inventoryPanel);
        
        // Grid container for slots
        _gridContainer = new GridContainer();
        _gridContainer.Columns = 4;
        _gridContainer.SetAnchorsPreset(Control.LayoutPreset.TopCenter);
        _gridContainer.Position = new Vector2(10, 10);
        _inventoryPanel.AddChild(_gridContainer);
        
        // Create slot buttons
        for (int i = 0; i < Size; i++)
        {
            Button slotButton = new Button();
            slotButton.CustomMinimumSize = new Vector2(45, 45);
            slotButton.Pressed += () => OnSlotPressed(i);
            _gridContainer.AddChild(slotButton);
        }
        
        // Item description label
        _itemDescription = new Label();
        _itemDescription.Position = new Vector2(10, 360);
        _itemDescription.Size = new Vector2(180, 30);
        _inventoryPanel.AddChild(_itemDescription);
        
        UpdateUI();
    }
    
    private void AddStartingItems()
    {
        // Add basic tools
        AddItem(ItemData.CreateTool("Shovel", "Used for digging and planting"));
        AddItem(ItemData.CreateTool("Water Can", "Used for watering plants"));
        
        // Add some seeds
        AddItem(ItemData.CreateSeed(PlantType.Sunflower), 5);
        AddItem(ItemData.CreateSeed(PlantType.Succulent), 3);
        
        // Add some fertilizer
        AddItem(ItemData.CreateFertilizer("Basic Fertilizer", "Standard fertilizer for plants"), 10);
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_inventory"))
        {
            ToggleInventory();
            // Prevent the input from propagating further
            GetViewport().SetInputAsHandled();
        }
    }
    
    private void ToggleInventory()
    {
        _inventoryPanel.Visible = !_inventoryPanel.Visible;
        if (!_inventoryPanel.Visible)
        {
            _selectedSlot = null;
            _itemDescription.Text = "";
        }
    }
    
    private void OnSlotPressed(int index)
    {
        var slot = _slots[index];
        if (_selectedSlot == null)
        {
            _selectedSlot = slot;
            if (!slot.IsEmpty)
            {
                _itemDescription.Text = $"{slot.Item.Name}\n{slot.Item.Description}";
                if (slot.Item.Type == ItemType.Tool)
                {
                    _itemDescription.Text += $"\nDurability: {slot.GetDurability():F0}%";
                }
            }
        }
        else
        {
            if (_selectedSlot != slot)
            {
                _selectedSlot.TransferTo(slot, _selectedSlot.Quantity);
                EmitSignal(SignalName.InventoryChanged);
            }
            _selectedSlot = null;
            _itemDescription.Text = "";
        }
        UpdateUI();
    }
    
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (amount <= 0) return false;
        
        // First try to stack with existing items
        foreach (var slot in _slots.Where(s => !s.IsEmpty && s.Item.Name == item.Name && !s.IsFull))
        {
            int spaceInSlot = item.MaxStackSize - slot.Quantity;
            int amountToAdd = Math.Min(amount, spaceInSlot);
            
            if (slot.AddItem(item, amountToAdd))
            {
                amount -= amountToAdd;
                EmitSignal(SignalName.ItemAdded, item, amountToAdd);
                if (amount <= 0)
                {
                    UpdateUI();
                    return true;
                }
            }
        }
        
        // If we still have items to add, find empty slots
        foreach (var slot in _slots.Where(s => s.IsEmpty))
        {
            int amountToAdd = Math.Min(amount, item.MaxStackSize);
            
            if (slot.AddItem(item, amountToAdd))
            {
                amount -= amountToAdd;
                EmitSignal(SignalName.ItemAdded, item, amountToAdd);
                if (amount <= 0)
                {
                    UpdateUI();
                    return true;
                }
            }
        }
        
        UpdateUI();
        return amount <= 0;
    }
    
    public bool RemoveItem(string itemName, int amount = 1)
    {
        int remainingAmount = amount;
        
        foreach (var slot in _slots.Where(s => !s.IsEmpty && s.Item.Name == itemName))
        {
            int amountToRemove = Math.Min(remainingAmount, slot.Quantity);
            if (slot.RemoveItems(amountToRemove))
            {
                remainingAmount -= amountToRemove;
                EmitSignal(SignalName.ItemRemoved, slot.Item, amountToRemove);
                if (remainingAmount <= 0)
                {
                    UpdateUI();
                    return true;
                }
            }
        }
        
        UpdateUI();
        return remainingAmount <= 0;
    }
    
    public bool HasItem(string itemName, int amount = 1)
    {
        return _slots.Where(s => !s.IsEmpty && s.Item.Name == itemName)
                    .Sum(s => s.Quantity) >= amount;
    }
    
    public InventorySlot GetSelectedSlot()
    {
        return _selectedSlot;
    }
    
    private void UpdateUI()
    {
        for (int i = 0; i < Size; i++)
        {
            var slot = _slots[i];
            var button = _gridContainer.GetChild<Button>(i);
            
            if (!slot.IsEmpty)
            {
                // Update button text/icon based on item
                button.Text = $"{slot.Item.Name}\n{slot.Quantity}";
                // If you have icons:
                // button.Icon = ResourceLoader.Load<Texture2D>(slot.Item.IconPath);
            }
            else
            {
                button.Text = "";
                button.Icon = null;
            }
            
            // Highlight selected slot
            button.Modulate = slot == _selectedSlot ? 
                new Color(1, 1, 0.8f) : // Light yellow highlight
                new Color(1, 1, 1);
        }
    }
    
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < _slots.Count)
        {
            return _slots[index];
        }
        return null;
    }
    
    public void Clear()
    {
        foreach (var slot in _slots)
        {
            slot.Clear();
        }
        UpdateUI();
    }
    
    public bool AddItemToSlot(ItemData item, int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count) return false;
        
        var slot = _slots[slotIndex];
        if (slot.AddItem(item, amount))
        {
            EmitSignal(SignalName.ItemAdded, item, amount);
            UpdateUI();
            return true;
        }
        return false;
    }
    
    public void SaveInventory()
    {
        SaveManager.SaveInventory(this);
    }
    
    public void LoadInventory()
    {
        SaveManager.LoadInventory(this);
    }
    
    // Add autosave on certain events
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            // Autosave when the game is closed
            SaveInventory();
        }
    }
    
    private void OnInventoryChanged()
    {
        // Autosave when inventory changes
        SaveInventory();
    }
} 