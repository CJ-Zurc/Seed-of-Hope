using Godot;
using System;

public partial class FarmlandTile : Area2D
{
    [Export]
    public NodePath PlayerPath { get; set; }
    
    private Player _player;
    private Inventory _inventory;
    private PlantingSystem _plantingSystem;
    private bool _isPlayerInRange = false;
    private Control _dialogueBox;
    private bool _isDialogueOpen = false;
    
    public override void _Ready()
    {
        _player = GetNode<Player>(PlayerPath);
        _inventory = GetNode<Inventory>("/root/PlayerInventory");
        _plantingSystem = GetNode<PlantingSystem>("/root/MainGame/PlantingSystem");
        
        // Connect the area entered/exited signals
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        
        // Load the dialogue scene
        var dialogueScene = GD.Load<PackedScene>("res://scenes/PlantingDialogue.tscn");
        _dialogueBox = dialogueScene.Instantiate<Control>();
        
        // Connect dialogue buttons when created
        var plantButton = _dialogueBox.GetNode<Button>("CanvasLayer/Control/plantButton");
        var cancelButton = _dialogueBox.GetNode<Button>("CanvasLayer/Control/cancelButton");
        
        plantButton.Pressed += OnPlantPressed;
        cancelButton.Pressed += OnCancelPressed;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (!_isPlayerInRange || _isDialogueOpen) return;
        
        if (@event.IsActionPressed("interact"))
        {
            ShowPlantingDialogue();
        }
    }
    
    private void ShowPlantingDialogue()
    {
        _isDialogueOpen = true;
        _player.SetProcess(false); // Disable player movement
        
        // Update dialogue text based on available seeds
        var messageLabel = _dialogueBox.GetNode<Label>("CanvasLayer/Control/message");
        var plantButton = _dialogueBox.GetNode<Button>("CanvasLayer/Control/plantButton");
        
        if (HasAvailableSeeds())
        {
            messageLabel.Text = "Choose a seed to plant:";
            plantButton.Disabled = false;
        }
        else
        {
            messageLabel.Text = "There are no plants to seed, buy in the computer";
            plantButton.Disabled = true;
        }
        
        GetTree().Root.AddChild(_dialogueBox);
    }
    
    private bool HasAvailableSeeds()
    {
        // Check if player has any plantable seeds in inventory
        foreach (var item in _inventory.GetItems())
        {
            if (item.Type.ToString().EndsWith("Seed"))
            {
                return true;
            }
        }
        return false;
    }
    
    private void OnPlantPressed()
    {
        // Show seed selection menu if player has seeds
        var seedMenu = new PopupMenu();
        seedMenu.Name = "SeedMenu";
        
        // Add available seeds to menu
        int index = 0;
        foreach (var item in _inventory.GetItems())
        {
            if (item.Type.ToString().EndsWith("Seed"))
            {
                seedMenu.AddItem(item.Type.ToString(), index);
                index++;
            }
        }
        
        seedMenu.IndexPressed += (idx) =>
        {
            var selectedSeed = seedMenu.GetItemText((int)idx);
            PlantSeed(selectedSeed);
            seedMenu.QueueFree();
            CloseDialogue();
        };
        
        _dialogueBox.AddChild(seedMenu);
        seedMenu.Position = new Vector2(100, 100); // Adjust position as needed
        seedMenu.Popup();
    }
    
    private void PlantSeed(string seedType)
    {
        // Convert seed type to plant type (remove "Seed" suffix)
        var plantType = (PlantType)Enum.Parse(typeof(PlantType), seedType.Replace("Seed", ""));
        
        // Plant at the farmland tile's position
        if (_plantingSystem.PlantSeed(plantType, GlobalPosition))
        {
            // Remove seed from inventory
            _inventory.RemoveItem(seedType);
        }
    }
    
    private void OnCancelPressed()
    {
        CloseDialogue();
    }
    
    private void CloseDialogue()
    {
        _isDialogueOpen = false;
        _player.SetProcess(true); // Re-enable player movement
        _dialogueBox.QueueFree();
    }
    
    private void OnAreaEntered(Area2D area)
    {
        if (area.IsInGroup("player"))
        {
            _isPlayerInRange = true;
        }
    }
    
    private void OnAreaExited(Area2D area)
    {
        if (area.IsInGroup("player"))
        {
            _isPlayerInRange = false;
        }
    }
} 