using Godot;
using System;
using System.Collections.Generic;
public partial class InventoryHarvestedPanel : PanelContainer
{
	private Label sunflowerLabel;
    private Label ampalayaLabel;
    private Label calamansiLabel;   
    private Label pechayLabel;
    private Label succulentLabel;
    public override void _Ready()
	{
        // Initialize the labels for each harvested item
        sunflowerLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventoryHarvestedPanel/MarginContainer/VBoxContainer/sunflower/sunflowerLabel");
        ampalayaLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventoryHarvestedPanel/MarginContainer/VBoxContainer/ampalaya/ampalayaLabel");
        calamansiLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventoryHarvestedPanel/MarginContainer/VBoxContainer/calamansi/calamansiLabel");
        pechayLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventoryHarvestedPanel/MarginContainer/VBoxContainer/pechay/pechayLabel");
        succulentLabel = GetNode<Label>("/root/MainGame/HUD/Control/inventoryHarvestedPanel/MarginContainer/VBoxContainer/succulent/succulentLabel");

        //connects this script to the global Harvestmanager signal
        var harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
        // Connects the InventoryChanged signal to the OnInventoryChanged method
        harvestManager.InventoryChanged += OnInventoryChanged;
	}

	private void OnInventoryChanged()
	{
        // This method is called whenever the inventory changes
        var harvestManager = GetNode<HarvestManager>("/root/HarvestManager");
        // Get the inventory from the HarvestManager
        var inventory = harvestManager.Inventory;

        // Update the sunflower label with the current count of sunflowers in the inventory
        int sunflowerCount = inventory.ContainsKey("Sunflower") ? inventory["Sunflower"]: 0;
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
