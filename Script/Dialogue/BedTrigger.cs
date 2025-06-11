using Godot;
using System;

public partial class BedTrigger : Area2D
{
    [Export] public NodePath playerPath;
    private CharacterBody2D player;
    public override void _Ready()
    {
        player = GetNode<CharacterBody2D>(playerPath);
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body == player) // Ensure correct player node
        {
            var dialogue = GD.Load<PackedScene>("res://scenes/BedDialogue.tscn").Instantiate<Control>();
            GetTree().Root.AddChild(dialogue);
        }
    }
}
