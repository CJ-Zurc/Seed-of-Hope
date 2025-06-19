using Godot;
using System;

public partial class BuyAndSellPanel : Control
{
    private CanvasLayer buyCanvasLayer;
    private CanvasLayer sellCanvasLayer;
    private Button buyButton;
    private Button sellButton;

    private SelectedSeedManager selectedSeedManager;
    private SeedInventoryManager seedInventoryManager;

    public override void _Ready()
    {
        buyCanvasLayer = GetNode<CanvasLayer>("BuyUI/CanvasLayer");
        sellCanvasLayer = GetNode<CanvasLayer>("sellUI/CanvasLayer");

        // Correct button paths
        buyButton = GetNode<Button>("/root/Control/sellUI/CanvasLayer/sellUIBG/buyButton");
        sellButton = GetNode<Button>("/root/Control/BuyUI/CanvasLayer/SappyBG/sellButton");

        // Only show buy UI at start
        buyCanvasLayer.Visible = true;
        sellCanvasLayer.Visible = false;

        buyButton.Pressed += OnBuyButtonPressed;
        sellButton.Pressed += OnSellButtonPressed;

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
        seedInventoryManager.BuySeed(SeedName);
        GD.Print($"Bought seed: {SeedName}");

        //sets the selected seed after buying
        var selectedSeedManager = GetNode<SelectedSeedManager>("/root/SelectedSeedManager");
        selectedSeedManager.SelectedSeed = SeedName;
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