using Godot;
using System;

public partial class PlayerStamina : Node
{
    private TextureProgressBar _staminaBar;
    private const float MAX_STAMINA = 100f;
    private const float STAMINA_COST_WATER = 5f;
    private const float STAMINA_COST_PLANT = 5f;
    private const float STAMINA_COST_HARVEST = 5f;
    
    private float _currentStamina = MAX_STAMINA;
    
    public override void _Ready()
    {
        _staminaBar = GetNode<TextureProgressBar>("StaminaBar");
        InitializeStaminaBar();
    }
    
    private void InitializeStaminaBar()
    {
        if (_staminaBar != null)
        {
            _staminaBar.MinValue = 0;
            _staminaBar.MaxValue = MAX_STAMINA;
            _staminaBar.Value = _currentStamina;
        }
    }
    
    public bool HasEnoughStamina(float cost)
    {
        return _currentStamina >= cost;
    }
    
    public bool UseStamina(float amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;
            UpdateStaminaBar();
            return true;
        }
        GD.Print("Not enough stamina!");
        return false;
    }
    
    public bool CanWaterPlant()
    {
        return HasEnoughStamina(STAMINA_COST_WATER);
    }
    
    public bool CanPlantSeed()
    {
        return HasEnoughStamina(STAMINA_COST_PLANT);
    }
    
    public bool CanHarvest()
    {
        return HasEnoughStamina(STAMINA_COST_HARVEST);
    }
    
    public void ConsumeWaterStamina()
    {
        UseStamina(STAMINA_COST_WATER);
    }
    
    public void ConsumePlantStamina()
    {
        UseStamina(STAMINA_COST_PLANT);
    }
    
    public void ConsumeHarvestStamina()
    {
        UseStamina(STAMINA_COST_HARVEST);
    }
    
    private void UpdateStaminaBar()
    {
        if (_staminaBar != null)
        {
            _staminaBar.Value = _currentStamina;
        }
    }
    
    // Call this when the day starts or when resting
    public void RestoreStamina()
    {
        _currentStamina = MAX_STAMINA;
        UpdateStaminaBar();
    }
} 
