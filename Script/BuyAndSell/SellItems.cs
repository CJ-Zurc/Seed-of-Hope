/*Yung code na'to is basically yung selling ng seeds and ng buong harvests i will be
 indicating kung nasaan ung selling seeds and selling the harvest in case na it's kind of overwhelming to look at*/
using Godot;
using System;
using System.Collections.Generic;

public partial class SellItems : TextureRect
{
	private Button sellAmpalaya;
    private Button sellSucculent;
	private Button sellCalamansi;
	private Button sellPechay;
    private Button sellSunflower;


    //This creates a dictionary to hold the prices of the seeds. SEEDS ONLY 
    private readonly Dictionary<string, int> seedPrices = new Dictionary<string, int>
    {
        { "Succulent", 10 },
        { "Ampalaya", 15 },
        { "Calamansi", 20 },
        { "Pechay", 25 },
        { "Sunflower", 25 }
    };

    //This creates a dictionary to hold the prices of the harvested items. HARVESTED ITEMS ONLY
    private readonly Dictionary<string, int> harvestedPrices = new Dictionary<string, int>
    {
        { "Succulent", 30 },
        { "Ampalaya", 30 },
        { "Calamansi", 40 },
        { "Pechay", 50 },
        { "Sunflower", 70}
    };

    public override void _Ready()
	{
        // Initialize buttons for selling seeds
        GetNode<Button>("sellAmpalaya").Pressed += () => SellSeed("Ampalaya");
        GetNode<Button>("sellSucculent").Pressed += () => SellSeed("Succulent");
        GetNode<Button>("sellPechay").Pressed += () => SellSeed("Pechay");
        GetNode<Button>("sellCalamansi").Pressed += () => SellSeed("Calamansi");
        GetNode<Button>("sellSunflower").Pressed += () => SellSeed("Sunflower");

        // Connect the sellHarvested button to the SellHarvestedItemsPressed method
        GetNode<Button>("sellHarvested").Pressed += SellHarvestedItemsPressed;
    }

    // This method is called when a seed sell button is pressed
    private void SellSeed(string seedName)
    {
        // Get the SeedInventoryManager and MoneyManager nodes
        var seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
        var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");

        // Check if the seed exists in the inventory and has a sell price
        if (seedInventoryManager.GetSeedCount(seedName) > 0)
        {
            int sellPrice = seedPrices.ContainsKey(seedName) ? seedPrices[seedName] : 0;
            if (sellPrice > 0)
            {
                seedInventoryManager.SellSeed(seedName); // Remove the seed from inventory
                moneyManager.AddMoney(sellPrice); // Add money to the player
                textManager.Instance.showPopup($"Sold {seedName} for {sellPrice} coins.");
            }
            else
            {
                textManager.Instance.showPopup($"{seedName} has no assigned sell price.");
            }
        }
        else
        {
            textManager.Instance.showPopup($"No {seedName} seeds to sell.");
        }
    }

    // This method is called when the sell harvested items button is pressed
    private void SellHarvestedItemsPressed()
    {
        var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");
        var seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
        var harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
        int totalEarnings = 0;

        // Iterate through the harvested items and calculate total earnings
        foreach (var item in harvestManager.Inventory)
        {
            string itemName = item.Key;
            int itemCount = item.Value;
            if (harvestedPrices.ContainsKey(itemName) && itemCount > 0)
            {
                int sellPrice = harvestedPrices[itemName] * itemCount;
                totalEarnings += sellPrice;
            }
        }
        if (totalEarnings > 0)
        {
            moneyManager.AddMoney(totalEarnings); // Add total earnings to the player's money
            harvestManager.SellHarvestedItems(); // Clear the harvested items from the inventory
            textManager.Instance.showPopup($"Sold harvests for {totalEarnings} coins.");
        }
        else
        {
            textManager.Instance.showPopup("No harvested items to sell.");
        }
    }

}
