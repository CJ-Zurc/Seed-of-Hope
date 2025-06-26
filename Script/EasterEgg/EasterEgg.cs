using Godot;
using System;

public partial class EasterEgg : SceneTrigger
{
    public override void _Ready()
	{
        Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
        Connect("body_exited", new Callable(this, nameof(OnBodyExited)));
        SceneToLoadPath = "res://scenes/easter_egg.tscn";
        base._Ready();
    }

    private void OnBodyEntered(Node body) // This method is called when a body enters the trigger area
    {
        if (body.Name == "player")
        {
            // Get MainGame (the parent node)
            var mainGame = GetParent<MainGame>();
            if (mainGame != null)
            {
                mainGame.PlayMusic(mainGame.easterEggMusic); // Play the Easter egg music on body entered
            }
        }
    }

    private void OnBodyExited(Node body) // This method is called when a body exits the trigger area
    {
        if (body.Name == "player") 
        {
            var mainGame = GetParent<MainGame>();
            if (mainGame != null)
            {
                if (mainGame.GetHourCount() >= 6f && mainGame.GetHourCount() < 19f) // Check if the time is between 6 AM and 7 PM 
                    mainGame.PlayMusic(mainGame.wakeUpSound); // Play the morning sound if it's daytime after exiting the Easter egg area
                else
                    mainGame.PlayMusic(mainGame.eveningSound); // Play the evening sound if it's nighttime after exiting the Easter egg area
            }
 
            }
        }
    }
