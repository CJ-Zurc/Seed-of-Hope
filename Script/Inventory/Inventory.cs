using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Node
{
    private const int MAX_SLOTS = 24; // 6x4 grid
    private List<InventoryItem> _items = new List<InventoryItem>();
    private Control _inventoryUI;
    private GridContainer _itemGrid;
    private bool _isVisible = false;
    
    public override void _Ready()
    {
        // Load and setup the inventory UI
        var inventoryScene = GD.Load<PackedScene>("res://scenes/InventoryUI.tscn");
        _inventoryUI = inventoryScene.Instantiate<Control>();
        AddChild(_inventoryUI);
        
        // Get the grid container reference
        _itemGrid = _inventoryUI.GetNode<GridContainer>("Panel/MarginContainer/GridContainer");
        
        // Hide inventory initially
        _inventoryUI.Hide();
        
        // Create empty slots
        InitializeEmptySlots();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_inventory"))
        {
            ToggleInventory();
        }
    }
    
    private void InitializeEmptySlots()
    {
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            var slot = GD.Load<PackedScene>("res://scenes/InventorySlot.tscn").Instantiate<Panel>();
            _itemGrid.AddChild(slot);
        }
    }
    
    public void ToggleInventory()
    {
        _isVisible = !_isVisible;
        if (_isVisible)
        {
            _inventoryUI.Show();
            UpdateInventoryDisplay();
        }
        else
        {
            _inventoryUI.Hide();
        }
    }
    
    public bool AddItem(InventoryItem item)
    {
        if (_items.Count >= MAX_SLOTS)
        {
            GD.Print("Inventory is full!");
            return false;
        }
        
        // Try to stack with existing item if it's the same type
        var existingItem = _items.Find(i => i.Type == item.Type && i.Count < i.MaxStack);
        if (existingItem != null)
        {
            existingItem.Count++;
            UpdateInventoryDisplay();
            return true;
        }
        
        // Add as new item
        _items.Add(item);
        UpdateInventoryDisplay();
        return true;
    }
    
    public bool RemoveItem(string itemType)
    {
        var item = _items.Find(i => i.Type.ToString() == itemType);
        if (item != null)
        {
            item.Count--;
            if (item.Count <= 0)
            {
                _items.Remove(item);
            }
            UpdateInventoryDisplay();
            return true;
        }
        return false;
    }
    
    public List<InventoryItem> GetItems()
    {
        return _items;
    }
    
    private void UpdateInventoryDisplay()
    {
        // Clear all slots first
        foreach (var child in _itemGrid.GetChildren())
        {
            if (child is Panel slot)
            {
                var icon = slot.GetNodeOrNull<TextureRect>("ItemIcon");
                var count = slot.GetNodeOrNull<Label>("CountLabel");
                
                if (icon != null) icon.Texture = null;
                if (count != null) count.Text = "";
            }
        }
        
        // Update slots with items
        for (int i = 0; i < _items.Count; i++)
        {
            if (i >= _itemGrid.GetChildCount()) break;
            
            var slot = _itemGrid.GetChild<Panel>(i);
            var item = _items[i];
            
            var icon = slot.GetNodeOrNull<TextureRect>("ItemIcon");
            var count = slot.GetNodeOrNull<Label>("CountLabel");
            
            if (icon != null)
            {
                icon.Texture = GD.Load<Texture2D>($"res://2D Arts/Items/{item.Type}.png");
            }
            
            if (count != null && item.Count > 1)
            {
                count.Text = item.Count.ToString();
            }
        }
    }
} 
