using Godot;
using System;

public abstract partial class PlantableArea : Area2D
{
    // Grouped variables for saving/loading
    // I think ito yung mga kailangan i-save sa plant cinompile ko na pero test nyo if ever ayaw gumana	
    public abstract string UniqueID { get; }
    public abstract string CurrentSeedType { get; }
    public abstract int GrowthDaysLeft { get; }
    public abstract float WaterLevel { get; }
    public abstract bool IsPlanted { get; }
    public abstract void LoadState(Godot.Collections.Dictionary data);
    public abstract Godot.Collections.Dictionary SaveState();

    // Abstract actions
    public abstract void Plant(string seedType);
    public abstract void Water();
}
