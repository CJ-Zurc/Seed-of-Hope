using Godot;
using System;
using System.Collections.Generic;

public partial class Computer : Node2D
{
    private Control _computerUI;
    private ItemList _seedsList;
    private ItemList _harvestList;
    private Label _coinsLabel;
    private Button _buyButton;
    private Button _sellButton;
    private Button _closeButton;
    
    private Inventory _playerInventory;
    private CurrencyManager _currencyManager;
    
    // Seed prices (in AkimCoins)
    private readonly Dictionary<PlantType, float> _seedPrices = new()
    {
        { PlantType.Tomato, 10.0f },
        { PlantType.Carrot, 5.0f },
        { PlantType.Potato, 8.0f },
        { PlantType.Corn, 15.0f },
        { PlantType.Wheat, 3.0f }
    };
    
    // Harvest sell prices (in AkimCoins)
    private readonly Dictionary<PlantType, float> _harvestPrices = new()
    {
        { PlantType.Tomato, 25.0f },
        { PlantType.Carrot, 12.0f },
        { PlantType.Potato, 20.0f },
        { PlantType.Corn, 35.0f },
        { PlantType.Wheat, 8.0f }
    };
    
    public override void _Ready()
    {
        _playerInventory = GetNode<Inventory>("/root/PlayerInventory");
        _currencyManager = GetNode<CurrencyManager>("/root/CurrencyManager");
        
        CreateComputerUI();
        _computerUI.Visible = false;
    }
    
    private void CreateComputerUI()
    {
        _computerUI = new Control();
        _computerUI.SetAnchorsPreset(Control.LayoutPreset.Center);
        
        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(600, 400);
        _computerUI.AddChild(panel);
        
        var mainVBox = new VBoxContainer();
        panel.AddChild(mainVBox);
        
        // Title and coins
        var titleHBox = new HBoxContainer();
        mainVBox.AddChild(titleHBox);
        
        var titleLabel = new Label { Text = "Farm Computer Terminal" };
        titleHBox.AddChild(titleLabel);
        
        _coinsLabel = new Label { Text = $"AkimCoins: {_currencyManager.GetCurrentCoins():F1}" };
        titleHBox.AddChild(_coinsLabel);
        
        // Lists container
        var listsHBox = new HBoxContainer();
        mainVBox.AddChild(listsHBox);
        
        // Seeds list
        var seedsVBox = new VBoxContainer();
        listsHBox.AddChild(seedsVBox);
        
        seedsVBox.AddChild(new Label { Text = "Available Seeds" });
        
        _seedsList = new ItemList();
        _seedsList.CustomMinimumSize = new Vector2(250, 300);
        seedsVBox.AddChild(_seedsList);
        
        // Harvest list
        var harvestVBox = new VBoxContainer();
        listsHBox.AddChild(harvestVBox);
        
        harvestVBox.AddChild(new Label { Text = "Harvested Plants" });
        
        _harvestList = new ItemList();
        _harvestList.CustomMinimumSize = new Vector2(250, 300);
        harvestVBox.AddChild(_harvestList);
        
        // Buttons
        var buttonsHBox = new HBoxContainer();
        mainVBox.AddChild(buttonsHBox);
        
        _buyButton = new Button { Text = "Buy Selected" };
        _buyButton.Pressed += OnBuyPressed;
        buttonsHBox.AddChild(_buyButton);
        
        _sellButton = new Button { Text = "Sell Selected" };
        _sellButton.Pressed += OnSellPressed;
        buttonsHBox.AddChild(_sellButton);
        
        _closeButton = new Button { Text = "Close" };
        _closeButton.Pressed += OnClosePressed;
        buttonsHBox.AddChild(_closeButton);
        
        AddChild(_computerUI);
        
        // Connect to currency manager
        _currencyManager.CurrencyChanged += OnCurrencyChanged;
        
        // Initial population
        PopulateSeedsList();
    }
    
