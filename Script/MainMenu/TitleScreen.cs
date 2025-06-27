using Godot;
using System;

public partial class TitleScreen : Control
{
	private Button newGame;
	private Button loadGame;
	private Button settingsGame;
	private Button exitGame;
	private const int MaxSaveFiles = 3;
	private string[] saveFileNames = new string[]
	{
		"user://savegame1.json",
		"user://savegame2.json",
		"user://savegame3.json"
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		newGame = GetNode<Button>("New");
		loadGame = GetNode<Button>("Load");
		settingsGame = GetNode<Button>("Options");
		exitGame = GetNode<Button>("ExitGame");

		int saveCount = 0;
		foreach (var file in saveFileNames)
		{
			if (FileAccess.FileExists(file))
				saveCount++;
		}
		// Disable New Game button if 3 save files exist
		newGame.Disabled = saveCount >= MaxSaveFiles;
		// Disable Load Game button if no save file exists
		loadGame.Disabled = saveCount == 0;

		loadGame.Pressed += OnLoadGamePressed;
		settingsGame.Pressed += OnSettingsGamePressed;
		exitGame.Pressed += OnExitGamePressed;
		newGame.Pressed += OnNewGamePressed;
	}

	private void OnLoadGamePressed()
	{
		// Open the Load UI scene instead of loading the game directly
		var loadUiScene = GD.Load<PackedScene>("res://scenes/LoadUI.tscn");
		var loadUiInstance = loadUiScene.Instantiate();
		AddChild(loadUiInstance);
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
		// Find the first available save slot
		int slot = -1;
		for (int i = 0; i < MaxSaveFiles; i++)
		{
			if (!FileAccess.FileExists(saveFileNames[i]))
			{
				slot = i + 1;
				break;
			}
		}
		if (slot != -1)
		{
			MainGame.CurrentSaveSlot = slot; // Set the slot for the new game
			GD.Print($"Starting new game in slot {slot}");
			GetTree().ChangeSceneToFile("res://scenes/intro_cutscene.tscn");
		}
	}	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _Process(double delta)
	{
	}
}
