using Godot;
using System;

public abstract partial class PlantableArea : Area2D
{
	public abstract void Plant(string seedType);
	public abstract void Water();
}
