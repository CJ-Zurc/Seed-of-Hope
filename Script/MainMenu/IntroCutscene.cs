using Godot;
using System;

public partial class IntroCutscene : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("Intro_Cutscene");
		animationPlayer.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName == "Intro_Cutscene")
		{
			GetTree().ChangeSceneToFile("res://scenes/main_game.tscn"); // Update path if needed
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
