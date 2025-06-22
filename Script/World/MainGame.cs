using Godot;
using System;

public partial class MainGame : Node2D
{
	[Export] public NodePath modulatePath;
	[Export] public AudioStream wakeUpSound;
	[Export] public AudioStream eveningSound;
	[Export] public PackedScene SettingsScene; // Assign your Settings.tscn in the editor

	private CanvasModulate canvasModulate;
	private Label timeLabel;
	private Label yearWeekLabel;
	private AudioStreamPlayer audioPlayer;
	private bool isMorningMusicPlaying = false;
	private bool isEveningMusicPlaying = false;

	private float time = 6f;
	private float speed = 600f;

	private int dayCount = 1;
	private int year = 1;

	private bool hasPlayedMorningSound = false;
	private bool hasPlayedEveningSound = false;

	private float autosaveTimer = 0f;
	private const float AUTOSAVE_INTERVAL = 10f; // seconds //Time interval for autosave

	// Volume settings: stores the current master volume in dB, and the name of the audio bus
	private float masterVolume = 0f; // Default volume in dB
	private const string MusicBusName = "Master";
	

	public override void _Ready()
	{
		canvasModulate = GetNode<CanvasModulate>(modulatePath);
		timeLabel = GetNode<Label>("HUD/Control/containerDateTime/backgroundColor/timeContainer/time");
		yearWeekLabel = GetNode<Label>("HUD/Control/containerDateTime/backgroundColor/yearWeek");

		audioPlayer = new AudioStreamPlayer();
		audioPlayer.Bus = "Master";
		AddChild(audioPlayer);

		// --- Save/load logic ---
		if (HasNode("player"))
		{
			var player = GetNode<Node2D>("player");
			if (!FileAccess.FileExists("user://savegame.json"))
			{
				// File does not exist: create with default values
				player.Position = new Vector2(-16, -459); // Set your default starting position here
				dayCount = 1;
				year = 1;
				time = 6f;
				
				masterVolume = 0f; // Default volume
				

				SaveGame(); // Save the initial state
			}
			else
			{
				// File exists: load values
				using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
				var json = file.GetAsText();
				var result = Json.ParseString(json);
				var saveData = result.As<Godot.Collections.Dictionary>();
				if (saveData != null)
				{
					if (HasNode("player"))
					{
						var playerNode = GetNode<Node2D>("player");

						if (saveData.ContainsKey("player_position"))
						{
							var posVariant = saveData["player_position"];
							var posArray = posVariant.As<Godot.Collections.Array>();

							if (posArray != null && posArray.Count == 2)
							{
								float x = (float)(double)posArray[0];
								float y = (float)(double)posArray[1];
								playerNode.GlobalPosition = new Vector2(x, y);
							}
						}
					}
					if (saveData.ContainsKey("day"))
						dayCount = (int)saveData["day"];
					if (saveData.ContainsKey("year"))
						year = (int)saveData["year"];
					if (saveData.ContainsKey("time"))
						time = (float)saveData["time"];
					if (saveData.ContainsKey("volume"))
						
						masterVolume = (float)saveData["volume"];
						
				}
			}
		}

		
		// Apply the loaded or default volume to the audio bus
		int busIdx = AudioServer.GetBusIndex(MusicBusName);
		AudioServer.SetBusVolumeDb(busIdx, masterVolume);
	   

		UpdateYearLabel();
		UpdateTimeLabel();
		UpdateLighting();

		// Play background music based on loaded time
		if (time >= 6f && time < 19f)
		{
			audioPlayer.Stream = wakeUpSound;
			audioPlayer.Play();
			hasPlayedMorningSound = true;
			hasPlayedEveningSound = false;
		}
		else if (time >= 19f || time < 6f)
		{
			audioPlayer.Stream = eveningSound;
			audioPlayer.Play();
			hasPlayedMorningSound = false;
			hasPlayedEveningSound = true;
		}
	}



