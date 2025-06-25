using Godot;
using System;

public partial class Money : CanvasLayer
{
	private Label moneyLabel;
	private Button optionButton;
	private TextureProgressBar wateringCanBar;

	private float wateringCanLevel = 1.0f; // 1.0 = 100%

	public override void _Ready()
	{
		moneyLabel = GetNode<Label>("Control/moneyContainer/money");
		var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");
		optionButton = GetNode<Button>("Control/optionButton");
		moneyLabel.Text = moneyManager.CurrentMoney.ToString();
		optionButton.Pressed += OnOptionButtonPressed;

		// Connect the MoneyChanged signal to update the label when money changes
		moneyManager.MoneyChanged += OnMoneyChanged;

		wateringCanBar = GetNode<TextureProgressBar>("Control/waterBar/TextureProgressBar");
		wateringCanBar.Value = wateringCanLevel * 100f;
	}
	

	private void OnMoneyChanged(int newAmount)
	{
		moneyLabel.Text = newAmount.ToString();
	}

	private void OnOptionButtonPressed()
	{
		// Find the MainGame node and call OpenSettings
		var mainGame = GetTree().CurrentScene as MainGame;
		if (mainGame != null)
		{
			mainGame.Call("OpenSettings");
		}
		else
		{
			// Fallback: open settings directly if not in main game
			var settingsScene = GD.Load<PackedScene>("res://scenes/settings.tscn");
			var settingsInstance = settingsScene.Instantiate();
			AddChild(settingsInstance);
		}
	}

	public void UseWateringCan()
	{
		wateringCanLevel -= 0.05f; // Decrease by 5%
		if (wateringCanLevel < 0f) wateringCanLevel = 0f;
		wateringCanBar.Value = wateringCanLevel * 100f;
	}

	public void RefillWateringCan()
	{
		wateringCanLevel = 1.0f;
		wateringCanBar.Value = 100f;
	}
}
