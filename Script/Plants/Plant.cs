using Godot;
using System;

public partial class Plant : Node2D
{
    [Export]
    public PlantType Type { get; private set; }
    
    [Export]
    public int GrowthStage { get; private set; } = 0;
    
    [Export]
    public int MaxGrowthStage { get; set; } = 5;
    
    private PlantConfig _config;
    private TimeManager _timeManager;
    private float _currentWater = 0.0f;
    private int _lastWateredDay = -1;
    private int _lastWateredYear = -1;
    private double _plantedTime;
    private float _growthProgress = 0.0f;
    private MainGame _mainGame;
    
    private Sprite2D _sprite;
    private Area2D _interactionArea;
    private ProgressBar _waterBar;
    private Label _growthLabel;
    private Label _timeLabel;
    private PopupMenu _hoverMenu;
    private Sprite2D _descriptionBackground;
    private Label _descriptionLabel;
    
    private Inventory _playerInventory;
    
    private bool _isHarvested = false;
    
    private PlayerStamina _playerStamina;
    
    // Action availability
    public bool CanWater => CheckCanWater();
    public bool CanHarvest => _growthProgress >= 1.0f && GrowthStage >= MaxGrowthStage;
    public bool CanMove => _config.IsMoveable;
    
    private Sprite2D _waterBarSprite;  // New water bar sprite
    private const float HOURS_PER_WATER_LEVEL = 4.8f;  // 24 hours / 5 levels = 4.8 hours per level
    private const float SECONDS_PER_WATER_LEVEL = HOURS_PER_WATER_LEVEL * 3600f;  // Convert hours to seconds
    
    private bool CheckCanWater()
    {
        // Can water if it's a new day or never watered before
        if (_lastWateredDay == -1 || _lastWateredYear == -1) return true;
        
        // Get current day and year from MainGame
        int currentDay = _mainGame.dayCount;
        int currentYear = _mainGame.year;
        
        // Can water if it's a different day
        return currentDay != _lastWateredDay || currentYear != _lastWateredYear;
    }
    
    public override void _Ready()
    {
        _config = PlantConfig.GetConfig(Type);
        _timeManager = GetNode<TimeManager>("/root/TimeManager");
        _playerInventory = GetNode<Inventory>("/root/PlayerInventory");
        _mainGame = GetNode<MainGame>("/root/MainGame");
        _plantedTime = Time.GetUnixTimeFromSystem();
        
        // Set max growth stage based on plant type
        MaxGrowthStage = GetMaxGrowthStageForType();
        
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.AddToGroup("interactable");
        
        CreateUI();
        CreateHoverMenu();
        
        UpdateVisuals();
        
        // Get reference to the player's stamina system
        _playerStamina = GetNode<PlayerStamina>("/root/MainGame/PlayerStamina");
    }
    
    private int GetMaxGrowthStageForType()
    {
        return Type switch
        {
            PlantType.Ampalaya => 5,   // 5 growth stages (0-4) + harvested state (5) + fruit (6)
            PlantType.Calamansi => 5,  // 5 growth stages (0-4) + harvested state (5) + fruit (6)
            PlantType.Pechay => 3,     // 4 growth stages (0-3), last frame for inventory
            PlantType.Succulent => 3,   // 4 growth stages (0-3), last frame for inventory
            PlantType.Sunflower => 5,   // 5 growth stages (0-4) + stem state (5) + flower (6)
            _ => 3  // Default for other plants
        };
    }
    
