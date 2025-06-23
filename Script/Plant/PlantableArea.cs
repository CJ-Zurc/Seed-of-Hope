using Godot;
using System;
using System.Collections.Generic;

public abstract partial class PlantableArea : Area2D
{
    // Grouped variables for saving/loading
    public abstract string UniqueID { get; }
    public abstract string CurrentSeedType { get; }
    public abstract int GrowthDaysLeft { get; }
    public abstract float WaterLevel { get; }
    public abstract bool IsPlanted { get; }
    public abstract void LoadState(Dictionary<string, object> data);
    public abstract Dictionary<string, object> SaveState();

    // Abstract actions
    public abstract void Plant(string seedType);
    public abstract void Water();
}
