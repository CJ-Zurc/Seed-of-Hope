using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class InventorySaveData
{
    public List<SlotSaveData> Slots { get; set; } = new();
}

public class SlotSaveData
{
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public ItemType ItemType { get; set; }
    public int Quantity { get; set; }
    public float Durability { get; set; }
    public int SlotIndex { get; set; }
}

public static class SaveManager
{
    private const string SAVE_FOLDER = "user://saves/";
    private const string SAVE_FILE = "inventory.json";
    
    public static void SaveInventory(Inventory inventory)
    {
        var saveData = new InventorySaveData();
        
        // Convert inventory slots to save data
        for (int i = 0; i < inventory.Size; i++)
        {
            var slot = inventory.GetSlot(i);
            if (!slot.IsEmpty)
            {
                saveData.Slots.Add(new SlotSaveData
                {
                    ItemName = slot.Item.Name,
                    ItemDescription = slot.Item.Description,
                    ItemType = slot.Item.Type,
                    Quantity = slot.Quantity,
                    Durability = slot.Item.Durability,
                    SlotIndex = slot.SlotIndex
                });
            }
        }
        
        // Create save directory if it doesn't exist
        var dir = DirAccess.Open("user://");
        if (!dir.DirExists("saves"))
        {
            dir.MakeDir("saves");
        }
        
        // Serialize and save
        string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        using var file = FileAccess.Open(SAVE_FOLDER + SAVE_FILE, FileAccess.ModeFlags.Write);
        file.StoreString(jsonString);
        GD.Print("Inventory saved successfully!");
    }
    
    public static void LoadInventory(Inventory inventory)
    {
        if (!FileAccess.FileExists(SAVE_FOLDER + SAVE_FILE))
        {
            GD.Print("No save file found.");
            return;
        }
        
        try
        {
            using var file = FileAccess.Open(SAVE_FOLDER + SAVE_FILE, FileAccess.ModeFlags.Read);
            string jsonString = file.GetAsText();
            var saveData = JsonSerializer.Deserialize<InventorySaveData>(jsonString);
            
            // Clear current inventory
            inventory.Clear();
            
            // Load saved items
            foreach (var slotData in saveData.Slots)
            {
                ItemData item;
                switch (slotData.ItemType)
                {
                    case ItemType.Tool:
                        item = ItemData.CreateTool(slotData.ItemName, slotData.ItemDescription, slotData.Durability);
                        break;
                    case ItemType.Seed:
                        // Extract plant type from item name
                        string plantName = slotData.ItemName.Replace(" Seeds", "");
                        PlantType plantType = Enum.Parse<PlantType>(plantName);
                        item = ItemData.CreateSeed(plantType);
                        break;
                    case ItemType.Fertilizer:
                        item = ItemData.CreateFertilizer(slotData.ItemName, slotData.ItemDescription);
                        break;
                    case ItemType.Harvest:
                        plantType = Enum.Parse<PlantType>(slotData.ItemName);
                        item = ItemData.CreateHarvest(plantType);
                        break;
                    default:
                        continue;
                }
                
                inventory.AddItemToSlot(item, slotData.SlotIndex, slotData.Quantity);
            }
            
            GD.Print("Inventory loaded successfully!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading inventory: {e.Message}");
        }
    }
} 