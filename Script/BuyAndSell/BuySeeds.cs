using Godot;
using System;

public partial class BuySeeds : Control
{
    //storesthe reference of the SeedInventoryManager
    private SeedInventoryManager seedInventoryManager;



    
    public override void _Ready()
    {
        // Get the SeedInventoryManager instance from the scene tree
        seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");


        GD.Print("This node: ", Name);
        GD.Print("Full path: ", GetPath());
        // Connect the button pressed signals to the corresponding methods
        GetNode<Control>("/root/Control");
        GetNode<Button>("CanvasLayer/SappyBG/buySucculent").Pressed += () => onBuySeedButtonPressed("Succulent");
        GetNode<Control>("/root/Control");
        GetNode<Button>("CanvasLayer/SappyBG/buyAmpalaya").Pressed += () => onBuySeedButtonPressed("Ampalaya");
        GetNode<Control>("/root/Control");
        GetNode<Button>("CanvasLayer/SappyBG/buyCalamansi").Pressed += () => onBuySeedButtonPressed("Calamansi");
        GetNode<Control>("/root/Control");
        GetNode<Button>("CanvasLayer/SappyBG/buyPechay").Pressed += () => onBuySeedButtonPressed("Pechay");
        GetNode<Control>("/root/Control");
        GetNode<Button>("CanvasLayer/SappyBG/buySunflower").Pressed += () => onBuySeedButtonPressed("Sunflower");
    }
    private void onBuySeedButtonPressed(string SeedName)
    {
        seedInventoryManager.BuySeed(SeedName);
        GD.Print($"Bought seed: {SeedName}");
    }
    
}
