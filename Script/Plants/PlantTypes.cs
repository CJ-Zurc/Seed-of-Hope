using Godot;
using System;

public enum PlantType
{
    Sunflower,
    Ampalaya,
    Succulent,
    Pechay,
    Calamansi
}

public class PlantConfig
{
    public string Name { get; set; }
    public string Description { get; set; }
    public float WaterNeed { get; set; } // How much water it needs daily (0-100)
    public float WaterDepletionRate { get; set; } // How fast water depletes
    public float GrowthRate { get; set; } // Base growth rate multiplier
    public int DaysToMature { get; set; } // Days needed to reach full growth
    public bool IsMoveable { get; set; } // Whether the plant can be moved after planting
    
    public static PlantConfig GetConfig(PlantType type)
    {
        switch (type)
        {
            case PlantType.Sunflower:
                return new PlantConfig
                {
                    Name = "Sunflower",
                    Description = "A tall, cheerful flower that requires regular watering.",
                    WaterNeed = 70f,
                    WaterDepletionRate = 0.15f,
                    GrowthRate = 1.0f, // Adjusted for 4-day growth cycle
                    DaysToMature = 4,  // Can be harvested on day 4-5
                    IsMoveable = false // Sunflowers have deep roots
                };
                
            case PlantType.Ampalaya:
                return new PlantConfig
                {
                    Name = "Ampalaya",
                    Description = "Also known as bitter gourd, this climbing vine produces nutritious but bitter fruits.",
                    WaterNeed = 60f,
                    WaterDepletionRate = 0.12f,
                    GrowthRate = 1.0f, // Adjusted for 5-day growth cycle
                    DaysToMature = 5,  // Takes 5 days to mature
                    IsMoveable = true
                };
                
            case PlantType.Succulent:
                return new PlantConfig
                {
                    Name = "Succulent",
                    Description = "A drought-resistant plant that stores water in its leaves. Perfect for beginners.",
                    WaterNeed = 30f,
                    WaterDepletionRate = 0.05f, // Very slow water depletion
                    GrowthRate = 1.0f, // Adjusted for 4-day growth cycle
                    DaysToMature = 4,  // Can be harvested on day 4-5
                    IsMoveable = true
                };
                
            case PlantType.Pechay:
                return new PlantConfig
                {
                    Name = "Pechay",
                    Description = "A leafy vegetable that grows quickly. Needs frequent watering for best results.",
                    WaterNeed = 80f,
                    WaterDepletionRate = 0.2f, // Needs frequent watering
                    GrowthRate = 1.0f, // Adjusted for 4-day growth cycle
                    DaysToMature = 4,  // Can be harvested on day 4-5
                    IsMoveable = true
                };
                
            case PlantType.Calamansi:
                return new PlantConfig
                {
                    Name = "Calamansi",
                    Description = "A citrus tree that produces small, lime-like fruits. Requires consistent care.",
                    WaterNeed = 65f,
                    WaterDepletionRate = 0.1f,
                    GrowthRate = 1.0f, // Adjusted for daily growth stages
                    DaysToMature = 7,  // Takes 7 days to mature
                    IsMoveable = false // Tree, can't be moved once planted
                };
                
            default:
                throw new ArgumentException("Unknown plant type");
        }
    }
} 