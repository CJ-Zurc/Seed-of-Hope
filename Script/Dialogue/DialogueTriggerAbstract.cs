// SceneTrigger.cs
using Godot;
using System;

public abstract partial class SceneTrigger : Area2D
{
    [Export] public NodePath playerPath;
    public string SceneToLoadPath = "";

    protected CharacterBody2D player;
    protected Control activeScene;

    public override void _Ready()
    {
        player = GetNode<CharacterBody2D>(playerPath);
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body == player && activeScene == null && !string.IsNullOrEmpty(SceneToLoadPath))
        {
            var packedScene = GD.Load<PackedScene>(SceneToLoadPath);
            if (packedScene != null)
            {
                activeScene = packedScene.Instantiate<Control>();
                GetTree().Root.AddChild(activeScene);

                activeScene.TreeExited += () => activeScene = null;
            }
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body == player && activeScene != null)
        {
            activeScene.QueueFree();
            activeScene = null;
        }
    }
}
