using Godot;
using System;

public partial class PlantArea2D : PlantableArea
{
	[Export]
	public NodePath plantAnimationPath;
	private AnimatedSprite2D plantAnimation;
	private Timer plantInfoTimer;
	private Control plantInfoInstance;
	private string currentSeedType = null;
	private float lastGrowthDay = 0;
	private ProgressBar waterBar;
	private bool isPlanted = false;
	private MainGame mainGame;
	private SeedInventoryManager seedInventoryManager;
	private InventorySeedsPanel inventorySeedsPanel;
	private InventoryPanel inventoryPanel;
	private bool wateringCanActive = false;
	private int growthDaysLeft = 5;
	private bool waitingForWaterClick = false;
	private float waterLevel = 0f;
	private float lastWaterDecreaseHour = 0f;

	public override void _Ready()
	{
		plantAnimation = GetNode<AnimatedSprite2D>(plantAnimationPath);
		mainGame = GetNode<MainGame>("/root/MainGame");
		seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
		inventorySeedsPanel = GetNode<InventorySeedsPanel>("/root/MainGame/HUD/Control/inventorySeedsPanel");
		inventoryPanel = GetNode<InventoryPanel>("/root/MainGame/HUD/Control/inventoryPanel");
		waterBar = null;
		Connect("input_event", new Callable(this, nameof(OnInputEvent)));

		if (inventoryPanel != null)
		{
			inventoryPanel.WaterButtonPressed += OnWaterButtonActivated;
		}
	}

	public override void Plant(string seedType)
	{
		if (isPlanted) return;
		if (string.IsNullOrEmpty(seedType)) return;
		if (seedInventoryManager.GetSeedCount(seedType) <= 0) return;

		currentSeedType = seedType;
		isPlanted = true;
		growthDaysLeft = 5; // Set growth stage to 5 days
		waterLevel = 0f; // Reset water level on planting
		lastWaterDecreaseHour = mainGame.GetHourCount(); // Track water decrease timing

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
		mainGame.DecreaseStamina(0.1f); // Decrease stamina by 10% per plant
		ShowPlantInfo();
	}

	public override void Water()
	{
		// Not used directly; watering is handled via OnInputEvent
	}

	private void OnWaterButtonActivated()
	{
		waitingForWaterClick = true;
	}

	private void OnInputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (!isPlanted && !string.IsNullOrEmpty(inventorySeedsPanel.SelectedSeed))
			{
				Plant(inventorySeedsPanel.SelectedSeed);
			}
			// Only allow watering if wateringCanActive is true in InventoryPanel
			else if (isPlanted && waitingForWaterClick && inventoryPanel != null && inventoryPanel.Get("wateringCanActive").AsBool())
			{
				waterLevel = 1.0f;
				ShowPlantInfo();
				waitingForWaterClick = false;
				if (inventoryPanel != null)
					inventoryPanel.SetWateringCanActive(false);
				Input.SetDefaultCursorShape(Input.CursorShape.Arrow);
				mainGame.DecreaseStamina(0.1f); // Decrease stamina by 10% on watering
			}
			else if (isPlanted)
			{
				ShowPlantInfo();
			}
		}
	}

	private void ShowPlantInfo()
	{
		if (plantInfoInstance != null && IsInstanceValid(plantInfoInstance))
		{
			plantInfoInstance.QueueFree();
		}

		var plantInfoScene = GD.Load<PackedScene>("res://scenes/plant_info.tscn");
		plantInfoInstance = (Control)plantInfoScene.Instantiate();

		// Set plant name
		var plantNameLabel = plantInfoInstance.GetNode<RichTextLabel>("CanvasLayer/Control/plantName");
		plantNameLabel.Text = currentSeedType;

		// Set growth info
		var growthLabel = plantInfoInstance.GetNode<RichTextLabel>("CanvasLayer/Control/RichTextLabel");
		if (growthDaysLeft > 0)
		{
			growthLabel.Text = $"Growth: {growthDaysLeft} Days";
		}
		else
		{
			growthLabel.Text = "Ready to Harvest!";
		}

		// Set water bar value
		var waterBarNode = plantInfoInstance.GetNode<TextureProgressBar>("CanvasLayer/Control/WaterBar/TextureProgressBar");
		waterBarNode.Value = waterLevel * 100f;

		// Add to HUD (assumes HUD is at /root/MainGame/HUD)
		var hud = GetNode<CanvasLayer>("/root/MainGame/HUD");
		hud.AddChild(plantInfoInstance);

		// Position: bottom middle, just above the seed inventory
		plantInfoInstance.AnchorLeft = 0.5f;
		plantInfoInstance.AnchorRight = 0.5f;
		plantInfoInstance.AnchorBottom = 1.0f;
		plantInfoInstance.AnchorTop = 1.0f;
		plantInfoInstance.OffsetLeft = -plantInfoInstance.Size.X / 2f;
		plantInfoInstance.OffsetTop = -180; // Adjust as needed to be above inventory

		// Auto-hide after 1.5 seconds
		if (plantInfoTimer == null)
		{
			plantInfoTimer = new Timer();
			plantInfoTimer.OneShot = true;
			plantInfoTimer.WaitTime = 1.5f;
			plantInfoTimer.Timeout += () => {
				if (plantInfoInstance != null && IsInstanceValid(plantInfoInstance))
					plantInfoInstance.QueueFree();
			};
			hud.AddChild(plantInfoTimer);
		}
		plantInfoTimer.Start();
	}

	public override void _Process(double delta)
	{
		if (isPlanted && mainGame.GetDayCount() > lastGrowthDay)
		{
			if (growthDaysLeft > 0)
			{
				growthDaysLeft--;
			}
			plantAnimation.Frame++;
			lastGrowthDay = mainGame.GetDayCount();
		}

		// Water decrease logic: decrease by 20% every 5 in-game hours
		if (isPlanted)
		{
			float currentHour = mainGame.GetHourCount();
			if (currentHour - lastWaterDecreaseHour >= 5f)
			{
				if (waterLevel > 0f)
				{
					waterLevel -= 0.2f;
					if (waterLevel < 0f) waterLevel = 0f;
				}
				lastWaterDecreaseHour += 5f;
				// Optionally update info panel if visible
				if (plantInfoInstance != null && IsInstanceValid(plantInfoInstance))
				{
					var waterBarNode = plantInfoInstance.GetNode<TextureProgressBar>("CanvasLayer/Control/WaterBar/TextureProgressBar");
					waterBarNode.Value = waterLevel * 100f;
				}
			}
		}
	}

	public void SetWateringCanActive(bool active)
	{
		wateringCanActive = active;
	}
}
