using Godot;
using System;

public partial class Teleporter : Area2D
{
	private CharacterBody2D player;
	private Area2D block;
	[Export]public int targetX = 10;
	[Export]public int targetY = 2;
	private TeleportHandler teleporter;
	[Export] public NodePath playerPath;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		player = GetNode<CharacterBody2D>(playerPath);
		teleporter = new CharacterTeleport(player);
		BodyEntered += OnBodyEntered;

	}

	private void OnBodyEntered(Node Body)
	{
		if (Body == player)
		{
			teleporter.TeleportTo(targetX, targetY);
	
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
