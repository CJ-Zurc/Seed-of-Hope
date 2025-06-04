using Godot;
using System;

public partial class Plant : Node2D
{
    [Export]
    public PlantType Type { get; private set; }
    
    [Export]
    public int GrowthStage { get; private set; } = 0;
    
    [Export]
    public int MaxGrowthStage { get; set; } = 3;
    
    private PlantConfig _config;
    private TimeManager _timeManager;
    private float _currentWater = 0.0f;
    private float _currentSunlight = 0.0f;
    private bool _isInSunlight = false;
    private bool _isFertilized = false;
    private double _lastWateredTime = -86400.0;
    private double _lastFertilizedTime = -86400.0;
    private double _plantedTime;
    private float _growthProgress = 0.0f;
    
    private Sprite2D _sprite;
    private Area2D _interactionArea;
    private ProgressBar _waterBar;
    private ProgressBar _sunlightBar;
    private Label _growthLabel;
    private Label _timeLabel;
    private PopupMenu _hoverMenu;
    
    private Inventory _playerInventory;
    
    // Action availability
    public bool CanWater => (Time.GetUnixTimeFromSystem() - _lastWateredTime) >= 86400.0;
    public bool CanFertilize => (Time.GetUnixTimeFromSystem() - _lastFertilizedTime) >= 86400.0;
    public bool CanHarvest => _growthProgress >= 1.0f && GrowthStage >= MaxGrowthStage;
    public bool CanMove => _config.IsMoveable;
    
    public override void _Ready()
    {
        _config = PlantConfig.GetConfig(Type);
        _timeManager = GetNode<TimeManager>("/root/TimeManager");
        _playerInventory = GetNode<Inventory>("/root/PlayerInventory");
        _plantedTime = Time.GetUnixTimeFromSystem();
        
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.AddToGroup("interactable");
        
        CreateUI();
        CreateHoverMenu();
        
        UpdateVisuals();
    }
    
    private void CreateUI()
    {
        Control uiContainer = new Control();
        uiContainer.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        uiContainer.Position = new Vector2(-50, -60);
        AddChild(uiContainer);
        
        VBoxContainer vbox = new VBoxContainer();
        vbox.CustomMinimumSize = new Vector2(100, 0);
        uiContainer.AddChild(vbox);
        
        // Plant Name
        Label nameLabel = new Label();
        nameLabel.Text = _config.Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        vbox.AddChild(nameLabel);
        
        // Water Bar
        _waterBar = new ProgressBar();
        _waterBar.MaxValue = _config.WaterNeed;
        _waterBar.Value = _currentWater;
        _waterBar.CustomMinimumSize = new Vector2(100, 10);
        Label waterLabel = new Label();
        waterLabel.Text = "Water";
        vbox.AddChild(waterLabel);
        vbox.AddChild(_waterBar);
        
        // Sunlight Bar
        _sunlightBar = new ProgressBar();
        _sunlightBar.MaxValue = _config.SunlightNeed;
        _sunlightBar.Value = _currentSunlight;
        _sunlightBar.CustomMinimumSize = new Vector2(100, 10);
        Label sunlightLabel = new Label();
        sunlightLabel.Text = "Sunlight";
        vbox.AddChild(sunlightLabel);
        vbox.AddChild(_sunlightBar);
        
        // Growth Label
        _growthLabel = new Label();
        _growthLabel.Text = $"Growth: {GrowthStage}/{MaxGrowthStage}";
        vbox.AddChild(_growthLabel);
        
        // Time Label
        _timeLabel = new Label();
        UpdateTimeLabel();
        vbox.AddChild(_timeLabel);
    }
    
    private void CreateHoverMenu()
    {
        _hoverMenu = new PopupMenu();
        
        if (_config.IsMoveable)
        {
            _hoverMenu.AddItem("Move Plant", 0);
        }
        
        _hoverMenu.AddItem("Water Plant", 1);
        _hoverMenu.AddItem("Fertilize Plant", 2);
        _hoverMenu.AddItem("Harvest Plant", 3);
        
        _hoverMenu.IdPressed += OnHoverMenuItemSelected;
        AddChild(_hoverMenu);
        
        // Connect mouse events
        _interactionArea.MouseEntered += OnMouseEntered;
        _interactionArea.MouseExited += OnMouseExited;
    }
    
    private void OnMouseEntered()
    {
        UpdateHoverMenu();
        Vector2 mousePos = GetViewport().GetMousePosition();
        _hoverMenu.Position = mousePos;
        _hoverMenu.Popup();
    }
    
    private void OnMouseExited()
    {
        if (!_hoverMenu.GetRect().HasPoint(_hoverMenu.GetLocalMousePosition()))
        {
            _hoverMenu.Hide();
        }
    }
    
