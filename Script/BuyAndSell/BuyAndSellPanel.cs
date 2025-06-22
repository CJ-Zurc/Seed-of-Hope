using Godot;
using Godot.Collections;
using System;

public partial class BuyAndSellPanel : Control
{
    private CanvasLayer buyCanvasLayer;
    private CanvasLayer sellCanvasLayer;
    private Button buyButton;
    private Button sellButton;

    private SelectedSeedManager selectedSeedManager;
    private SeedInventoryManager seedInventoryManager;

    // Prices for seeds
    private readonly Dictionary<string, int> seedPrices = new Dictionary<string, int>
    {
        { "Succulent", 10 },
        { "Ampalaya", 15 },
        { "Calamansi", 20 },
        { "Pechay", 25 },
        { "Sunflower", 25 }
    };

    // Signals for buy and sell actions
    public override void _Ready()
    {
        // Initializes the canvas layers to appear correctly
        buyCanvasLayer = GetNode<CanvasLayer>("BuyUI/CanvasLayer");
        sellCanvasLayer = GetNode<CanvasLayer>("sellUI/CanvasLayer");

        // Correct button paths
        buyButton = GetNode<Button>("/root/Control/sellUI/CanvasLayer/sellUIBG/buyButton");
        sellButton = GetNode<Button>("/root/Control/BuyUI/CanvasLayer/SappyBG/sellButton");
        buyButton.Pressed += OnBuyButtonPressed;
        sellButton.Pressed += OnSellButtonPressed;

        // Only show buy UI at start
        buyCanvasLayer.Visible = true;
        sellCanvasLayer.Visible = false;

        // Initialize managers
        seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");

        // Connect buy seed buttons
        var buyPanel = GetNode<Control>("BuyUI/CanvasLayer/SappyBG");
        buyPanel.GetNode<Button>("buySucculent").Pressed += () => onBuySeedButtonPressed("Succulent");
        buyPanel.GetNode<Button>("buyAmpalaya").Pressed += () => onBuySeedButtonPressed("Ampalaya");
        buyPanel.GetNode<Button>("buyCalamansi").Pressed += () => onBuySeedButtonPressed("Calamansi");
        buyPanel.GetNode<Button>("buyPechay").Pressed += () => onBuySeedButtonPressed("Pechay");
        buyPanel.GetNode<Button>("buySunflower").Pressed += () => onBuySeedButtonPressed("Sunflower");

    }
    private void onBuySeedButtonPressed(string SeedName)
    {
        var selectedSeedManager = GetNode<SelectedSeedManager>("/root/SelectedSeedManager");
        var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");
        int seedCost = seedPrices.ContainsKey(SeedName) ? seedPrices[SeedName] : 0;

        if (seedCost > 0 && moneyManager.RemoveMoney(seedCost))
        {
            seedInventoryManager.BuySeed(SeedName);
            textManager.Instance.showPopup($"Bought {SeedName} for {seedCost} coins.");

            // Sets the selected seed after buying
            selectedSeedManager.SelectedSeed = SeedName;
        }
        else
        {
            textManager.Instance.showPopup($"Not enough money to buy {SeedName}. Required: {seedCost}");
            // Optionally display a popup/message to the player
        }
    }

    private void OnBuyButtonPressed()
    {
        buyCanvasLayer.Visible = true;
        sellCanvasLayer.Visible = false;
    }

    private void OnSellButtonPressed()
    {
        buyCanvasLayer.Visible = false;
        sellCanvasLayer.Visible = true;
    }
}