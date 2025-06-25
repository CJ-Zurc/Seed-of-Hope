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

	// Check for available save slots
	bool[] slotExists = new bool[3];
	int usedSlots = 0;
	for (int i = 0; i < 3; i++)
	{
		string path = $"user://savegame{i+1}.json";
		slotExists[i] = FileAccess.FileExists(path);
		if (slotExists[i]) usedSlots++;
	}
	// Disable New Game if all slots are used
	newGame.Disabled = usedSlots >= 3;
	// Always allow loading (to go to load UI)
	loadGame.Disabled = usedSlots == 0;

	loadGame.Pressed += OnLoadGamePressed;
	settingsGame.Pressed += OnSettingsGamePressed;
	exitGame.Pressed += OnExitGamePressed;
	newGame.Pressed += OnNewGamePressed;
	}

	private void OnLoadGamePressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/LoadUI.tscn");
	}

	private void OnSettingsGamePressed()
	{
		var settingsScene = GD.Load<PackedScene>("res://scenes/settings.tscn");
		var settingsInstance = settingsScene.Instantiate();
		if (settingsInstance.HasMethod("SetOpenedFromGame"))
		{
			settingsInstance.Call("SetOpenedFromGame", false);
		}
		AddChild(settingsInstance);
	}
	
	private void OnExitGamePressed()
	{
		GetTree().Quit();
	}


	private void OnNewGamePressed()
	{
		// Find the first available slot (slot 1, 2, or 3)
		int slot = -1;
		for (int i = 0; i < 3; i++)
		{
			string path = $"user://savegame{i+1}.json";
			if (!FileAccess.FileExists(path))
			{
				slot = i;
				break;
			}
		}
		if (slot == -1)
		{
			GD.Print("No available save slots!");
			return;
		}
		GameState.Instance.SelectedSaveSlot = slot;
		// Create a blank save file for this slot with 200 currency
		string savePath = $"user://savegame{slot+1}.json";
		var saveData = new Godot.Collections.Dictionary
		{
			["player_position"] = new Godot.Collections.Array { -16, -459 },
			["day"] = 1,
			["year"] = 1,
			["time"] = 6f,
			["volume"] = 0f,
			["currency"] = 200
		};
		string json = Json.Stringify(saveData);
		using (var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write))
		{
			file.StoreString(json);
		}
		// Start the game (intro cutscene or main game)
		GetTree().ChangeSceneToFile("res://scenes/intro_cutscene.tscn");
	}	// Called every frame. 'delta' is the elapsed time since the previous frame.


	public override void _Process(double delta)
	{
	}
}