	public override void _Process(double delta)
	{
		time += (float)delta * (24f / speed);
		if (time >= 24f)
		{
			time = 0f;
			dayCount++;
			if (dayCount > 30)
			{
				dayCount = 1;
				year++;
			}
			UpdateYearLabel();

			// Reset audio triggers for the new day
			hasPlayedMorningSound = false;
			hasPlayedEveningSound = false;
		}

		PlayScheduledAudio();

		UpdateTimeLabel();
		UpdateLighting();

		// Autosave logic
		autosaveTimer += (float)delta;
		if (autosaveTimer >= AUTOSAVE_INTERVAL)
		{
			autosaveTimer = 0f;
			SaveGame();
		}
	}

	private void PlayScheduledAudio()
	{
		// Play at 6:00 AM
		if (!hasPlayedMorningSound && time >= 6f && time < 6.1f)
		{
			audioPlayer.Stream = wakeUpSound;
			audioPlayer.Play();
			hasPlayedMorningSound = true;
		}

		// Play at 7:00 PM
		if (!hasPlayedEveningSound && time >= 19f && time < 19.1f)
		{
			audioPlayer.Stream = eveningSound;
			audioPlayer.Play();
			hasPlayedEveningSound = true;
		}
	}

	private void UpdateTimeLabel()
	{
		int hour = (int)time;
		int minutes = (int)((time - hour) * 60);
		string formattedTime = $"{hour:D2}:{minutes:D2}";
		timeLabel.Text = formattedTime;
	}

	private void UpdateYearLabel()
	{
		yearWeekLabel.Text = $"Year {year}, Day {dayCount}";
	}

	private void UpdateLighting()
	{
		if (time >= 6 && time < 8)
			canvasModulate.Color = new Color("a9b4eb");
		else if (time >= 8 && time < 10)
			canvasModulate.Color = new Color("c4ccf2");
		else if (time >= 10 && time < 12)
			canvasModulate.Color = new Color("f4effe");
		else if (time >= 12 && time < 15)
			canvasModulate.Color = new Color("fffff3");
		else if (time >= 15 && time < 17)
			canvasModulate.Color = new Color("f9f8b3");
		else if (time >= 17 && time < 19)
			canvasModulate.Color = new Color("cee397");
		else if (time >= 19 && time < 24)
			canvasModulate.Color = new Color("8db1ab");
		else
			canvasModulate.Color = new Color("587792");
	}

	public void SkipToNextDay()
	{
		time = 6f;
		dayCount++;
		if (dayCount > 30)
		{
			dayCount = 1;
			year++;
		}

		hasPlayedMorningSound = false;
		hasPlayedEveningSound = false;

		UpdateYearLabel();
		UpdateTimeLabel();
		UpdateLighting();
	}

	
	// Method to update volume (called from Settings)
	public void SetVolume(float volume)
	{
		GD.Print("Setting volume to: " + volume);
		masterVolume = volume;
		int busIdx = AudioServer.GetBusIndex(MusicBusName);
		if (busIdx >= 0)
		{
			AudioServer.SetBusVolumeDb(busIdx, volume);
			GD.Print("Volume set in audio bus: " + volume);
		}
		else
		{
			GD.PrintErr("Audio bus 'Master' not found!");
		}
		SaveGame(); // Save immediately when volume changes
	}
	

	// Method to get current volume (called from Settings)
	public float GetVolume()
	{
		return masterVolume;
	}

	private void SaveGame()
	{
		// Example: Save player position, day, time, etc.
		var saveData = new Godot.Collections.Dictionary();

        if (HasNode("player"))
        {
            var player = GetNode<Node2D>("player");
            var pos = player.GlobalPosition;
            saveData["player_position"] = new Godot.Collections.Array { pos.X, pos.Y };
        }
        saveData["day"] = dayCount;
        saveData["year"] = year;
        saveData["time"] = time;

        string json = Json.Stringify(saveData);
        using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }
}
