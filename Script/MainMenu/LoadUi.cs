using Godot;
using System;

public partial class LoadUi : Control
{
	private Button backButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackButtonPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/title_screen.tscn");
	}
}
