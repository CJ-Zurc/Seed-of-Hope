using Godot;
using System;

public partial class TitleScreen : Control
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

	// Disable New Game button if save file exists
	newGame.Disabled = FileAccess.FileExists("user://savegame.json");

	// Disable Load Game button if save file does not exist
	loadGame.Disabled = !FileAccess.FileExists("user://savegame.json");

	loadGame.Pressed += OnLoadGamePressed;
	settingsGame.Pressed += OnSettingsGamePressed;
	exitGame.Pressed += OnExitGamePressed;
	newGame.Pressed += OnNewGamePressed;
	}

	private void OnLoadGamePressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main_game.tscn");
	}

	private void OnSettingsGamePressed()
	{
		var settingsScene = GD.Load<PackedScene>("res://scenes/settings.tscn");
		var settingsInstance = settingsScene.Instantiate();
		AddChild(settingsInstance);
	}
	
	private void OnExitGamePressed()
	{
		GetTree().Quit();
	}


	private void OnNewGamePressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/intro_cutscene.tscn");
	}	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _Process(double delta)
	{
	}
}
