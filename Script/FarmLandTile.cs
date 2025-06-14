using Godot;
using System;

public partial class FarmLandTile : Area2D
{
    [Export] public NodePath dialogueBoxPath;
    [Export] public PackedScene ampalayaScene;
    [Export] public PackedScene calamansiScene;
    [Export] public PackedScene pechayScene;
    [Export] public PackedScene succulentScene;
    [Export] public PackedScene sunflowerScene;
    [Export] public PackedScene plantInfoScene;

    private DialogueBox dialogueBox;
    private bool isPlayerInRange = false;
    private bool isPlanted = false;
    private string currentPlant = "";
    private PlantBase currentPlantInstance;
    private Control plantInfoInstance;
    private bool isAwaitingInteraction = false;

    // Plant options
    private readonly string[] availablePlants = new string[] {
        "Ampalaya",
        "Calamansi",
        "Pechay",
        "Succulent",
        "Sunflower"
    };

    public override void _Ready()
    {
        // Add this node to the FarmTiles group
        AddToGroup("FarmTiles");
        
        dialogueBox = GetNode<DialogueBox>(dialogueBoxPath);
        
        // Connect signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
        InputEvent += OnInputEvent;
    }

    public override void _Input(InputEvent @event)
    {
        if (isPlayerInRange && @event.IsActionPressed("interact"))
        {
            ShowInteractionDialogue();
        }
    }

    private void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (isPlayerInRange && @event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            ShowInteractionDialogue();
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            isPlayerInRange = false;
            if (plantInfoInstance != null)
            {
                plantInfoInstance.QueueFree();
                plantInfoInstance = null;
            }
        }
    }

    private void ShowPlantingOptions()
    {
        dialogueBox.ShowDialogue("What would you like to do?", new string[] { "Plant Seed", "Cancel" }, OnPlantingOptionSelected);
    }

    private void OnPlantingOptionSelected(int optionIndex)
    {
        if (optionIndex == 0) // Plant Seed
        {
            ShowPlantSelection();
        }
        // If optionIndex is 1 (Cancel), dialogue box will close automatically
    }

    private void ShowPlantSelection()
    {
        dialogueBox.ShowDialogue("Choose a plant to grow:", availablePlants, OnPlantSelected);
    }

    private void OnPlantSelected(int plantIndex)
    {
        if (plantIndex >= 0 && plantIndex < availablePlants.Length)
        {
            currentPlant = availablePlants[plantIndex];
            PlantSeed();
        }
    }

    private void PlantSeed()
    {
        if (isPlanted) return;

        PackedScene plantScene = GetPlantScene(currentPlant);
        if (plantScene != null)
        {
            currentPlantInstance = plantScene.Instantiate<PlantBase>();
            currentPlantInstance.PlantName = currentPlant;
            AddChild(currentPlantInstance);
            isPlanted = true;
            GD.Print($"Planted {currentPlant}");
        }
    }

    private void ShowPlantInteractionOptions()
    {
        if (currentPlantInstance == null) return;

        string[] options = new string[] { "Water", "Harvest" };
        if (!currentPlantInstance.NeedsWater())
        {
            options = new string[] { "Harvest" }; // Only show harvest if plant doesn't need water
        }

        dialogueBox.ShowDialogue($"{currentPlantInstance.PlantName}\nWater Level: {currentPlantInstance.GetWaterLevelPercentage():F0}%", 
            options, OnPlantInteractionSelected);
    }

    private void OnPlantInteractionSelected(int optionIndex)
    {
        if (currentPlantInstance == null) return;

        if (optionIndex == 0) // Water
        {
            if (currentPlantInstance.NeedsWater())
            {
                currentPlantInstance.WaterPlant();
                ShowPlantInteractionOptions(); // Refresh the dialogue
            }
        }
        else if (optionIndex == 1 || (optionIndex == 0 && !currentPlantInstance.NeedsWater())) // Harvest
        {
            if (currentPlantInstance.IsFullyGrown())
            {
                HarvestPlant();
            }
            else
            {
                dialogueBox.ShowDialogue("This plant is not ready to harvest yet.", new string[] { "OK" }, null);
            }
        }
    }

    private void HarvestPlant()
    {
        // TODO: Add harvesting logic (give items to player, etc.)
        currentPlantInstance.QueueFree();
        currentPlantInstance = null;
        isPlanted = false;
        currentPlant = "";
    }

    private PackedScene GetPlantScene(string plantName)
    {
        return plantName switch
        {
            "Ampalaya" => ampalayaScene,
            "Calamansi" => calamansiScene,
            "Pechay" => pechayScene,
            "Succulent" => succulentScene,
            "Sunflower" => sunflowerScene,
            _ => null
        };
    }

    public void UpdatePlant(float currentGameTime)
    {
        if (currentPlantInstance != null)
        {
            currentPlantInstance.UpdateWaterLevel(currentGameTime);
        }
    }

    public void AdvanceDay()
    {
        if (currentPlantInstance != null)
        {
            currentPlantInstance.AdvanceDay();
        }
    }

    public bool IsPlanted()
    {
        return isPlanted;
    }

    public bool NeedsWater()
    {
        return currentPlantInstance?.NeedsWater() ?? false;
    }

    private void ShowInteractionDialogue()
    {
        if (!isPlanted)
        {
            ShowPlantingOptions();
        }
        else
        {
            ShowPlantInteractionOptions();
        }
    }
} 
