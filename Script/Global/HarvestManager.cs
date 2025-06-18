using Godot;
using System;
using System.Collections.Generic;

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
}
