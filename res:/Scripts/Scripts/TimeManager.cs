using Godot;
using System;

public partial class TimeManager : Node
{
    [Signal]
    public delegate void HourChangedEventHandler(int hour);
    
    [Signal]
    public delegate void DayChangedEventHandler(int day);
    
    [Signal]
    public delegate void SeasonChangedEventHandler(Season season, int year);
    
    [Export]
    public float TimeScale { get; set; } = 60.0f; // 1 real second = 1 game minute
    
    private double _currentMinute = 0;
    private int _currentHour = 6; // Start at 6 AM
    private int _currentDay = 1;
    private Season _currentSeason = Season.Spring;
    private int _currentYear = 1;
    
    // Constants
    private const int MINUTES_PER_HOUR = 60;
    private const int HOURS_PER_DAY = 24;
    private const int DAYS_PER_SEASON = 30;
    private const int SEASONS_PER_YEAR = 4;
    
    public enum Season
    {
        Spring,
        Summer,
        Fall,
        Winter
    }
    
    public override void _Ready()
    {
        // Start the day at 6 AM
        EmitSignal(SignalName.HourChanged, _currentHour);
        EmitSignal(SignalName.DayChanged, _currentDay);
        EmitSignal(SignalName.SeasonChanged, _currentSeason, _currentYear);
    }
    
    public override void _Process(double delta)
    {
        // Update time
        _currentMinute += delta * TimeScale;
        
        // Check for hour change
        if (_currentMinute >= MINUTES_PER_HOUR)
        {
            _currentMinute -= MINUTES_PER_HOUR;
            _currentHour++;
            EmitSignal(SignalName.HourChanged, _currentHour);
            
            // Check for day change
            if (_currentHour >= HOURS_PER_DAY)
            {
                _currentHour = 0;
                _currentDay++;
                EmitSignal(SignalName.DayChanged, _currentDay);
                
                // Check for season change
                if (_currentDay > DAYS_PER_SEASON)
                {
                    _currentDay = 1;
                    _currentSeason = (Season)(((int)_currentSeason + 1) % SEASONS_PER_YEAR);
                    
                    // Check for year change
                    if (_currentSeason == Season.Spring)
                    {
                        _currentYear++;
                    }
                    
                    EmitSignal(SignalName.SeasonChanged, _currentSeason, _currentYear);
                }
            }
        }
    }
    
    // Getters for current time values
    public int GetCurrentHour() => _currentHour;
    public int GetCurrentDay() => _currentDay;
    public Season GetCurrentSeason() => _currentSeason;
    public int GetCurrentYear() => _currentYear;
    public float GetDayProgress() => (_currentHour * MINUTES_PER_HOUR + (float)_currentMinute) / (HOURS_PER_DAY * MINUTES_PER_HOUR);
    public double GetCurrentMinute() => _currentMinute;
    
    // Time-related utility functions
    public bool IsDaytime() => _currentHour >= 6 && _currentHour < 18;
    public bool IsNighttime() => !IsDaytime();
    public string GetTimeString() => $"{_currentHour:D2}:{_currentMinute:D2}";
    public string GetDateString() => $"Year {_currentYear}, {_currentSeason} {_currentDay}";
    
    // Time manipulation functions
    public void AdvanceHour()
    {
        _currentMinute = 0;
        _currentHour++;
        EmitSignal(SignalName.HourChanged, _currentHour);
        
        if (_currentHour >= HOURS_PER_DAY)
        {
            _currentHour = 0;
            AdvanceDay();
        }
    }
    
    public void AdvanceDay()
    {
        _currentDay++;
        EmitSignal(SignalName.DayChanged, _currentDay);
        
        if (_currentDay > DAYS_PER_SEASON)
        {
            _currentDay = 1;
            AdvanceSeason();
        }
    }
    
    private void AdvanceSeason()
    {
        _currentSeason = (Season)(((int)_currentSeason + 1) % SEASONS_PER_YEAR);
        
        if (_currentSeason == Season.Spring)
        {
            _currentYear++;
        }
        
        EmitSignal(SignalName.SeasonChanged, _currentSeason, _currentYear);
    }
    
    // Weather and season effects
    public float GetSeasonGrowthMultiplier()
    {
        return _currentSeason switch
        {
            Season.Spring => 1.2f,  // Best growing season
            Season.Summer => 1.0f,  // Normal growth
            Season.Fall => 0.8f,    // Slower growth
            Season.Winter => 0.5f,  // Slowest growth
            _ => 1.0f
        };
    }
    
    public float GetSeasonWaterRetentionMultiplier()
    {
        return _currentSeason switch
        {
            Season.Spring => 1.0f,  // Normal water retention
            Season.Summer => 0.7f,  // Water evaporates faster
            Season.Fall => 1.1f,    // Better water retention
            Season.Winter => 1.3f,  // Best water retention
            _ => 1.0f
        };
    }
    
    public void LoadSaveData(
        double currentMinute,
        int currentHour,
        int currentDay,
        Season currentSeason,
        int currentYear)
    {
        _currentMinute = currentMinute;
        _currentHour = currentHour;
        _currentDay = currentDay;
        _currentSeason = currentSeason;
        _currentYear = currentYear;
        
        // Emit signals to update any listeners
        EmitSignal(SignalName.HourChanged, _currentHour);
        EmitSignal(SignalName.DayChanged, _currentDay);
        EmitSignal(SignalName.SeasonChanged, _currentSeason, _currentYear);
    }
} 