using Godot;
using System;

public partial class InventorySeedsPanel : PanelContainer
{
	private Button ampalayaButton;
    private Button calamansiButton;
    private Button pechayButton;
    private Button succulentButton;
    private Button sunflowerButton;


    //inventory seed manager to manage the seeds in the inventory
    private SeedInventoryManager seedInventoryManager;
    public override void _Ready()
	{
        //gets the reference of the SeedInventoryManager
        seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");

        //gets the ampalayaButton from the node
        ampalayaButton = GetNode<Button>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/ampalayaSeed/ampalayaButton");
        ampalayaButton.Pressed += OnAmpalayaButtonPressed;

        //gets the calamansiButton from the node
        calamansiButton = GetNode<Button>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/calamansiSeed/calamansiButton");
        calamansiButton.Pressed += OnCalamansiButtonPressed;

        //gets the pechayButton from the node
        pechayButton = GetNode<Button>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/pechaySeed/pechayButton");
        pechayButton.Pressed += OnPechayButtonPressed;

        //gets the succulentButton from the node
        succulentButton = GetNode<Button>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/succulentSeed/succulentButton");
        succulentButton.Pressed += OnSucculentButtonPressed;

        //gets the sunflowerButton from the node
        sunflowerButton = GetNode<Button>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/sunflowerSeed/sunflowerButton");
        sunflowerButton.Pressed += OnSunflowerButtonPressed;

    }

    //function for when the ampalayaButton is pressed
    private void OnAmpalayaButtonPressed()
    {
        Texture2D ampalayaSmolSeed = GD.Load<Texture2D>("res://2D Arts/Plant Assets/ampalayaSmolSeed.png");
        Input.SetCustomMouseCursor(ampalayaSmolSeed, Input.CursorShape.Arrow, Vector2.Zero);
    }

    //function for when the calamansiButton is pressed
    private void OnCalamansiButtonPressed() 
    {
        Texture2D calamansiSmolSeed = GD.Load<Texture2D>("res://2D Arts/Plant Assets/calamansiSmolSeed.png");
        Input.SetCustomMouseCursor(calamansiSmolSeed, Input.CursorShape.Arrow, Vector2.Zero);
    }

    //function for when the pechayButton is pressed
    private void OnPechayButtonPressed() 
    {
        Texture2D pechaySmolSeed = GD.Load<Texture2D>("res://2D Arts/Plant Assets/pechaySmolSeed.png");
        Input.SetCustomMouseCursor(pechaySmolSeed, Input.CursorShape.Arrow, Vector2.Zero);
    }

    //function for when the succulentButton is pressed
    private void OnSucculentButtonPressed() 
    {
        Texture2D succulentSmolSeed = GD.Load<Texture2D>("res://2D Arts/Plant Assets/succulentSmolSeed.png");
        Input.SetCustomMouseCursor(succulentSmolSeed, Input.CursorShape.Arrow, Vector2.Zero);
    }

    //function for when the sunflowerButton is pressed
    private void OnSunflowerButtonPressed() 
    {
        Texture2D sunflowerSmolSeed = GD.Load<Texture2D>("res://2D Arts/Plant Assets/sunflowerSmolSeed.png");
        Input.SetCustomMouseCursor(sunflowerSmolSeed, Input.CursorShape.Arrow, Vector2.Zero);

    }
}
