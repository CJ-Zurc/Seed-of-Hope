using Godot;
using System;

public partial class TV : Node2D
{
    private Area2D _interactionArea;
    
    public override void _Ready()
    {
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.AddToGroup("interactable");
    }
    
    public void Use()
    {
        // Here you can implement the TV functionality
        // For example, showing a weather report or news scene
        GD.Print("TV turned on!");
        // You could emit a signal here to change scenes or show TV content
    }
} 