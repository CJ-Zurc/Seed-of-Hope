using Godot;
using System;

public partial class PlantManager : Node
{
    private Plant _selectedPlant;
    private bool _isMovingPlant;
    
    public override void _Ready()
    {
        // Find all plants in the scene and connect their signals
        foreach (Plant plant in GetTree().GetNodesInGroup("plants"))
        {
            ConnectPlantSignals(plant);
        }
    }
    
    private void ConnectPlantSignals(Plant plant)
    {
        plant.PlantMoveRequested += OnPlantMoveRequested;
    }
    
    private void OnPlantMoveRequested(Plant plant)
    {
        _selectedPlant = plant;
        _isMovingPlant = true;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        
        // Create a preview sprite
        Sprite2D preview = new Sprite2D();
        preview.Texture = plant.GetNode<Sprite2D>("Sprite2D").Texture;
        preview.GlobalPosition = GetViewport().GetMousePosition();
        preview.Modulate = new Color(1, 1, 1, 0.5f);
        preview.Name = "PlantPreview";
        AddChild(preview);
    }
    
    public override void _Input(InputEvent @event)
    {
        if (!_isMovingPlant) return;
        
        if (@event is InputEventMouseMotion motion)
        {
            // Update preview position
            if (HasNode("PlantPreview"))
            {
                GetNode<Sprite2D>("PlantPreview").GlobalPosition = motion.GlobalPosition;
            }
        }
        else if (@event is InputEventMouseButton mouseButton && 
                mouseButton.ButtonIndex == MouseButton.Left && 
                mouseButton.Pressed)
        {
            // Place the plant at the new location
            _selectedPlant.GlobalPosition = GetViewport().GetMousePosition();
            FinishMoving();
        }
        else if (@event is InputEventMouseButton mouseButton2 && 
                mouseButton2.ButtonIndex == MouseButton.Right && 
                mouseButton2.Pressed)
        {
            // Cancel moving
            FinishMoving();
        }
    }
    
    private void FinishMoving()
    {
        _isMovingPlant = false;
        _selectedPlant = null;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        
        if (HasNode("PlantPreview"))
        {
            GetNode("PlantPreview").QueueFree();
        }
    }
} 