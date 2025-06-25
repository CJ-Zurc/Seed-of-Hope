using Godot;
using System;

public partial class MoneyManager : Node
{
    // Variable to hold the current amount of money
    private int currentMoney = 0;

    // Signal to notify when the money changes
    [Signal]
    public delegate void MoneyChangedEventHandler(int newAmount);

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

    public override void _Ready()
    {
        // Do not set CurrentMoney here!
    }

    public void AddMoney(int amount)
    {
        if (amount > 0)
        {
            CurrentMoney += amount; // Use the property!
        }
    }

    public bool RemoveMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            CurrentMoney -= amount;
            return true;
        }
        return false; // Not enough money
    }
}