using Godot;
using System;

public enum PlantType
{
    Sunflower,
    Bittergourd,
    Succulent,
    Petchay,
    Calamansi
}

public class PlantConfig
{
    public string Name { get; set; }
    public float WaterNeed { get; set; } // How much water it needs daily (0-100)
    public float WaterDepletionRate { get; set; } // How fast water depletes
    public float SunlightNeed { get; set; } // How much sunlight it needs daily (0-100)
    public float GrowthRate { get; set; } // Base growth rate multiplier
    public int DaysToMature { get; set; } // Days needed to reach full growth
    public bool IsMoveable { get; set; } // Whether the plant can be moved after planting
    public float FertilizerEfficiency { get; set; } // How well it responds to fertilizer (multiplier)
    
    public static PlantConfig GetConfig(PlantType type)
    {
        switch (type)
        {
            case PlantType.Sunflower:
                return new PlantConfig
                {
                    Name = "Sunflower",
                    WaterNeed = 70f,
                    WaterDepletionRate = 0.15f,
                    SunlightNeed = 90f,
                    GrowthRate = 1.2f,
                    DaysToMature = 4,
                    IsMoveable = false, // Sunflowers have deep roots
                    FertilizerEfficiency = 1.3f
                };
                
            case PlantType.Bittergourd:
                return new PlantConfig
                {
                    Name = "Bitter Gourd",
                    WaterNeed = 60f,
                    WaterDepletionRate = 0.12f,
                    SunlightNeed = 75f,
                    GrowthRate = 0.9f,
                    DaysToMature = 5,
                    IsMoveable = true,
                    FertilizerEfficiency = 1.5f
                };
                
            case PlantType.Succulent:
                return new PlantConfig
                {
                    Name = "Succulent",
                    WaterNeed = 30f,
                    WaterDepletionRate = 0.05f, // Very slow water depletion
                    SunlightNeed = 50f,
                    GrowthRate = 0.5f, // Slow growing
                    DaysToMature = 7,
                    IsMoveable = true,
                    FertilizerEfficiency = 0.8f // Less dependent on fertilizer
                };
                
            case PlantType.Petchay:
                return new PlantConfig
                {
                    Name = "Petchay",
                    WaterNeed = 80f,
                    WaterDepletionRate = 0.2f, // Needs frequent watering
                    SunlightNeed = 70f,
                    GrowthRate = 1.5f, // Fast growing
                    DaysToMature = 3,
                    IsMoveable = true,
                    FertilizerEfficiency = 1.6f
                };
                
            case PlantType.Calamansi:
                return new PlantConfig
                {
                    Name = "Calamansi",
                    WaterNeed = 65f,
                    WaterDepletionRate = 0.1f,
                    SunlightNeed = 85f,
                    GrowthRate = 0.7f, // Slow growing tree
                    DaysToMature = 6,
                    IsMoveable = false, // Tree, can't be moved once planted
                    FertilizerEfficiency = 1.2f
                };
                
            default:
                throw new ArgumentException("Unknown plant type");
        }
    }
} 