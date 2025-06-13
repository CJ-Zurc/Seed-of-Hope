using Godot;
using System;

public partial class ComputerInteraction : Node2D
{
    private Control _computerUI;
    private Control _buyUI;
    private Control _sellUI;
    private Label _currencyLabel;
    private Inventory _playerInventory;
    private int _playerCurrency = 10; // Starting currency
    
    private const int SEED_PRICE = 20;
    private const int PLANT_SELL_PRICE = 50;
    
    public override void _Ready()
    {
        // Load UI scenes
        _computerUI = GetNode<Control>("ComputerUI");
        _buyUI = _computerUI.GetNode<Control>("BuyUI");
        _sellUI = _computerUI.GetNode<Control>("SellUI");
        _currencyLabel = _computerUI.GetNode<Label>("CurrencyBackground/CurrencyLabel");
        
        // Hide UIs initially
        _computerUI.Hide();
        _buyUI.Hide();
        _sellUI.Hide();
        
        // Get player inventory reference
        var player = GetNode<Player>("/root/MainGame/Player");
        _playerInventory = player.GetNode<Inventory>("Inventory");
        
        // Update currency display
        UpdateCurrencyDisplay();
        
        // Connect button signals
        ConnectSignals();
    }
    
    private void ConnectSignals()
    {
        // Buy buttons
        var buyAmpalayaButton = _buyUI.GetNode<Button>("GridContainer/AmpalayaSeedButton");
        var buyCalamansiButton = _buyUI.GetNode<Button>("GridContainer/CalamansiSeedButton");
        var buyPechayButton = _buyUI.GetNode<Button>("GridContainer/PechaySeedButton");
        var buySucculentButton = _buyUI.GetNode<Button>("GridContainer/SucculentSeedButton");
        var buySunflowerButton = _buyUI.GetNode<Button>("GridContainer/SunflowerSeedButton");
        
        buyAmpalayaButton.Pressed += () => BuySeed(PlantType.Ampalaya);
        buyCalamansiButton.Pressed += () => BuySeed(PlantType.Calamansi);
        buyPechayButton.Pressed += () => BuySeed(PlantType.Pechay);
        buySucculentButton.Pressed += () => BuySeed(PlantType.Succulent);
        buySunflowerButton.Pressed += () => BuySeed(PlantType.Sunflower);
        
        // Navigation buttons
        var toBuyButton = _computerUI.GetNode<Button>("BuyButton");
        var toSellButton = _computerUI.GetNode<Button>("SellButton");
        var exitButton = _computerUI.GetNode<Button>("ExitButton");
        
        toBuyButton.Pressed += ShowBuyUI;
        toSellButton.Pressed += ShowSellUI;
        exitButton.Pressed += HideComputer;
    }
    
    public void ShowComputer()
    {
        _computerUI.Show();
        ShowBuyUI(); // Default to buy UI
    }
    
    public void HideComputer()
    {
        _computerUI.Hide();
        _buyUI.Hide();
        _sellUI.Hide();
    }
    
    private void ShowBuyUI()
    {
        _buyUI.Show();
        _sellUI.Hide();
        UpdateCurrencyDisplay();
    }
    
    private void ShowSellUI()
    {
        _sellUI.Show();
        _buyUI.Hide();
        UpdateSellUI();
        UpdateCurrencyDisplay();
    }
    
    private void BuySeed(PlantType plantType)
    {
        if (_playerCurrency >= SEED_PRICE)
        {
            var seedItem = new InventoryItem($"{plantType}Seed", $"A {plantType} seed ready for planting.", SEED_PRICE);
            if (_playerInventory.AddItem(seedItem))
            {
                _playerCurrency -= SEED_PRICE;
                UpdateCurrencyDisplay();
                
                // Track expense in bed's daily report
                var bed = GetNode<BedInteraction>("/root/MainGame/Bed");
                bed.AddExpenses(SEED_PRICE);
                
                GD.Print($"Bought {plantType} seed for {SEED_PRICE} Akim Coins");
            }
            else
            {
                GD.Print("Inventory is full!");
            }
        }
        else
        {
            GD.Print("Not enough Akim Coins!");
        }
    }
    
    private void SellPlant(string plantType)
    {
        if (_playerInventory.RemoveItem(plantType))
        {
            _playerCurrency += PLANT_SELL_PRICE;
            UpdateCurrencyDisplay();
            UpdateSellUI();
            
            // Track earning in bed's daily report
            var bed = GetNode<BedInteraction>("/root/MainGame/Bed");
            bed.AddEarnings(PLANT_SELL_PRICE);
            
            GD.Print($"Sold {plantType} for {PLANT_SELL_PRICE} Akim Coins");
        }
    }
    
    private void UpdateCurrencyDisplay()
    {
        _currencyLabel.Text = $"{_playerCurrency} Akim Coins";
    }
    
    private void UpdateSellUI()
    {
        var items = _playerInventory.GetItems();
        var itemCounts = new Dictionary<string, int>();
        
        // Count items
        foreach (var item in items)
        {
            if (itemCounts.ContainsKey(item.Type))
            {
                itemCounts[item.Type] += item.Count;
            }
            else
            {
                itemCounts[item.Type] = item.Count;
            }
        }
        
        // Update quantity labels
        foreach (var plantType in Enum.GetValues(typeof(PlantType)))
        {
            var label = _sellUI.GetNode<Label>($"GridContainer/{plantType}CountLabel");
            var button = _sellUI.GetNode<Button>($"GridContainer/{plantType}SellButton");
            
            int count = itemCounts.GetValueOrDefault(plantType.ToString(), 0);
            label.Text = count.ToString();
            button.Disabled = count == 0;
            
            // Connect sell button if not already connected
            if (!button.IsConnected("pressed", Callable.From(() => SellPlant(plantType.ToString()))))
            {
                button.Pressed += () => SellPlant(plantType.ToString());
            }
        }
    }
} 