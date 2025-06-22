using Godot;
using System;

public partial class PlantArea2D : PlantableArea
{
	[Export]
	public NodePath plantAnimationPath;
	private AnimatedSprite2D plantAnimation;
	private string currentSeedType = null;
	private float lastGrowthDay = 0;
	private ProgressBar waterBar;
	private bool isPlanted = false;
	private MainGame mainGame;
	private SeedInventoryManager seedInventoryManager;
	private InventorySeedsPanel inventorySeedsPanel;
	private InventoryPanel inventoryPanel;
	private bool wateringCanActive = false;

	public override void _Ready()
	{
		plantAnimation = GetNode<AnimatedSprite2D>(plantAnimationPath);
		mainGame = GetNode<MainGame>("/root/MainGame");
		seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
		inventorySeedsPanel = GetNode<InventorySeedsPanel>("/root/MainGame/HUD/Control/inventorySeedsPanel");
		inventoryPanel = GetNode<InventoryPanel>("/root/MainGame/HUD/Control/inventoryPanel");
		waterBar = null;
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));
	}

	public override void Plant(string seedType)
	{
		if (isPlanted) return;
		if (string.IsNullOrEmpty(seedType)) return;
		if (seedInventoryManager.GetSeedCount(seedType) <= 0) return;

		currentSeedType = seedType;
		isPlanted = true;

		switch (seedType)
		{
			case "Ampalaya":
				plantAnimation.Animation = "ampalayaanimation";
				plantAnimation.Frame = 0;
				break;
			case "Calamansi":
				plantAnimation.Animation = "calamansianimation";
				plantAnimation.Frame = 0;
				break;
			case "Pechay":
				plantAnimation.Animation = "pechayanimation";
				plantAnimation.Frame = 0;
				break;
			case "Succulent":
				plantAnimation.Animation = "succulentanimation";
				plantAnimation.Frame = 0;
				break;
			case "Sunflower":
				plantAnimation.Animation = "sunfloweranimation";
				plantAnimation.Frame = 0;
				break;
			default:
				return;
		}

		lastGrowthDay = mainGame.GetDayCount();
		seedInventoryManager.SellSeed(seedType);
		inventorySeedsPanel.SetSeedSelectedFalse(seedType);
		ShowPlantInfo();
	}

	public override void Water()
	{
		// Reset: Only set up the basic structure for watering
	}

	private void OnInputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (!isPlanted && !string.IsNullOrEmpty(inventorySeedsPanel.SelectedSeed))
			{
				Plant(inventorySeedsPanel.SelectedSeed);
			}
			else if (isPlanted)
			{
				ShowPlantInfo();
			}
		}
	}

	private void ShowPlantInfo()
	{
		// Reset: Only set up the basic structure for showing plant info
	}

	public override void _Process(double delta)
	{
		if (isPlanted && mainGame.GetDayCount() > lastGrowthDay)
		{
			plantAnimation.Frame++;
			lastGrowthDay = mainGame.GetDayCount();
		}
		if (waterBar != null && waterBar.Value > 0)
		{
			// Simulate growth if watered
		}
	}

	public void SetWateringCanActive(bool active)
	{
		wateringCanActive = active;
	}
}
