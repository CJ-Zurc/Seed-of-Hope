using Godot;
using System;

public partial class Volume : HSlider
{
    [Export]
    public string BusName = "Master";
    private int BusIndex;

    public override void _Ready()
    {
        BusIndex = AudioServer.GetBusIndex(BusName);
        this.ValueChanged += OnValueChanged;

        // Get current volume in dB and convert to linear for slider
        float currentDB = AudioServer.GetBusVolumeDb(BusIndex);
        float linear = Mathf.DbToLinear(currentDB);

        // Clamp between min and max just in case
        this.Value = Mathf.Clamp(linear, (float)this.MinValue, (float)this.MaxValue);
    }

    private void OnValueChanged(double value)
    {
        float linear = (float)value / 100f; // Assuming the slider is set to 0-100 range
        AudioServer.SetBusVolumeDb(BusIndex, Mathf.LinearToDb(linear));
    }
}