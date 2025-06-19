using Godot;
using System;

public partial class SellItems : TextureRect
{
	private Button sellAmpalaya;
    private Button sellSucculent;
	private Button sellCalamansi;
	private Button sellPechay;
    private Button sellSunflower;
    public override void _Ready()
	{
        GetNode<Button>("sellAmpalaya").Pressed += () => SellSeed("Ampalaya");
        GetNode<Button>("sellSucculent").Pressed += () => SellSeed("Succulent");
        GetNode<Button>("sellPechay").Pressed += () => SellSeed("Pechay");
        GetNode<Button>("sellCalamansi").Pressed += () => SellSeed("Calamansi");
        GetNode<Button>("sellSunflower").Pressed += () => SellSeed("Sunflower");
        GetNode<Button>("sellHarvested").Pressed += SellHarvestedItemsPressed;


    }

    private void SellSeed(string seedName)
    {
        var seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
        seedInventoryManager.SellSeed(seedName);
        GD.Print($"Selling {seedName}...");
    }

private void SellHarvestedItemsPressed()
    {
        var harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
        harvestManager.SellHarvestedItems();
        GD.Print("All harvested items sold.");
    }

}
