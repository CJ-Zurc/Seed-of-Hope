using Godot;
using System;

public partial class BedTrigger : SceneTrigger
{
 
    public override void _Ready()
    {
        SceneToLoadPath = "res://scenes/BedDialogue.tscn";
        base._Ready();
    }

    
}
