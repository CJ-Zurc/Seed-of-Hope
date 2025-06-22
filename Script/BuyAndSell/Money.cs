using Godot;
using System;

public partial class Money : CanvasLayer
{
	private Label moneyLabel;
	private Button optionButton;

	
	public override void _Ready()
	{
		moneyLabel = GetNode<Label>("Control/moneyContainer/money");
		var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");
		optionButton = GetNode<Button>("Control/optionButton");
		moneyLabel.Text = moneyManager.CurrentMoney.ToString();
		optionButton.Pressed += OnOptionButtonPressed;

		// Connect the MoneyChanged signal to update the label when money changes
		moneyManager.MoneyChanged += OnMoneyChanged;
	}

	private void OnMoneyChanged(int newAmount)
	{
		moneyLabel.Text = newAmount.ToString();
	}

	private void OnOptionButtonPressed()
	{
		var settingsScene = GD.Load<PackedScene>("res://scenes/settings.tscn");
		var settingsInstance = settingsScene.Instantiate();
		AddChild(settingsInstance);
	}
}
