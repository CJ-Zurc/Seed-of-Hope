using Godot;
using System;

public partial class CollectableItems : Area2D
{
	[Export]
	public string CollectableItemName = "";
	private Label textPopUp;
	private TextureRect textbg;
	private Timer textDuration;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect the body entered signal to the OnBodyEntered method
		this.BodyEntered += OnBodyEntered;
	}


 

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			textManager.Instance.showPopup($"{CollectableItemName} collected.");
			HarvestManager harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
			harvestManager.AddCollectable(CollectableItemName);
			GetParent().QueueFree(); // Remove the collectable item from the scene
		}
	}
}
