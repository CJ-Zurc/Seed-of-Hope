using Godot;
using System;

public partial class InventoryPanel : PanelContainer
{
	private Button waterButton;
	private Texture2D defaultCursor;

    public override void _Ready()
	{

        defaultCursor = GD.Load<Texture2D>("res://2D Arts/UI stuff/Buttons/Triangle Mouse icon 1_64x64.png");
        Input.SetCustomMouseCursor(defaultCursor, Input.CursorShape.Arrow, Vector2.Zero);

        waterButton = GetNode<Button>("/root/MainGame/HUD/Control/inventoryPanel/MarginContainer/HBoxContainer/waterCan");
        waterButton.Pressed += OnWaterButtonPressed;

    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
        {
            // Reset the mouse cursor to the default custom icon when right mouse button is pressed
            Input.SetCustomMouseCursor(defaultCursor, Input.CursorShape.Arrow, Vector2.Zero);
        }
    }

	private void OnWaterButtonPressed()
	{
        //loads the watering can texture and sets it as the custom mouse cursor
        Texture2D wateringCan = GD.Load<Texture2D>("res://2D Arts/GardenAssets/wateringCan.png");
 
        Input.SetCustomMouseCursor(wateringCan, Input.CursorShape.Arrow, Vector2.Zero);
    }
 
}
