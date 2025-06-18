using Godot;
using System;
using System.Collections.Generic;
public partial class SeedInventoryManager : Node
{
    [Signal]
    public delegate void InventoryChangedEventHandler();

    public Dictionary<string,int> Inventory = new Dictionary<string, int>();

    public void AddSeed(String seedName) 
    {
        if (Inventory.ContainsKey(seedName))
        {
            Inventory[seedName] += 1;
        }
        else
        {
            Inventory[seedName] = 1;
        }
        EmitSignal(nameof(InventoryChanged));
    }

    public void RemoveSeed(String seedName) 
    {
        if(Inventory.ContainsKey(seedName) && Inventory[seedName] >0)
        {
            Inventory[seedName] -= 1;
            EmitSignal(nameof(InventoryChanged));
        }
    }
}
