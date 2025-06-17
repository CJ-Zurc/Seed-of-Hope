using Godot;
using System;

public partial class TitleScreen : Node2D
{
	private Button newGame;
	private Button loadGame;
	private Button settingsGame;
	private Button exitGame;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		newGame = GetNode<Button>("New");
		loadGame = GetNode<Button>("Load");
		settingsGame = GetNode<Button>("Options");
		exitGame = GetNode<Button>("ExitGame");

		loadGame.Pressed += OnLoadGamePressed;
		settingsGame.Pressed += OnSettingsGamePressed;
		exitGame.Pressed += OnExitGamePressed;
	}

	private void OnLoadGamePressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/LoadUI.tscn");
	}

	private void OnSettingsGamePressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/settings.tscn");
	}
	
	private void OnExitGamePressed()
	{
		GetTree().Quit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _Process(double delta)
	{
	}
}
