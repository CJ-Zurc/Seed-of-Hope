using Godot;
using System;

public partial class Volume : HSlider
{
	[Export]
	public string BusName = "Master";
	private int BusIndex;
	private bool isInitializing = false;

	[Signal]
	public delegate void VolumeChangedEventHandler(float newVolume);

	public override void _Ready()
	{
		BusIndex = AudioServer.GetBusIndex(BusName);
		this.ValueChanged += OnValueChanged;

		float db = 0f; // Default
		if (FileAccess.FileExists("user://savegame.json"))
		{
			using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			var saveData = result.As<Godot.Collections.Dictionary>();
			if (saveData != null && saveData.ContainsKey("volume"))
			{
				db = (float)saveData["volume"];
			}
			else
			{
				db = AudioServer.GetBusVolumeDb(BusIndex);
			}
		}
		else
		{
			db = AudioServer.GetBusVolumeDb(BusIndex);
		}

		float minDb = -40f;
		float maxDb = 0f;
		float linear = Mathf.InverseLerp(minDb, maxDb, db);
		float sliderValue = Mathf.Clamp(linear * 100f, (float)this.MinValue, (float)this.MaxValue);

		isInitializing = true;
		this.Value = sliderValue;
		isInitializing = false;
	}

	private void OnValueChanged(double value)
	{
		if (isInitializing)
			return;

		float minDb = -40f;
		float maxDb = 0f;
		float linear = (float)value / 100f;
		float db = Mathf.Lerp(minDb, maxDb, linear);

		GD.Print($"Volume.cs: Slider changed to {value}, setting dB to {db}");
		AudioServer.SetBusVolumeDb(BusIndex, db);

		// Save dB value directly to JSON
		var saveData = new Godot.Collections.Dictionary();
		if (FileAccess.FileExists("user://savegame.json"))
		{
			using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			saveData = result.As<Godot.Collections.Dictionary>() ?? new Godot.Collections.Dictionary();
		}
		saveData["volume"] = db; // Save as dB
		string newJson = Json.Stringify(saveData);
		using var writeFile = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
		writeFile.StoreString(newJson);
		GD.Print($"Volume.cs: Saved dB value {db} to JSON");

		EmitSignal(nameof(VolumeChanged), db);
	}

	public void Mute()
	{
		GD.Print("Mute called");
		GD.Print($"Setting bus {BusName} (index {BusIndex}) to -40 dB");
		float minDb = -40f; // -40 dB for mute
		AudioServer.SetBusVolumeDb(BusIndex, minDb);

		// Update slider to position corresponding to -40 dB
		float maxDb = 0f;
		float linear = Mathf.InverseLerp(minDb, maxDb, minDb);
		this.Value = Mathf.Clamp(linear * 100f, (float)this.MinValue, (float)this.MaxValue);

		// Save dB value directly to JSON
		var saveData = new Godot.Collections.Dictionary();
		if (FileAccess.FileExists("user://savegame.json"))
		{
			using var file = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			saveData = result.As<Godot.Collections.Dictionary>() ?? new Godot.Collections.Dictionary();
		}
		saveData["volume"] = minDb; // Save as dB
		string newJson = Json.Stringify(saveData);
		using var writeFile = FileAccess.Open("user://savegame.json", FileAccess.ModeFlags.Write);
		writeFile.StoreString(newJson);
	}
}
