using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class GameSaveData
{
    public InventorySaveData Inventory { get; set; } = new();
    public List<PlantSaveData> Plants { get; set; } = new();
    public TimeManagerSaveData TimeManager { get; set; } = new();
    public float AkimCoins { get; set; } = 100.0f;
}

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

public class PlantSaveData
{
    public PlantType Type { get; set; }
    public int GrowthStage { get; set; }
    public float CurrentWater { get; set; }
    public float CurrentSunlight { get; set; }
    public bool IsFertilized { get; set; }
    public double LastWateredTime { get; set; }
    public double LastFertilizedTime { get; set; }
    public double PlantedTime { get; set; }
    public float GrowthProgress { get; set; }
    public Vector2 Position { get; set; }
}

public class TimeManagerSaveData
{
    public double CurrentMinute { get; set; }
    public int CurrentHour { get; set; }
    public int CurrentDay { get; set; }
    public Season CurrentSeason { get; set; }
    public int CurrentYear { get; set; }
}

public static class SaveManager
{
    private const string SAVE_FOLDER = "user://saves/";
    private const string SAVE_FILE = "game_save.json";
    
    public static void SaveGame()
    {
        var saveData = new GameSaveData();
        
        // Get singletons
        var inventory = GetNode<Inventory>("/root/PlayerInventory");
        var timeManager = GetNode<TimeManager>("/root/TimeManager");
        var currencyManager = GetNode<CurrencyManager>("/root/CurrencyManager");
        
        // Save currency
        saveData.AkimCoins = currencyManager.GetCurrentCoins();
        
        // Save inventory
        for (int i = 0; i < inventory.Size; i++)
        {
            var slot = inventory.GetSlot(i);
            if (!slot.IsEmpty)
            {
                saveData.Inventory.Slots.Add(new SlotSaveData
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
        
        // Save plants
        var plants = GetTree().GetNodesInGroup("plants");
        foreach (Plant plant in plants)
        {
            saveData.Plants.Add(new PlantSaveData
            {
                Type = plant.Type,
                GrowthStage = plant.GrowthStage,
                CurrentWater = plant.GetCurrentWater(),
                CurrentSunlight = plant.GetCurrentSunlight(),
                IsFertilized = plant.IsFertilized(),
                LastWateredTime = plant.GetLastWateredTime(),
                LastFertilizedTime = plant.GetLastFertilizedTime(),
                PlantedTime = plant.GetPlantedTime(),
                GrowthProgress = plant.GetGrowthProgress(),
                Position = plant.Position
            });
        }
        
        // Save time manager state
        saveData.TimeManager = new TimeManagerSaveData
        {
            CurrentMinute = timeManager.GetCurrentMinute(),
            CurrentHour = timeManager.GetCurrentHour(),
            CurrentDay = timeManager.GetCurrentDay(),
            CurrentSeason = timeManager.GetCurrentSeason(),
            CurrentYear = timeManager.GetCurrentYear()
        };
        
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
        GD.Print("Game saved successfully!");
    }
    
    public static void LoadGame()
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
            var saveData = JsonSerializer.Deserialize<GameSaveData>(jsonString);
            
            // Get singletons
            var inventory = GetNode<Inventory>("/root/PlayerInventory");
            var timeManager = GetNode<TimeManager>("/root/TimeManager");
            var currencyManager = GetNode<CurrencyManager>("/root/CurrencyManager");
            
            // Load currency
            currencyManager.LoadCoins(saveData.AkimCoins);
            
            // Load inventory
            inventory.Clear();
            foreach (var slotData in saveData.Inventory.Slots)
            {
                ItemData item;
                switch (slotData.ItemType)
                {
                    case ItemType.Tool:
                        item = ItemData.CreateTool(slotData.ItemName, slotData.ItemDescription, slotData.Durability);
                        break;
                    case ItemType.Seed:
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
            
            // Clear existing plants
            var existingPlants = GetTree().GetNodesInGroup("plants");
            foreach (Node plant in existingPlants)
            {
                plant.QueueFree();
            }
            
            // Load plants
            var plantScene = GD.Load<PackedScene>("res://Scenes/Plant.tscn");
            foreach (var plantData in saveData.Plants)
            {
                var plant = plantScene.Instantiate<Plant>();
                plant.Type = plantData.Type;
                plant.Position = plantData.Position;
                plant.LoadSaveData(
                    plantData.GrowthStage,
                    plantData.CurrentWater,
                    plantData.CurrentSunlight,
                    plantData.IsFertilized,
                    plantData.LastWateredTime,
                    plantData.LastFertilizedTime,
                    plantData.PlantedTime,
                    plantData.GrowthProgress
                );
                GetTree().CurrentScene.AddChild(plant);
            }
            
            // Load time manager state
            timeManager.LoadSaveData(
                saveData.TimeManager.CurrentMinute,
                saveData.TimeManager.CurrentHour,
                saveData.TimeManager.CurrentDay,
                saveData.TimeManager.CurrentSeason,
                saveData.TimeManager.CurrentYear
            );
            
            GD.Print("Game loaded successfully!");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading game: {e.Message}");
        }
    }
} 