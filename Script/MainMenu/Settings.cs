using Godot;
using System;

public partial class Settings : Control
{
	[Export] public NodePath BackButtonPath;
	[Export] public NodePath VolumeSliderPath;

	private Button backButton;
	private HSlider volumeSlider;

	// Name of your custom bus (must match the name in the Audio panel)
	private const string MusicBusName = "Master";


	// Signal for when the volume changes, to notify other scripts (e.g., MainGame)
	[Signal]
	public delegate void VolumeChangedEventHandler(float newVolume);


	private bool OpenedFromGame = false;
	private Button mainMenuButton;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Return button
		backButton = GetNode<Button>(BackButtonPath);
		backButton.Pressed += OnBackButtonPressed;
		//Main menu button
		mainMenuButton = GetNode<Button>("../settings/Main Menu");
		mainMenuButton.Pressed += GoToTitleScreen;

		volumeSlider = GetNode<HSlider>(VolumeSliderPath);
		volumeSlider.ValueChanged += OnVolumeChanged;


		// Load saved volume if available, otherwise use current bus volume
		float initialVolume = 0f;
		if (FileAccess.FileExists("user://savegame.json"))
		{
			using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			var saveData = result.As<Godot.Collections.Dictionary>();
			if (saveData != null && saveData.ContainsKey("volume"))
				initialVolume = (float)saveData["volume"];
			else
			{
				int busIdx = AudioServer.GetBusIndex(MusicBusName);
				initialVolume = AudioServer.GetBusVolumeDb(busIdx);
			}
		}
		else
		{
			int busIdx = AudioServer.GetBusIndex(MusicBusName);
			initialVolume = AudioServer.GetBusVolumeDb(busIdx);
		}
		volumeSlider.Value = initialVolume;

	}

	public void SetOpenedFromGame(bool fromGame)
	{
		OpenedFromGame = fromGame;
	}

	private void OnVolumeChanged(double value)
	{
		int busIdx = AudioServer.GetBusIndex(MusicBusName);
		if (busIdx >= 0)
			AudioServer.SetBusVolumeDb(busIdx, (float)value);
		else
			GD.PrintErr($"Audio bus '{MusicBusName}' not found!");


		// Save volume to savegame.json so it persists between scenes and sessions
		Godot.Collections.Dictionary saveData;
		if (FileAccess.FileExists("user://savegame.json"))
		{
			using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			saveData = result.As<Godot.Collections.Dictionary>() ?? new Godot.Collections.Dictionary();
		}
		else
		{
			saveData = new Godot.Collections.Dictionary();
		}
		saveData["volume"] = (float)value;
		string newJson = Json.Stringify(saveData);
		using var writeFile = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
		writeFile.StoreString(newJson);

		// Emit the signal so other scripts (like MainGame) can react to the volume change
		EmitSignal(nameof(VolumeChangedEventHandler), (float)value);

	}

	private void OnBackButtonPressed()
	{
		if (OpenedFromGame)
		{
			QueueFree(); // Just close settings if in-game
		}
		
	}

	private void OnMainMenuButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/title_screen.tscn");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void GoToTitleScreen()
	{
		GetTree().ChangeSceneToFile("res://scenes/title_screen.tscn");
	}
}
