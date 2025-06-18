using Godot;
using System;

public partial class InventorySeedsPanel : PanelContainer
{
	private Button ampalayaButton;
    private Button calamansiButton;
    private Button pechayButton;
    private Button succulentButton;
    private Button sunflowerButton;

    private Label sunflowerLabel;
    private Label ampalayaLabel;
    private Label calamansiLabel;
    private Label pechayLabel;
    private Label succulentLabel;


    //inventory seed manager to manage the seeds in the inventory
    private SeedInventoryManager seedInventoryManager;
    public override void _Ready()
	{
        //gets the reference of the SeedInventoryManager
        seedInventoryManager = GetNode<SeedInventoryManager>("/root/SeedInventoryManager");
        seedInventoryManager.InventoryChanged += OnInventoryChanged;

        //gets the labels for each seed in the inventory
        sunflowerLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/sunflowerSeed/sunflowerLabelN");
        ampalayaLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/ampalayaSeed/ampalayaLabelN");
        calamansiLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/calamansiSeed/calamansiLabelN");
        pechayLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/pechaySeed/pechayLabelN");
        succulentLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventorySeedsPanel/MarginContainer/HBoxContainer/succulentSeed/succulentLabelN");

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

        OnInventoryChanged(); // Initialize the labels with current inventory counts

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

    private void OnInventoryChanged()
    {
        // Get the inventory from the HarvestManager
        var inventory = seedInventoryManager.Inventory;

        // Update the sunflower label with the current count of sunflowers in the inventory
        int sunflowerCount = inventory.ContainsKey("Sunflower") ? inventory["Sunflower"] : 0;
        sunflowerLabel.Text = sunflowerCount.ToString();

        // Update the ampalaya label with the current count of ampalayas in the inventory
        int ampalayaCount = inventory.ContainsKey("Ampalaya") ? inventory["Ampalaya"] : 0;
        ampalayaLabel.Text = ampalayaCount.ToString();

        // Update the calamansi label with the current count of calamansis in the inventory
        int calamansiCount = inventory.ContainsKey("Calamansi") ? inventory["Calamansi"] : 0;
        calamansiLabel.Text = calamansiCount.ToString();

        // Update the pechay label with the current count of pechays in the inventory
        int pechayCount = inventory.ContainsKey("Pechay") ? inventory["Pechay"] : 0;
        pechayLabel.Text = pechayCount.ToString();

        // Update the succulent label with the current count of succulents in the inventory
        int succulentCount = inventory.ContainsKey("Succulent") ? inventory["Succulent"] : 0;
        succulentLabel.Text = succulentCount.ToString();

    }
}
