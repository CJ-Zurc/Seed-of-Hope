using Godot;
using System;

public partial class Money : CanvasLayer
{
	private Label moneyLabel;
    public override void _Ready()
	{
		moneyLabel = GetNode<Label>("Control/moneyContainer/money");
		var moneyManager = GetNode<MoneyManager>("/root/MoneyManager");
		moneyLabel.Text = moneyManager.CurrentMoney.ToString();

        // Connect the MoneyChanged signal to update the label when money changes
		moneyManager.MoneyChanged += OnMoneyChanged;
    }

	private void OnMoneyChanged(int newAmount)
	{
		moneyLabel.Text = newAmount.ToString();
	}

}
