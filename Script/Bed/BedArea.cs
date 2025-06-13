using Godot;
using System;

public partial class BedArea : Area2D
{
    private BedInteraction _bed;
    private Player _nearbyPlayer;
    
    public override void _Ready()
    {
        _bed = GetParent<BedInteraction>();
        
        // Connect signals
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }
    
    public override void _Input(InputEvent @event)
    {
        if (_nearbyPlayer != null && @event.IsActionPressed("interact"))
        {
            _bed.TryToSleep();
        }
    }
    
    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            _nearbyPlayer = player;
        }
    }
    
    private void OnBodyExited(Node2D body)
    {
        if (body is Player)
        {
            _nearbyPlayer = null;
        }
    }
} 