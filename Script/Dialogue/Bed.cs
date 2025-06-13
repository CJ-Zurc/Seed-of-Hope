using Godot;
using System;

public partial class Bed : DialogueBase
{
    protected override void OnYesPressed()
    {
        var fadeScene = GD.Load<PackedScene>("res://scenes/spending_earning_summary.tscn");
        var fadeRoot = fadeScene.Instantiate<Control>();
        GetTree().Root.AddChild(fadeRoot);

        // Advance to next day
        var mainGame = GetTree().Root.GetNode<MainGame>("MainGame");
        mainGame.SkipToNextDay();

        // Simulate saving data
        var saveData = new Godot.Collections.Dictionary
        {
            { "day", 1 },
            { "player_position", new Vector2(100, 100) }
        };

        var innerControl = fadeRoot
            .GetNode<CanvasLayer>("CanvasLayer")
            .GetNode<Control>("Control");

        innerControl.MouseFilter = Control.MouseFilterEnum.Stop;

        innerControl.GuiInput += (InputEvent ev) =>
        {
            if (ev is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
            {
                fadeRoot.QueueFree();
                QueueFree();
            }
        };
    }
}
