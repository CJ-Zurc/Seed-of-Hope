using Godot;
using System;
using System.Collections.Generic;

public partial class MainGame : Node2D
{
    [Export] public NodePath modulatePath;
    [Export] public AudioStream wakeUpSound;
    [Export] public AudioStream eveningSound;

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
    private List<FarmLandTile> farmTiles = new List<FarmLandTile>();

    public override void _Ready()
    {
        canvasModulate = GetNode<CanvasModulate>(modulatePath);
        timeLabel = GetNode<Label>("HUD/Control/containerDateTime/backgroundColor/timeContainer/time");
        yearWeekLabel = GetNode<Label>("HUD/Control/containerDateTime/backgroundColor/yearWeek");

        audioPlayer = new AudioStreamPlayer();
        AddChild(audioPlayer); // Add dynamically

        // Find all farm tiles in the scene
        FindFarmTiles();
    }

    private void FindFarmTiles()
    {
        // Find all FarmLandTile nodes in the scene
        var farmTileNodes = GetTree().GetNodesInGroup("FarmTiles");
        foreach (var node in farmTileNodes)
        {
            if (node is FarmLandTile farmTile)
            {
                farmTiles.Add(farmTile);
            }
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

            // Advance all plants to next day
            foreach (var farmTile in farmTiles)
            {
                farmTile.AdvanceDay();
            }
        }

        // Update plant water levels
        foreach (var farmTile in farmTiles)
        {
            farmTile.UpdatePlant(time);
        }

        PlayScheduledAudio();
        UpdateTimeLabel();
        UpdateLighting();
    }

    public float GetCurrentTime()
    {
        return time;
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

}