    private void CreateUI()
    {
        Control uiContainer = new Control();
        uiContainer.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        uiContainer.Position = new Vector2(-50, -60);
        AddChild(uiContainer);
        
        _descriptionBackground = new Sprite2D();
        _descriptionBackground.Texture = GD.Load<Texture2D>("res://2D Arts/GardenAssets/plant description.png");
        _descriptionBackground.Position = new Vector2(100, 0);
        uiContainer.AddChild(_descriptionBackground);
        _descriptionBackground.Hide();
        
        VBoxContainer vbox = new VBoxContainer();
        vbox.CustomMinimumSize = new Vector2(100, 0);
        vbox.Position = new Vector2(_descriptionBackground.Position.X - 50, _descriptionBackground.Position.Y - 40);
        uiContainer.AddChild(vbox);
        
        Label nameLabel = new Label();
        nameLabel.Text = _config.Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AddThemeColorOverride("font_color", Colors.DarkGreen);
        nameLabel.AddThemeFontSizeOverride("font_size", 16);
        vbox.AddChild(nameLabel);
        
        _descriptionLabel = new Label();
        _descriptionLabel.Text = _config.Description;
        _descriptionLabel.HorizontalAlignment = HorizontalAlignment.Left;
        _descriptionLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _descriptionLabel.CustomMinimumSize = new Vector2(180, 0);
        _descriptionLabel.Position = new Vector2(10, 30);
        _descriptionBackground.AddChild(_descriptionLabel);
        
        VBoxContainer progressBarsContainer = new VBoxContainer();
        progressBarsContainer.CustomMinimumSize = new Vector2(100, 0);
        progressBarsContainer.Position = new Vector2(0, 100);
        vbox.AddChild(progressBarsContainer);
        
        HBoxContainer waterContainer = new HBoxContainer();
        waterContainer.CustomMinimumSize = new Vector2(100, 32); // Adjust size as needed
        
        TextureRect waterIcon = new TextureRect();
        waterIcon.Texture = GD.Load<Texture2D>("res://2D Arts/UI stuff/water_icon.png");
        waterIcon.CustomMinimumSize = new Vector2(16, 16);
        waterContainer.AddChild(waterIcon);
        
        _waterBarSprite = new Sprite2D();
        _waterBarSprite.Texture = GD.Load<Texture2D>("res://2D Arts/fullWaterBar.png");
        _waterBarSprite.Hframes = 5; // 5 frames for water levels
        _waterBarSprite.Frame = 0;
        
        SubViewportContainer waterBarContainer = new SubViewportContainer();
        waterBarContainer.CustomMinimumSize = new Vector2(80, 32);
        SubViewport waterBarViewport = new SubViewport();
        waterBarViewport.Size = new Vector2i(80, 32);
        waterBarViewport.TransparentBg = true;
        waterBarViewport.HandleInputLocally = false;
        waterBarContainer.AddChild(waterBarViewport);
        waterBarViewport.AddChild(_waterBarSprite);
        
        waterContainer.AddChild(waterBarContainer);
        progressBarsContainer.AddChild(waterContainer);
        
        _growthLabel = new Label();
        _growthLabel.Text = $"Growth: {GrowthStage}/{MaxGrowthStage}";
        progressBarsContainer.AddChild(_growthLabel);
        
        _timeLabel = new Label();
        UpdateTimeLabel();
        progressBarsContainer.AddChild(_timeLabel);
        
        uiContainer.Hide();
        _interactionArea.MouseEntered += () => {
            uiContainer.Show();
            _descriptionBackground.Show();
        };
        _interactionArea.MouseExited += () => {
            if (!_hoverMenu.Visible)
            {
                uiContainer.Hide();
                _descriptionBackground.Hide();
            }
        };
    }
    
    private void CreateHoverMenu()
    {
        _hoverMenu = new PopupMenu();
        _hoverMenu.AddThemeFontSizeOverride("font_size", 14);
        
        if (_config.IsMoveable)
        {
            _hoverMenu.AddItem("🔄 Move Plant", 0);
        }
        
        _hoverMenu.AddItem("💧 Water Plant", 1);
        _hoverMenu.AddItem("✂️ Harvest Plant", 2);
        
        _hoverMenu.IdPressed += OnHoverMenuItemSelected;
        AddChild(_hoverMenu);
        
        _interactionArea.MouseEntered += OnMouseEntered;
        _interactionArea.MouseExited += OnMouseExited;
        
        _hoverMenu.Modulate = new Color(1, 1, 1, 0.9f);
    }
    
    private void OnMouseEntered()
    {
        UpdateHoverMenu();
        Vector2 mousePos = GetViewport().GetMousePosition();
        _hoverMenu.Position = new Vector2(mousePos.X, mousePos.Y + 20);
        _hoverMenu.Popup();
    }
    
    private void OnMouseExited()
    {
        var timer = GetTree().CreateTimer(0.2);
        timer.Timeout += () =>
        {
            if (!_hoverMenu.GetRect().HasPoint(_hoverMenu.GetLocalMousePosition()))
            {
                _hoverMenu.Hide();
            }
        };
    }
    
