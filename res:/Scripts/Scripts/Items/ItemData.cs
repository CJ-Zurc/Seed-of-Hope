using Godot;
using System;

public enum ItemType
{
    Tool,       // Shovel, Water Can, etc.
    Seed,       // Plant seeds
    Fertilizer, // Different types of fertilizer
    Harvest,    // Harvested plants
    Material    // Other materials
}

public class ItemData
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }
    public int MaxStackSize { get; set; }
    public bool IsStackable { get; set; }
    public float Durability { get; set; } // For tools
    public string IconPath { get; set; }
    
    public static ItemData CreateTool(string name, string description, float durability = 100f)
    {
        return new ItemData
        {
            Name = name,
            Description = description,
            Type = ItemType.Tool,
            MaxStackSize = 1,
            IsStackable = false,
            Durability = durability,
            IconPath = $"res://Assets/Items/{name.ToLower().Replace(" ", "_")}.png"
        };
    }
    
    public static ItemData CreateSeed(PlantType plantType)
    {
        var config = PlantConfig.GetConfig(plantType);
        return new ItemData
        {
            Name = $"{config.Name} Seeds",
            Description = $"Seeds for growing {config.Name}",
            Type = ItemType.Seed,
            MaxStackSize = 99,
            IsStackable = true,
            Durability = 100f,
            IconPath = $"res://Assets/Items/seed_{plantType.ToString().ToLower()}.png"
        };
    }
    
    public static ItemData CreateFertilizer(string name, string description)
    {
        return new ItemData
        {
            Name = name,
            Description = description,
            Type = ItemType.Fertilizer,
            MaxStackSize = 50,
            IsStackable = true,
            Durability = 100f,
            IconPath = $"res://Assets/Items/{name.ToLower().Replace(" ", "_")}.png"
        };
    }
    
    public static ItemData CreateHarvest(PlantType plantType)
    {
        var config = PlantConfig.GetConfig(plantType);
        return new ItemData
        {
            Name = config.Name,
            Description = $"Harvested {config.Name}",
            Type = ItemType.Harvest,
            MaxStackSize = 99,
            IsStackable = true,
            Durability = 100f,
            IconPath = $"res://Assets/Items/harvest_{plantType.ToString().ToLower()}.png"
        };
    }
} 