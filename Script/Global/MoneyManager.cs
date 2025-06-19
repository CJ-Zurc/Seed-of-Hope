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
    // Method to add money

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
