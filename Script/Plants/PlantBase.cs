using Godot;
using System;

public partial class PlantBase : Node2D
{
    [Export] public string PlantName { get; set; }
    [Export] public int GrowthDays { get; set; } = 7; // All plants take 7 days to grow
    [Export] public float WaterLevel { get; private set; } = 100f; // Start with full water
    [Export] public float MaxWaterLevel { get; set; } = 100f;
    [Export] public float WaterDecayRate { get; set; } = 20f; // 20% decrease every 4 hours

    private AnimatedSprite2D sprite;
    private int currentGrowthStage = 0;
    private int daysPlanted = 0;
    private bool isFullyGrown = false;
    private float lastWaterUpdateTime = 0f;
    private const float WATER_UPDATE_INTERVAL = 4f; // 4 hours in game time

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite != null)
        {
            sprite.Play("default");
            sprite.Frame = 0; // Start at first growth stage
        }
    }

    public override void _Process(double delta)
    {
        // Water level updates are handled by the game's time system
    }

    public void UpdateWaterLevel(float currentGameTime)
    {
        // Check if 4 hours have passed since last update
        if (currentGameTime - lastWaterUpdateTime >= WATER_UPDATE_INTERVAL)
        {
            WaterLevel = Mathf.Max(0, WaterLevel - WaterDecayRate);
            lastWaterUpdateTime = currentGameTime;
        }
    }

    public void WaterPlant()
    {
        if (WaterLevel <= 20f) // Only allow watering when water level is 20% or below
        {
            WaterLevel = MaxWaterLevel;
            lastWaterUpdateTime = GetNode<MainGame>("/root/MainGame").GetCurrentTime();
        }
    }

    public void AdvanceDay()
    {
        if (isFullyGrown) return;

        daysPlanted++;
        
        // Update growth stage based on days planted
        if (daysPlanted >= GrowthDays)
        {
            isFullyGrown = true;
            if (sprite != null)
            {
                sprite.Frame = sprite.SpriteFrames.GetFrameCount("default") - 1; // Set to last frame
            }
        }
        else if (sprite != null)
        {
            // Calculate which growth stage to show based on days planted
            float growthProgress = (float)daysPlanted / GrowthDays;
            int totalFrames = sprite.SpriteFrames.GetFrameCount("default");
            int targetFrame = Mathf.FloorToInt(growthProgress * (totalFrames - 1));
            sprite.Frame = targetFrame;
        }
    }

    public bool IsFullyGrown()
    {
        return isFullyGrown;
    }

    public bool NeedsWater()
    {
        return WaterLevel <= 20f; // Plant needs water when below 20%
    }

    public float GetWaterLevelPercentage()
    {
        return (WaterLevel / MaxWaterLevel) * 100f;
    }

    public int GetCurrentGrowthStage()
    {
        return sprite?.Frame ?? 0;
    }

    public int GetTotalGrowthStages()
    {
        return sprite?.SpriteFrames.GetFrameCount("default") ?? 0;
    }
} 