using Godot;
using System;

public partial class CollectableItems : Area2D
{
	[Export]
	public string CollectableItemName = "";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Connect the body entered signal to the OnBodyEntered method
		this.BodyEntered += OnBodyEntered;
		GD.Print($"{CollectableItemName} collectable item ready.");
    }

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{

			GD.Print($"{CollectableItemName} collected by {player.Name}.");

			HarvestManager harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
			harvestManager.AddCollectable(CollectableItemName);
			GetParent().QueueFree(); // Remove the collectable item from the scene
        }
		else
		{
			GD.Print($"{CollectableItemName} was not collected by a player.");
		}
    }
}
