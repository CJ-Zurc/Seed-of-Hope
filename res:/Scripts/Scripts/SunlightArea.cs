using Godot;
using System;

public partial class SunlightArea : Area2D
{
    public override void _Ready()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }
    
    private void OnAreaEntered(Area2D area)
    {
        if (area.GetParent() is Plant plant)
        {
            plant.SetInSunlight(true);
        }
    }
    
    private void OnAreaExited(Area2D area)
    {
        if (area.GetParent() is Plant plant)
        {
            plant.SetInSunlight(false);
        }
    }
} 