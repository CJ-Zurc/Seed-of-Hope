using Godot;
using System;

public partial class ComputerArea : Area2D
{
    private ComputerInteraction _computer;
    
    public override void _Ready()
    {
        _computer = GetParent<ComputerInteraction>();
        
        // Connect signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            player.SetNearbyComputer(_computer);
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        if (body is Player player)
        {
            player.ClearNearbyComputer();
            _computer.HideComputer();
        }
    }
} 