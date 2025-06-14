using Godot;
using System;

public partial class TvTrigger : SceneTrigger
{
	
    

    public override void _Ready()
    
    {
        SceneToLoadPath = "res://scenes/weather report.tscn";
        base._Ready();
    }
    

    
}
