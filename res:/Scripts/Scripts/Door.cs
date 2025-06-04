using Godot;
using System;

public partial class Door : Node2D
{
    [Export]
    public string TargetScene { get; set; } = "";
    
    [Export]
    public Vector2 SpawnPosition { get; set; } = Vector2.Zero;
    
    private Area2D _interactionArea;
    
    [Signal]
    public delegate void DoorEnteredEventHandler(string targetScene, Vector2 spawnPosition);
    
    public override void _Ready()
    {
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.AddToGroup("interactable");
    }
    
    public void Enter()
    {
        if (!string.IsNullOrEmpty(TargetScene))
        {
            EmitSignal(SignalName.DoorEntered, TargetScene, SpawnPosition);
        }
        else
        {
            GD.PrintErr("No target scene set for this door!");
        }
    }
} 