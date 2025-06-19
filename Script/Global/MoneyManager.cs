using Godot;
using System;

public partial class MoneyManager : Node
{
    // Signal to notify when the money changes
	[Signal]
	public delegate void MoneyChangedEventHandler(int newAmount);

    // Variable to hold the current amount of money
	private int currentMoney = 0;
	public int CurrentMoney
	{
		get => currentMoney;
		set
		{
			if (currentMoney != value)
			{
				currentMoney = value;
				EmitSignal(nameof(MoneyChanged), currentMoney);
			}
		}
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		currentMoney = 200; // Initialize with a default amount of money
    }

	public void AddMoney(int amount)
	{
		if (amount > 0)
		{
			CurrentMoney += amount;
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
