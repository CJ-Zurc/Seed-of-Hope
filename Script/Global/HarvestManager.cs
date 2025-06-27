using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HarvestManager : Node
{
	//signal to notify when the inventory changes
	[Signal]
	public delegate void InventoryChangedEventHandler();

    //dictionary to hold the item name and the count
    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> Inventory => inventory;

    public void AddCollectable(string CollectableName)
	{
        // Check if the collectable already exists in the inventory
        if (Inventory.ContainsKey(CollectableName))
		{
            // If it exists, increment the count
            inventory[CollectableName] += 1;
		}
		else
		{
            // If it doesn't exist, add it with a count of 1
            inventory[CollectableName] = 1;
		}
        // Emit the signal to notify that the inventory has changed
        EmitSignal(nameof(InventoryChanged));
	}

    //Method to sell all of the harvested items
    public void SellHarvestedItems()
    {
        foreach(var key in inventory.Keys.ToList()) //creates a copy of the keys in the dictionary that will be passed in the UI script
        {
            inventory[key] = 0; // Set the count to 0 for each item
        }
        EmitSignal(nameof(InventoryChanged)); // Emit the signal to notify that the inventory has changed
    }

    // Save the inventory to a Godot dictionary for JSON serialization
    public Godot.Collections.Dictionary ToDictionary()
    {
        var dict = new Godot.Collections.Dictionary();
        foreach (var kvp in inventory)
        {
            dict[kvp.Key] = kvp.Value;
        }
        return dict;
    }

    // Load the inventory from a Godot dictionary (from JSON)
    public void FromDictionary(Godot.Collections.Dictionary dict)
    {
        inventory.Clear();
        foreach (var key in dict.Keys)
        {
            inventory[key.ToString()] = (int)dict[key];
        }
        EmitSignal(nameof(InventoryChanged));
    }

} 