    private void UpdateHoverMenu()
    {
        if (_config.IsMoveable)
        {
            _hoverMenu.SetItemDisabled(0, false);
        }
        
        bool canWater = CanWater;
        bool canHarvest = CanHarvest;
        
        _hoverMenu.SetItemDisabled(1, !canWater);
        _hoverMenu.SetItemDisabled(2, !canHarvest);
        
        if (!canWater)
        {
            _hoverMenu.SetItemTooltip(1, "Can water again tomorrow");
        }
        else
        {
            _hoverMenu.SetItemTooltip(1, "💧 Water your plant to help it grow!");
        }
        
        if (!canHarvest)
        {
            double daysLeft = _config.DaysToMature - (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
            _hoverMenu.SetItemTooltip(2, $"🕒 Ready to harvest in {Math.Max(0, daysLeft):F1} days");
        }
        else
        {
            _hoverMenu.SetItemTooltip(2, "✨ Plant is ready to harvest!");
        }
    }
    
    private void OnHoverMenuItemSelected(long id)
    {
        switch (id)
        {
            case 0:
                if (_config.IsMoveable)
                {
                    EmitSignal(SignalName.PlantMoveRequested, this);
                }
                break;
            case 1:
                if (CanWater) Water();
                break;
            case 2:
                if (CanHarvest) Harvest();
                break;
        }
        _hoverMenu.Hide();
    }
    
    public override void _Process(double delta)
    {
        float seasonWaterMultiplier = _timeManager.GetSeasonWaterRetentionMultiplier();
        float seasonGrowthMultiplier = _timeManager.GetSeasonGrowthMultiplier();
        
        // Calculate time since last watered
        double timeSinceWatered = Time.GetUnixTimeFromSystem() - _lastWateredTime;
        
        // Calculate water level (0-4)
        int waterLevel = 4 - (int)(timeSinceWatered / SECONDS_PER_WATER_LEVEL);
        waterLevel = Math.Max(0, Math.Min(4, waterLevel)); // Clamp between 0 and 4
        
        // Update water bar sprite frame
        if (_waterBarSprite != null)
        {
            _waterBarSprite.Frame = waterLevel;
        }
        
        // Calculate water percentage for growth
        _currentWater = (waterLevel + 1) * 20f; // Convert level (0-4) to percentage (20-100)
        
        if (_currentWater >= _config.WaterNeed * 0.5f && !_isHarvested)
        {
            float growthRate = _config.GrowthRate * seasonGrowthMultiplier;
            _growthProgress += (float)delta * growthRate / (_config.DaysToMature * 86400.0f);
            
            double daysPassed = (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
            int targetStage = CalculateGrowthStage(daysPassed);
            
            if (targetStage > GrowthStage && GrowthStage < MaxGrowthStage)
            {
                GrowthStage = targetStage;
                UpdateVisuals();
                GD.Print($"{_config.Name} grew to stage {GrowthStage}!");
            }
        }
        
        UpdateUI();
        UpdateTimeLabel();
    }
    
    private int CalculateGrowthStage(double daysPassed)
    {
        return Type switch
        {
            PlantType.Ampalaya => Math.Min((int)daysPassed, 5),   // 5 days to mature
            PlantType.Calamansi => Math.Min((int)daysPassed, 5),  // 7 days to mature, but 5 visual stages
            PlantType.Pechay => Math.Min((int)daysPassed, 3),     // 4 days to mature, 4 visual stages (0-3)
            PlantType.Succulent => Math.Min((int)daysPassed, 3),  // 4 days to mature, 4 visual stages (0-3)
            PlantType.Sunflower => Math.Min((int)daysPassed, 4),  // 4 days to mature, 5 visual stages (0-4)
            _ => Math.Min((int)(daysPassed * MaxGrowthStage / _config.DaysToMature), MaxGrowthStage)
        };
    }
    
    private void UpdateTimeLabel()
    {
        double daysSincePlanted = (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
        _timeLabel.Text = $"Age: {daysSincePlanted:F1} days";
    }
    
    public bool Water()
    {
        if (!CanBeWatered())
        {
            GD.Print($"Cannot water {_config.Name} right now!");
            return false;
        }
        
        if (_playerStamina == null || !_playerStamina.CanWaterPlant())
        {
            GD.Print("Not enough stamina to water the plant!");
            return false;
        }
        
        _currentWater = Math.Min(_currentWater + 50f, 100f);
        _lastWateredDay = MainGame.Instance.CurrentDay;
        UpdateWaterBar();
        
        // Consume stamina for watering
        _playerStamina.ConsumeWaterStamina();
        return true;
    }
    
    public void Harvest()
    {
        if (!CanHarvest)
        {
            GD.Print($"Cannot harvest {_config.Name} yet!");
            return;
        }
        
        if (_playerStamina == null || !_playerStamina.CanHarvest())
        {
            GD.Print("Not enough stamina to harvest!");
            return;
        }
        
        var harvestedItem = ItemData.CreateHarvest(Type);
        if (_playerInventory.AddItem(harvestedItem))
        {
            // Consume stamina for harvesting
            _playerStamina.ConsumeHarvestStamina();
            
            switch (Type)
            {
                case PlantType.Ampalaya:
                case PlantType.Calamansi:
                    _isHarvested = true;
                    GrowthStage = 5;  // Set to harvested state (6th frame)
                    UpdateVisuals();
                    GD.Print($"Harvested {_config.Name}! Plant will regrow.");
                    break;
                case PlantType.Sunflower:
                    _isHarvested = true;
                    GrowthStage = 5;  // Set to stem state (6th frame)
                    UpdateVisuals();
                    GD.Print($"Harvested {_config.Name}! Stem remains in soil.");
                    break;
                case PlantType.Pechay:
                case PlantType.Succulent:
                    // Show the harvested frame briefly before removing
                    GrowthStage = 3;  // Last frame is the harvested state
                    UpdateVisuals();
                    GD.Print($"Harvested {_config.Name}!");
                    // Add a small delay before removing the plant
                    var timer = GetTree().CreateTimer(0.5);
                    timer.Timeout += () => QueueFree();
                    break;
                default:
                    GrowthStage = 0;
                    _growthProgress = 0;
                    _currentWater = 0;
                    _plantedTime = Time.GetUnixTimeFromSystem();
                    UpdateVisuals();
                    GD.Print($"Harvested {_config.Name}!");
                    break;
            }
            UpdateUI();
        }
        else
        {
            GD.Print("Inventory is full!");
        }
    }
    
    private void UpdateUI()
    {
        if ((Type == PlantType.Ampalaya || Type == PlantType.Calamansi) && _isHarvested)
        {
            _growthLabel.Text = "Harvested - Will regrow";
        }
        else if (Type == PlantType.Sunflower && _isHarvested)
        {
            _growthLabel.Text = "Harvested - Stem remains";
        }
        else
        {
            double daysPassed = (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
            double daysToMature = Type switch
            {
                PlantType.Ampalaya => 5,
                PlantType.Calamansi => 7,
                PlantType.Pechay => 4,
                PlantType.Succulent => 4,
                PlantType.Sunflower => 4,
                _ => _config.DaysToMature
            };
            
            if (GrowthStage >= MaxGrowthStage)
            {
                switch (Type)
                {
                    case PlantType.Pechay:
                    case PlantType.Succulent:
                        _growthLabel.Text = "Ready to harvest!\nWill be removed after harvest";
                        break;
                    case PlantType.Sunflower:
                        _growthLabel.Text = "Ready to harvest!\nStem will remain";
                        break;
                    default:
                        _growthLabel.Text = "Ready to harvest!";
                        break;
                }
            }
            else
            {
                double daysLeft = daysToMature - daysPassed;
                int totalStages = Type == PlantType.Sunflower ? 5 : 4;
                _growthLabel.Text = $"Growth: {GrowthStage + 1}/{totalStages}\nDays until mature: {Math.Max(0, daysLeft):F1}";
            }
        }
    }
    
    private void UpdateVisuals()
    {
        switch (Type)
        {
            case PlantType.Ampalaya:
            case PlantType.Calamansi:
                // For Ampalaya and Calamansi:
                // Frames 0-4: Growing stages
                // Frame 5: Harvested state (plant/tree only)
                // Frame 6: Harvested fruit (used in inventory)
                _sprite.Frame = _isHarvested ? 5 : GrowthStage;
                break;
            case PlantType.Sunflower:
                // For Sunflower:
                // Frames 0-4: Growing stages
                // Frame 5: Stem state after harvest
                // Frame 6: Harvested flower (used in inventory)
                _sprite.Frame = _isHarvested ? 5 : GrowthStage;
                break;
            case PlantType.Pechay:
            case PlantType.Succulent:
                // For Pechay and Succulent:
                // Frames 0-2: Growing stages
                // Frame 3: Final/Harvested state (also used for inventory)
                _sprite.Frame = GrowthStage;
                break;
            default:
                _sprite.Frame = GrowthStage;
                break;
        }
    }
    
    public float GetCurrentWater() => _currentWater;
    public double GetPlantedTime() => _plantedTime;
    public float GetGrowthProgress() => _growthProgress;
    
    public void LoadSaveData(
        int growthStage,
        float currentWater,
        int lastWateredDay,
        int lastWateredYear,
        double plantedTime,
        float growthProgress,
        bool isHarvested = false)
    {
        GrowthStage = growthStage;
        _currentWater = currentWater;
        _lastWateredDay = lastWateredDay;
        _lastWateredYear = lastWateredYear;
        _plantedTime = plantedTime;
        _growthProgress = growthProgress;
        _isHarvested = isHarvested;
        
        UpdateVisuals();
        UpdateUI();
    }
    
    [Signal]
    public delegate void PlantMoveRequestedEventHandler(Plant plant);
} 