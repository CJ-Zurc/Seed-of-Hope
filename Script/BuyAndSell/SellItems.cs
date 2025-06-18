using Godot;
using System;

public partial class SellItems : Button
{
	private Button sellItemsButton;

    public override void _Ready()
	{
		sellItemsButton = GetNode<Button>("/root/Control/sellUI/CanvasLayer/sellUIBG/SellItems");
		sellItemsButton.Pressed += OnSellItemsButtonPressed;
    }

	private void OnSellItemsButtonPressed()
	{
		var sm = GetNode<SelectedSeedManager>("/root/SelectedSeedManager");
		var selectedSeed = sm.SelectedSeed; // Get the currently selected seed

        if (!String.IsNullOrEmpty(selectedSeed))
		{
			var SeedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
            SeedInventoryManager.SellOneOfEachSeed();
            GD.Print($"Selling {selectedSeed}...");
			
			sm.SelectedSeed = null; // Reset the selected seed after selling
		}
		else
		{
			GD.Print("No seed selected to sell.");
        }
        // Logic to sell items goes here
        GD.Print("Sell Items button pressed.");
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