    private void PopulateSeedsList()
    {
        _seedsList.Clear();
        foreach (var seedPrice in _seedPrices)
        {
            _seedsList.AddItem($"{seedPrice.Key} Seeds - {seedPrice.Value} AkimCoins");
        }
    }
    
    private void UpdateHarvestList()
    {
        _harvestList.Clear();
        var harvestedItems = new Dictionary<PlantType, int>();
        
        // Count harvested items
        for (int i = 0; i < _playerInventory.Size; i++)
        {
            var slot = _playerInventory.GetSlot(i);
            if (!slot.IsEmpty && slot.Item.Type == ItemType.Harvest)
            {
                PlantType plantType = Enum.Parse<PlantType>(slot.Item.Name);
                if (harvestedItems.ContainsKey(plantType))
                    harvestedItems[plantType] += slot.Quantity;
                else
                    harvestedItems[plantType] = slot.Quantity;
            }
        }
        
        // Add to list
        foreach (var harvest in harvestedItems)
        {
            float totalValue = harvest.Value * _harvestPrices[harvest.Key];
            _harvestList.AddItem($"{harvest.Key} x{harvest.Value} - {totalValue} AkimCoins");
        }
    }
    
    private void OnBuyPressed()
    {
        var selectedItems = _seedsList.GetSelectedItems();
        if (selectedItems.Length == 0) return;
        
        var selectedIndex = selectedItems[0];
        var plantType = (PlantType)selectedIndex;
        
        if (_currencyManager.CanAfford(_seedPrices[plantType]))
        {
            var seedItem = ItemData.CreateSeed(plantType);
            if (_playerInventory.AddItem(seedItem))
            {
                _currencyManager.SpendCoins(_seedPrices[plantType]);
                GD.Print($"Bought {plantType} seeds");
                
                // Update UI after successful purchase
                UpdateHarvestList(); // In case the player has a full harvest stack that could be affected
                _seedsList.DeselectAll();
            }
            else
            {
                GD.Print("Inventory is full!");
            }
        }
        else
        {
            GD.Print("Not enough AkimCoins!");
        }
    }
    
    private void OnSellPressed()
    {
        var selectedItems = _harvestList.GetSelectedItems();
        if (selectedItems.Length == 0) return;
        
        var selectedIndex = selectedItems[0];
        var plantType = GetPlantTypeFromHarvestListIndex(selectedIndex);
        
        // Find and remove one harvest item
        for (int i = 0; i < _playerInventory.Size; i++)
        {
            var slot = _playerInventory.GetSlot(i);
            if (!slot.IsEmpty && slot.Item.Type == ItemType.Harvest && 
                Enum.Parse<PlantType>(slot.Item.Name) == plantType)
            {
                _playerInventory.RemoveItemFromSlot(i, 1);
                _currencyManager.AddCoins(_harvestPrices[plantType]);
                GD.Print($"Sold {plantType} for {_harvestPrices[plantType]} AkimCoins");
                
                // Update UI after successful sale
                UpdateHarvestList();
                _harvestList.DeselectAll();
                break;
            }
        }
    }
    
    private PlantType GetPlantTypeFromHarvestListIndex(int index)
    {
        var harvestedItems = new List<PlantType>();
        
        for (int i = 0; i < _playerInventory.Size; i++)
        {
            var slot = _playerInventory.GetSlot(i);
            if (!slot.IsEmpty && slot.Item.Type == ItemType.Harvest)
            {
                PlantType plantType = Enum.Parse<PlantType>(slot.Item.Name);
                if (!harvestedItems.Contains(plantType))
                    harvestedItems.Add(plantType);
            }
        }
        
        return harvestedItems[index];
    }
    
    private void OnClosePressed()
    {
        _computerUI.Visible = false;
    }
    
    private void OnCurrencyChanged(float newAmount)
    {
        _coinsLabel.Text = $"AkimCoins: {newAmount:F1}";
    }
    
    // Called when player interacts with computer
    public void OnInteract()
    {
        _computerUI.Visible = true;
        UpdateHarvestList();
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel") && _computerUI.Visible)
        {
            OnClosePressed();
        }
    }
} 