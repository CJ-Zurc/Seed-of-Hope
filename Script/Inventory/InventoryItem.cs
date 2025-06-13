using Godot;
using System;

public class InventoryItem
{
    public string Type { get; private set; }
    public int Count { get; set; } = 1;
    public int MaxStack { get; private set; } = 99;
    public string Description { get; private set; }
    public int Value { get; private set; }
    
    public InventoryItem(string type, string description = "", int value = 0)
    {
        Type = type;
        Description = description;
        Value = value;
    }
    
    public static InventoryItem CreateFromPlant(PlantType plantType)
    {
        return plantType switch
        {
            PlantType.Ampalaya => new InventoryItem(
                "Ampalaya",
                "A bitter gourd vegetable, rich in nutrients.",
                20),
            
            PlantType.Calamansi => new InventoryItem(
                "Calamansi",
                "A citrus fruit commonly used for cooking and drinks.",
                15),
            
            PlantType.Pechay => new InventoryItem(
                "Pechay",
                "A leafy vegetable popular in Asian cuisine.",
                10),
            
            PlantType.Succulent => new InventoryItem(
                "Succulent",
                "A decorative plant that stores water in its leaves.",
                25),
            
            PlantType.Sunflower => new InventoryItem(
                "Sunflower",
                "A beautiful flower that follows the sun.",
                30),
            
            _ => new InventoryItem("Unknown Plant", "An unknown plant type.", 0)
        };
    }
} 