using Godot;
using System;

namespace SeedOfHope.Script.Plant
{
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
		private string collectableScenePath = "";
		private bool spawnedCollectable = false;

		[Export]
		private string _uniqueID = "";
		public override string UniqueID => _uniqueID;

		[Export]
		public float x = 0;
		[Export]
		public float y = 0;

		private Timer autosaveTimer;
		private const float AUTOSAVE_INTERVAL = 10f; // seconds

		public override string CurrentSeedType => currentSeedType;
		public override int GrowthDaysLeft => growthDaysLeft;
		public override float WaterLevel => waterLevel;
		public override bool IsPlanted => isPlanted;

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
			moneyHUD = GetNode<Money>("/root/MainGame/HUD");

			// Autosave timer setup
			autosaveTimer = new Timer();
			autosaveTimer.WaitTime = AUTOSAVE_INTERVAL;
			autosaveTimer.OneShot = false;
			autosaveTimer.Timeout += OnAutosaveTimeout;
			AddChild(autosaveTimer);
			autosaveTimer.Start();

			// Load state on ready
			LoadPlantState();
		}

		private void OnAutosaveTimeout()
		{
			SavePlantState();
		}

		private string GetPlantSavePath()
		{
			return "user://plantsave.json";
		}

		private void SavePlantState()
		{
			// 1. Load all existing data
			Godot.Collections.Dictionary allData = new Godot.Collections.Dictionary();
			if (FileAccess.FileExists(GetPlantSavePath()))
			{
				using var file = FileAccess.Open(GetPlantSavePath(), FileAccess.ModeFlags.Read);
				var json = file.GetAsText();
				var parsed = Json.ParseString(json);
				if (parsed.VariantType == Variant.Type.Dictionary)
					allData = parsed.AsGodotDictionary();
			}

			// 2. Update only this plant's entry
			allData[Name] = SaveState();

			// 3. Write the whole dictionary back
			using var saveFile = FileAccess.Open(GetPlantSavePath(), FileAccess.ModeFlags.Write);
			saveFile.StoreString(Json.Stringify(allData));
			saveFile.Flush(); // <--- Add this
			GD.Print($"Saving plant: {Name} ({GetType()})");
		}

		private void LoadPlantState()
		{
			if (!FileAccess.FileExists(GetPlantSavePath()))
				return;

			using var file = FileAccess.Open(GetPlantSavePath(), FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var parsed = Json.ParseString(json);
			if (parsed.VariantType != Variant.Type.Dictionary)
				return;

			var allData = parsed.AsGodotDictionary();
			if (allData.ContainsKey(Name))
			{
				var plantData = allData[Name].AsGodotDictionary();
				LoadState(plantData);
			}
		}

		public override void Plant(string seedType)
		{
			if (isPlanted) return;
			if (string.IsNullOrEmpty(seedType)) return;
			if (seedInventoryManager.GetSeedCount(seedType) <= 0) return;

			currentSeedType = seedType;
			isPlanted = true;
			growthDaysLeft = 5;
			waterLevel = 0f;
			lastWaterDecreaseHour = mainGame.GetHourCount();
			spawnedCollectable = false;

			switch (seedType)
			{
				case "Ampalaya":
					plantAnimation.Animation = "ampalayaanimation";
					plantAnimation.Frame = 0;
					collectableScenePath = "res://scenes/ampalayaCollectable.tscn";
					break;
				case "Calamansi":
					plantAnimation.Animation = "calamansianimation";
					plantAnimation.Frame = 0;
					collectableScenePath = "res://scenes/calamansiCollectable.tscn";
					break;
				case "Pechay":
					plantAnimation.Animation = "pechayanimation";
					plantAnimation.Frame = 0;
					collectableScenePath = "res://scenes/pechayCollectable.tscn";
					break;
				case "Succulent":
					plantAnimation.Animation = "succulentanimation";
					plantAnimation.Frame = 0;
					collectableScenePath = "res://scenes/succulentCollectable.tscn";
					break;
				case "Sunflower":
					plantAnimation.Animation = "sunfloweranimation";
					plantAnimation.Frame = 0;
					collectableScenePath = "res://scenes/sunflowerCollectable.tscn";
					break;
				default:
					return;
			}

			lastGrowthDay = mainGame.GetDayCount();
			seedInventoryManager.SellSeed(seedType);
			mainGame.DecreaseStamina(0.1f);
			ShowPlantInfo();
			SavePlantState(); // Save immediately after planting
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
				else if (isPlanted && waitingForWaterClick && inventoryPanel != null && inventoryPanel.Get("wateringCanActive").AsBool())
				{
					waterLevel = 1.0f;
					ShowPlantInfo();
					waitingForWaterClick = false;
					moneyHUD.UseWateringCan();
					SavePlantState(); // Save after watering
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
				SavePlantState(); // Save after growth
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
					SavePlantState(); // Save after water decrease
				}
			}

			// When ready to harvest, spawn collectable if not already done
			if (isPlanted && growthDaysLeft <= 0 && !spawnedCollectable && !string.IsNullOrEmpty(collectableScenePath))
			{
				var collectableScene = GD.Load<PackedScene>(collectableScenePath);
				if (collectableScene != null)
				{
					var collectable = collectableScene.Instantiate();

					if (collectable is Sprite2D collectableNode)
					{
						GetParent().AddChild(collectableNode);
						collectableNode.GlobalPosition = new Vector2(x, y);
						collectableNode.ZIndex = 10; // Ensure it's on top		
						GD.Print($"Spawning collectable at {collectableNode.Position}");
					}
				}
				spawnedCollectable = true;

				// Change plant animation to "empty" or "harvested"
				plantAnimation.Animation = "none"; // Make sure you have an "empty" or "harvested" animation
				plantAnimation.Frame = 0;  

				// Mark area as not planted so it can be replanted
				isPlanted = false;
				SavePlantState(); // Save after harvest
			}
		
		}

		public void SetWateringCanActive(bool active)
		{
			wateringCanActive = active;
		}

		// Implement abstract methods for SaveState and LoadState, but leave them empty for now
		public override Godot.Collections.Dictionary SaveState()
		{
			var dict = new Godot.Collections.Dictionary
			{
				["current_seed_type"] = currentSeedType,
				["is_planted"] = isPlanted,
				["water_level"] = waterLevel,
				["growth_days_left"] = growthDaysLeft,
				["last_growth_day"] = lastGrowthDay,
				["last_water_decrease_hour"] = lastWaterDecreaseHour,
				["spawned_collectable"] = spawnedCollectable,
				["collectable_scene_path"] = collectableScenePath,
				["x"] = x,
				["y"] = y,
				["animation"] = plantAnimation != null ? plantAnimation.Animation : "",
				["animation_frame"] = plantAnimation != null ? plantAnimation.Frame : 0
			};
			return dict;
		}

		public override void LoadState(Godot.Collections.Dictionary data)
		{
			if (data == null) return;

			if (data.ContainsKey("current_seed_type"))
				currentSeedType = (string)data["current_seed_type"];
			if (data.ContainsKey("is_planted"))
				isPlanted = (bool)data["is_planted"];
			if (data.ContainsKey("water_level"))
				waterLevel = (float)(double)data["water_level"];
			if (data.ContainsKey("growth_days_left"))
				growthDaysLeft = (int)(long)data["growth_days_left"];
			if (data.ContainsKey("last_growth_day"))
				lastGrowthDay = (float)(double)data["last_growth_day"];
			if (data.ContainsKey("last_water_decrease_hour"))
				lastWaterDecreaseHour = (float)(double)data["last_water_decrease_hour"];
			if (data.ContainsKey("spawned_collectable"))
				spawnedCollectable = (bool)data["spawned_collectable"];
			if (data.ContainsKey("collectable_scene_path"))
				collectableScenePath = (string)data["collectable_scene_path"];
			if (data.ContainsKey("x"))
				x = (float)(double)data["x"];
			if (data.ContainsKey("y"))
				y = (float)(double)data["y"];
			if (data.ContainsKey("animation") && plantAnimation != null)
				plantAnimation.Animation = (string)data["animation"];
			if (data.ContainsKey("animation_frame") && plantAnimation != null)
				plantAnimation.Frame = (int)(long)data["animation_frame"];
		}

		private Money moneyHUD;
	}
}