    private void UpdateHoverMenu()
    {
        if (_config.IsMoveable)
        {
            _hoverMenu.SetItemDisabled(0, false);
        }
        
        _hoverMenu.SetItemDisabled(1, !CanWater);
        _hoverMenu.SetItemDisabled(2, !CanFertilize);
        _hoverMenu.SetItemDisabled(3, !CanHarvest);
        
        // Update tooltips
        if (!CanWater)
        {
            double timeUntilWater = 86400.0 - (Time.GetUnixTimeFromSystem() - _lastWateredTime);
            _hoverMenu.SetItemTooltip(1, $"Can water again in {TimeSpan.FromSeconds(timeUntilWater):hh\\:mm\\:ss}");
        }
        
        if (!CanFertilize)
        {
            double timeUntilFertilize = 86400.0 - (Time.GetUnixTimeFromSystem() - _lastFertilizedTime);
            _hoverMenu.SetItemTooltip(2, $"Can fertilize again in {TimeSpan.FromSeconds(timeUntilFertilize):hh\\:mm\\:ss}");
        }
        
        if (!CanHarvest)
        {
            double daysLeft = _config.DaysToMature - (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
            _hoverMenu.SetItemTooltip(3, $"Ready to harvest in {Math.Max(0, daysLeft):F1} days");
        }
    }
    
    private void OnHoverMenuItemSelected(long id)
    {
        switch (id)
        {
            case 0: // Move
                if (_config.IsMoveable)
                {
                    EmitSignal(SignalName.PlantMoveRequested, this);
                }
                break;
            case 1: // Water
                if (CanWater) Water();
                break;
            case 2: // Fertilize
                if (CanFertilize) Fertilize();
                break;
            case 3: // Harvest
                if (CanHarvest) Harvest();
                break;
        }
        _hoverMenu.Hide();
    }
    
    public override void _Process(double delta)
    {
        // Apply season effects
        float seasonWaterMultiplier = _timeManager.GetSeasonWaterRetentionMultiplier();
        float seasonGrowthMultiplier = _timeManager.GetSeasonGrowthMultiplier();
        
        // Update water level with season effect
        _currentWater = Math.Max(0, _currentWater - (float)delta * _config.WaterDepletionRate / seasonWaterMultiplier);
        
        // Update sunlight level
        if (_isInSunlight && _timeManager.IsDaytime())
        {
            _currentSunlight = Math.Min(_config.SunlightNeed, _currentSunlight + (float)delta * 0.2f);
        }
        else
        {
            _currentSunlight = Math.Max(0, _currentSunlight - (float)delta * 0.1f);
        }
        
        // Update growth progress
        if (_currentWater >= _config.WaterNeed * 0.5f && 
            _currentSunlight >= _config.SunlightNeed * 0.5f)
        {
            float growthRate = _config.GrowthRate * seasonGrowthMultiplier;
            if (_isFertilized)
            {
                growthRate *= _config.FertilizerEfficiency;
            }
            
            _growthProgress += (float)delta * growthRate / (_config.DaysToMature * 86400.0f);
            
            if (_growthProgress >= (GrowthStage + 1) / (float)MaxGrowthStage && GrowthStage < MaxGrowthStage)
            {
                GrowthStage++;
                UpdateVisuals();
                GD.Print($"{_config.Name} grew to stage {GrowthStage}!");
            }
        }
        
        UpdateUI();
        UpdateTimeLabel();
    }
    
    private void UpdateTimeLabel()
    {
        double daysSincePlanted = (Time.GetUnixTimeFromSystem() - _plantedTime) / 86400.0;
        _timeLabel.Text = $"Age: {daysSincePlanted:F1} days";
    }
    
    public void Water()
    {
        if (!CanWater) return;
        
        _currentWater = _config.WaterNeed;
        _lastWateredTime = Time.GetUnixTimeFromSystem();
        GD.Print($"{_config.Name} has been watered!");
        UpdateUI();
    }
    
    public void Fertilize()
    {
        if (!CanFertilize) return;
        
        _isFertilized = true;
        _lastFertilizedTime = Time.GetUnixTimeFromSystem();
        GD.Print($"{_config.Name} has been fertilized!");
    }
    
    public void Harvest()
    {
        if (!CanHarvest) return;
        
        // Create harvested item and add to inventory
        var harvestedItem = ItemData.CreateHarvest(Type);
        if (_playerInventory.AddItem(harvestedItem))
        {
            GD.Print($"{_config.Name} harvested!");
            GrowthStage = 0;
            _growthProgress = 0;
            _currentWater = 0;
            _currentSunlight = 0;
            _isFertilized = false;  
            _plantedTime = Time.GetUnixTimeFromSystem();
            _lastWateredTime = -86400.0;
            _lastFertilizedTime = -86400.0;
            UpdateVisuals();
            UpdateUI();
        }
        else
        {
            GD.Print("Inventory is full!");
        }
    }
    
    public void SetInSunlight(bool inSunlight)
    {
        _isInSunlight = inSunlight;
    }
    
    private void UpdateUI()
    {
        if (_waterBar != null)
        {
            _waterBar.Value = _currentWater;
        }
        
        if (_sunlightBar != null)
        {
            _sunlightBar.Value = _currentSunlight;
        }
        
        if (_growthLabel != null)
        {
            _growthLabel.Text = $"Growth: {GrowthStage}/{MaxGrowthStage} ({_growthProgress:P0})";
        }
    }
    
    private void UpdateVisuals()
    {
        // Update the plant's appearance based on growth stage
        _sprite.Frame = GrowthStage;
    }
    
    public float GetCurrentWater() => _currentWater;
    public float GetCurrentSunlight() => _currentSunlight;
    public bool IsFertilized() => _isFertilized;
    public double GetLastWateredTime() => _lastWateredTime;
    public double GetLastFertilizedTime() => _lastFertilizedTime;
    public double GetPlantedTime() => _plantedTime;
    public float GetGrowthProgress() => _growthProgress;
    
    public void LoadSaveData(
        int growthStage,
        float currentWater,
        float currentSunlight,
        bool isFertilized,
        double lastWateredTime,
        double lastFertilizedTime,
        double plantedTime,
        float growthProgress)
    {
        GrowthStage = growthStage;
        _currentWater = currentWater;
        _currentSunlight = currentSunlight;
        _isFertilized = isFertilized;
        _lastWateredTime = lastWateredTime;
        _lastFertilizedTime = lastFertilizedTime;
        _plantedTime = plantedTime;
        _growthProgress = growthProgress;
        
        UpdateVisuals();
        UpdateUI();
    }
    
    [Signal]
    public delegate void PlantMoveRequestedEventHandler(Plant plant);
} 