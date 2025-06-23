using Godot;
using System;

public partial class Faucet : Area2D
{
    public override void _Ready()
    {
        // Enable input pickable
        SetProcessInput(true);
        InputPickable = true;
    }

    public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            var moneyHUD = GetNode<Money>("/root/MainGame/HUD");
            moneyHUD.RefillWateringCan();
        }
    }
}