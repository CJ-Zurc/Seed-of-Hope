using Godot;
using System;
using System.Collections.Generic;
public partial class SeedInventoryManager : Node
{
    [Signal]
    public delegate void InventoryChangedEventHandler();

    private Dictionary<string, int> seedInventory = new Dictionary<string, int>();

    public IReadOnlyDictionary<string, int> Inventory => seedInventory;

    public void BuySeed(String seedName)
    {
        if (seedInventory.ContainsKey(seedName))
        {
            seedInventory[seedName] += 1;
        }
        else
        {
            seedInventory[seedName] = 1;
        }
        EmitSignal(nameof(InventoryChanged));
    }

    public void SellSeed(string seedName)
    {
        if (seedInventory.ContainsKey(seedName) && seedInventory[seedName] > 0)
        {
            seedInventory[seedName] -= 1;
            EmitSignal(nameof(InventoryChanged));
        }
    }

    public int GetSeedCount(string seedName)
    {
        var seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
        return seedInventoryManager.Inventory.ContainsKey(seedName) ? seedInventoryManager.Inventory[seedName] : 0;
    }
}
