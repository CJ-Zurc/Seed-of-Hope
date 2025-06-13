using Godot;
using System;
using System.Collections.Generic;

public partial class BedInteraction : Node2D
{
    private MainGame _mainGame;
    private Player _player;
    private Control _sleepUI;
    private Control _reportUI;
    private Label _timeLabel;
    private Label _dateLabel;
    private Label _earningsLabel;
    private Label _expensesLabel;
    private Label _netProfitLabel;
    private Label _daysPassedLabel;
    
    private int _dailyEarnings = 0;
    private int _dailyExpenses = 0;
    
    public override void _Ready()
    {
        _mainGame = GetNode<MainGame>("/root/MainGame");
        _player = GetNode<Player>("/root/MainGame/Player");
        
        // Load UI scenes
        _sleepUI = GetNode<Control>("SleepUI");
        _reportUI = GetNode<Control>("ReportUI");
        
        // Get UI elements
        _timeLabel = _reportUI.GetNode<Label>("TimeLabel");
        _dateLabel = _reportUI.GetNode<Label>("DateLabel");
        _earningsLabel = _reportUI.GetNode<Label>("EarningsLabel");
        _expensesLabel = _reportUI.GetNode<Label>("ExpensesLabel");
        _netProfitLabel = _reportUI.GetNode<Label>("NetProfitLabel");
        _daysPassedLabel = _reportUI.GetNode<Label>("DaysPassedLabel");
        
        // Hide UIs initially
        _sleepUI.Hide();
        _reportUI.Hide();
    }
    
    public void TryToSleep()
    {
        var currentHour = _mainGame.GetCurrentHour();
        
        if (currentHour < 20) // Before 8 PM
        {
            ShowDialogue("Too early to sleep, wait till 8pm");
            return;
        }
        
        ShowSleepUI();
    }
    
    private void ShowSleepUI()
    {
        _sleepUI.Show();
        var sleepButton = _sleepUI.GetNode<Button>("SleepButton");
        var cancelButton = _sleepUI.GetNode<Button>("CancelButton");
        
        sleepButton.Pressed += Sleep;
        cancelButton.Pressed += () => _sleepUI.Hide();
    }
    
    private void Sleep()
    {
        _sleepUI.Hide();
        
        // Calculate sleep duration and wake up time
        var currentHour = _mainGame.GetCurrentHour();
        var hoursToSleep = 8; // Fixed 8 hours of sleep
        var wakeUpHour = (currentHour + hoursToSleep) % 24;
        
        // Advance time
        _mainGame.AdvanceTime(hoursToSleep);
        
        // Restore player stamina
        _player.RestoreStamina();
        
        // Save game progress
        SaveGame();
        
        // Show daily report
        ShowDailyReport();
    }
    
    private void ShowDailyReport()
    {
        _reportUI.Show();
        
        // Update report labels
        _timeLabel.Text = $"Time: {_mainGame.GetCurrentHour():00}:00";
        _dateLabel.Text = $"Day: {GetDayName(_mainGame.GetCurrentDay())}";
        _earningsLabel.Text = $"Daily Earnings: {_dailyEarnings} Akim Coins";
        _expensesLabel.Text = $"Daily Expenses: {_dailyExpenses} Akim Coins";
        _netProfitLabel.Text = $"Net Profit: {_dailyEarnings - _dailyExpenses} Akim Coins";
        _daysPassedLabel.Text = $"Days Passed: {_mainGame.GetDaysPassed()}";
        
        // Reset daily stats
        _dailyEarnings = 0;
        _dailyExpenses = 0;
        
        // Add continue button
        var continueButton = _reportUI.GetNode<Button>("ContinueButton");
        continueButton.Pressed += () => _reportUI.Hide();
    }
    
    private string GetDayName(int day)
    {
        return (day % 7) switch
        {
            0 => "Monday",
            1 => "Tuesday",
            2 => "Wednesday",
            3 => "Thursday",
            4 => "Friday",
            5 => "Saturday",
            6 => "Sunday",
            _ => "Unknown"
        };
    }
    
    private void ShowDialogue(string message)
    {
        var dialogueBox = GetNode<DialogueBox>("/root/MainGame/DialogueBox");
        dialogueBox.ShowMessage(message);
    }
    
    private void SaveGame()
    {
        // Create save data
        var saveData = new Dictionary<string, object>
        {
            { "current_hour", _mainGame.GetCurrentHour() },
            { "current_day", _mainGame.GetCurrentDay() },
            { "days_passed", _mainGame.GetDaysPassed() },
            { "player_currency", _player.GetCurrency() },
            { "player_stamina", _player.GetStamina() },
            { "inventory_items", _player.GetNode<Inventory>("Inventory").GetItems() }
        };
        
        // Save to file
        using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var json = Json.Stringify(saveData);
        saveGame.StoreLine(json);
    }
    
    public void AddEarnings(int amount)
    {
        _dailyEarnings += amount;
    }
    
    public void AddExpenses(int amount)
    {
        _dailyExpenses += amount;
    }
} 