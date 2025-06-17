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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        backButton = GetNode<Button>(BackButtonPath);
        backButton.Pressed += OnBackButtonPressed;

        volumeSlider = GetNode<HSlider>(VolumeSliderPath);
        volumeSlider.ValueChanged += OnVolumeChanged;

        // Initialize slider to current volume of the custom bus
        int busIdx = AudioServer.GetBusIndex(MusicBusName);
        volumeSlider.Value = AudioServer.GetBusVolumeDb(busIdx);
    }

    private void OnVolumeChanged(double value)
    {
        int busIdx = AudioServer.GetBusIndex(MusicBusName);
        AudioServer.SetBusVolumeDb(busIdx, (float)value);
    }

    private void OnBackButtonPressed()
    {
        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }
}
