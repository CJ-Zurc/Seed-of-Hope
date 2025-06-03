using Godot;
using System;

public partial class Bed : Node2D
{
    private Area2D _interactionArea;
    private TimeManager _timeManager;
    private PopupMenu _sleepMenu;
    private Label _cannotSleepLabel;
    private bool _showingCannotSleepMessage = false;
    private double _messageTimer = 0;
    
    public override void _Ready()
    {
        _timeManager = GetNode<TimeManager>("/root/TimeManager");
        
        // Setup interaction area
        _interactionArea = GetNode<Area2D>("InteractionArea");
        _interactionArea.AddToGroup("interactable");
        
        // Create sleep menu
        CreateSleepMenu();
        
        // Create cannot sleep message
        CreateCannotSleepLabel();
    }
    
    private void CreateSleepMenu()
    {
        _sleepMenu = new PopupMenu();
        _sleepMenu.AddItem("Sleep until morning (8 hours)", 0);
        _sleepMenu.IdPressed += OnSleepMenuItemSelected;
        AddChild(_sleepMenu);
    }
    
    private void CreateCannotSleepLabel()
    {
        _cannotSleepLabel = new Label();
        _cannotSleepLabel.Text = "You can only sleep after 8 PM";
        _cannotSleepLabel.Position = new Vector2(-100, -50);
        _cannotSleepLabel.Modulate = new Color(1, 0.2f, 0.2f); // Red text
        _cannotSleepLabel.Visible = false;
        AddChild(_cannotSleepLabel);
    }
    
    public override void _Process(double delta)
    {
        if (_showingCannotSleepMessage)
        {
            _messageTimer += delta;
            if (_messageTimer >= 2.0) // Show message for 2 seconds
            {
                _cannotSleepLabel.Visible = false;
                _showingCannotSleepMessage = false;
                _messageTimer = 0;
            }
        }
    }
    
    private void OnSleepMenuItemSelected(long id)
    {
        if (id == 0) // Sleep option selected
        {
            TryToSleep();
        }
        _sleepMenu.Hide();
    }
    
    private void TryToSleep()
    {
        int currentHour = _timeManager.GetCurrentHour();
        
        if (currentHour >= 20 || currentHour < 4) // Can sleep between 8 PM and 4 AM
        {
            // Save the game before sleeping
            SaveManager.SaveGame();
            
            // Calculate how many hours to advance
            int hoursToSleep = 8;
            int targetHour = 6; // Wake up at 6 AM
            
            // Advance time
            for (int i = 0; i < hoursToSleep; i++)
            {
                _timeManager.AdvanceHour();
            }
            
            GD.Print("You slept well and woke up refreshed!");
            
            // Save again after sleeping
            SaveManager.SaveGame();
        }
        else
        {
            ShowCannotSleepMessage();
        }
    }
    
    private void ShowCannotSleepMessage()
    {
        _cannotSleepLabel.Visible = true;
        _showingCannotSleepMessage = true;
        _messageTimer = 0;
    }
    
    public void OnInteractionAreaMouseEntered()
    {
        if (_timeManager.GetCurrentHour() >= 20 || _timeManager.GetCurrentHour() < 4)
        {
            Vector2 mousePos = GetViewport().GetMousePosition();
            _sleepMenu.Position = mousePos;
            _sleepMenu.Popup();
        }
        else
        {
            ShowCannotSleepMessage();
        }
    }
    
    public void OnInteractionAreaMouseExited()
    {
        if (!_sleepMenu.GetRect().HasPoint(_sleepMenu.GetLocalMousePosition()))
        {
            _sleepMenu.Hide();
        }
    }
    
    // Called when player presses E near the bed
    public void OnInteract()
    {
        TryToSleep();
    }
} 