using Godot;
using System;

public partial class InventoryPanel : PanelContainer
{
	private Button waterButton;

    public override void _Ready()
	{
        // Connects the water button to the OnWaterButtonPressed method
        // This will change the mouse cursor to a watering can when the button is pressed
        waterButton = GetNode<Button>("/root/MainGame/HUD/Control/inventoryPanel/MarginContainer/HBoxContainer/waterCan");
        waterButton.Pressed += OnWaterButtonPressed;

    }
	private void OnWaterButtonPressed()
	{
        //loads the watering can texture and sets it as the custom mouse cursor
        Texture2D wateringCan = GD.Load<Texture2D>("res://2D Arts/GardenAssets/wateringCan.png");
        // Sets the custom mouse cursor to the watering can texture
        Input.SetCustomMouseCursor(wateringCan, Input.CursorShape.Arrow, Vector2.Zero);
    }
 
}
