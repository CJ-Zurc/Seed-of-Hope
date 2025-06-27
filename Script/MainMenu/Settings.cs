using Godot;
using System;

public partial class Settings : Control
{
	[Export] public NodePath BackButtonPath;
	[Export] public NodePath VolumeSliderPath;
	[Export] public NodePath MuteCheckBoxPath;

	private Button backButton;
	private Volume volumeSlider;
	private CheckBox muteCheckBox;
	private bool isMuting = false;
	private bool isInitializing = false;
	private float previousVolumeDb = 0f;
	private MainGame mainGame;
	public MainGame MainGame { get; set; }

	// Name of your custom bus (must match the name in the Audio panel)
	private const string MusicBusName = "Master";

	
	// Signal for when the volume changes, to notify other scripts (e.g., MainGame)
	[Signal]
	public delegate void VolumeChangedEventHandler(float newVolume);
	

	// <--- PUT THE CONSTRUCTOR HERE --->
	public Settings()
	{
		GD.Print("Settings constructor called");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Settings _Ready called");
		backButton = GetNode<Button>(BackButtonPath);
		backButton.Pressed += OnBackButtonPressed;

		volumeSlider = GetNode<Volume>(VolumeSliderPath);
		volumeSlider.ValueChanged += OnVolumeChanged;

		muteCheckBox = GetNode<CheckBox>(MuteCheckBoxPath);
		muteCheckBox.Toggled += OnMuteToggled;

		

        // Sync mute state and checkbox
        bool isMuted = mainGame != null ? mainGame.IsMuted() : false;
		isInitializing = true;
		muteCheckBox.ButtonPressed = isMuted;

		// Optionally, also sync the slider to the correct value
		float busDb = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(MusicBusName));
		float minDb = -40f;
		float maxDb = 0f;
		float linear = Mathf.InverseLerp(minDb, maxDb, busDb);
		float sliderValue = Mathf.Clamp(linear * 100f, (float)volumeSlider.MinValue, (float)volumeSlider.MaxValue);
		volumeSlider.Value = sliderValue;
		isInitializing = false;
	}

	private void OnVolumeChanged(double value)
	{
		if (isMuting || isInitializing)
			return; // Don't process if we're muting or initializing

		// Convert slider value (0-100) to dB (-40 to 0) for the audio bus
		float minDb = -40f;
		float maxDb = 0f;
		float linear = (float)value / 100f;
		float db = Mathf.Lerp(minDb, maxDb, linear);

		GD.Print($"Settings.cs: Slider changed to {value}, setting dB to {db}");

		int busIdx = AudioServer.GetBusIndex(MusicBusName);
		if (busIdx >= 0)
			AudioServer.SetBusVolumeDb(busIdx, db);
		else
			GD.PrintErr($"Audio bus '{MusicBusName}' not found!");

		// Emit the signal so other scripts (like MainGame) can react to the volume change
		EmitSignal(nameof(VolumeChanged), db); // Emit dB value
	}

	private void OnBackButtonPressed()
	{
		// Save the current slider value to JSON
		float minDb = -40f;
		float maxDb = 0f;
		float linear = (float)volumeSlider.Value / 100f;
		float db = Mathf.Lerp(minDb, maxDb, linear);

		if (mainGame != null)
			mainGame.SetVolume(db); // This will save to JSON

		QueueFree();
	}

	private void OnMuteToggled(bool buttonPressed)
	{
		GD.Print($"OnMuteToggled called with: {buttonPressed}");
		if (mainGame != null)
			mainGame.SetMute(buttonPressed);
		else
			GD.PrintErr("mainGame reference is null!");

		if (buttonPressed)
		{
			// Save the current volume before muting
			previousVolumeDb = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(MusicBusName));
			isMuting = true;
			volumeSlider.Mute();
			isMuting = false;
		}
		else
		{
			// Restore previous volume
			int busIdx = AudioServer.GetBusIndex(MusicBusName);
			AudioServer.SetBusVolumeDb(busIdx, previousVolumeDb);
			// Update slider and save to JSON
			float minDb = -40f;
			float maxDb = 0f;
			float linear = Mathf.InverseLerp(minDb, maxDb, previousVolumeDb);
			float sliderValue = Mathf.Clamp(linear * 100f, (float)volumeSlider.MinValue, (float)volumeSlider.MaxValue);
			volumeSlider.Value = sliderValue;
			// Save to JSON (already handled by SetMute)
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
