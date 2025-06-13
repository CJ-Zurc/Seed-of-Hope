using Godot;
using System;

public partial class ComputerTrigger : SceneTrigger
{

    public override void _Ready()
    
    {
        SceneToLoadPath = "res://scenes/buy_and_sell.tscn";
        base._Ready();
    }
    

}
