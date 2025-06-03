using Godot;
using System;

public partial class CurrencyManager : Node
{
    [Signal]
    public delegate void CurrencyChangedEventHandler(float newAmount);
    
    private float _akimCoins = 100.0f; // Starting amount
    
    public float AkimCoins
    {
        get => _akimCoins;
        private set
        {
            _akimCoins = value;
            EmitSignal(SignalName.CurrencyChanged, _akimCoins);
            GD.Print($"AkimCoins: {_akimCoins}");
        }
    }
    
    public bool CanAfford(float amount)
    {
        return _akimCoins >= amount;
    }
    
    public bool SpendCoins(float amount)
    {
        if (!CanAfford(amount)) return false;
        
        AkimCoins -= amount;
        return true;
    }
    
    public void AddCoins(float amount)
    {
        AkimCoins += amount;
    }
    
    // Save/Load methods
    public float GetCurrentCoins()
    {
        return _akimCoins;
    }
    
    public void LoadCoins(float amount)
    {
        AkimCoins = amount;
    }
} 