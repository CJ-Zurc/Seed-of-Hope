using Godot;
using System;

public partial class Settings : Control
{
    [Export] public NodePath BackButtonPath;

    private Button backButton;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        backButton = GetNode<Button>(BackButtonPath);
        backButton.Pressed += OnBackButtonPressed;

    }


    private void OnBackButtonPressed()
    {
        QueueFree();
    }
}
