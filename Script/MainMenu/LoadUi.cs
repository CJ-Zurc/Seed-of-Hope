using Godot;
using System;

public partial class LoadUi : Control
{
	private Button backButton;
	private Button[] loadButtons;
	private Button[] deleteButtons;
	private Label[] fileLabels;
	private Label[] infoLabels;
	private Label[] moneyLabels;
	private const int MaxSaveFiles = 3;
	private string[] saveFileNames = new string[]
	{
		"user://savegame1.json",
		"user://savegame2.json",
		"user://savegame3.json"
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackButtonPressed;

		loadButtons = new Button[MaxSaveFiles];
		deleteButtons = new Button[MaxSaveFiles];
		fileLabels = new Label[MaxSaveFiles];
		infoLabels = new Label[MaxSaveFiles];
		moneyLabels = new Label[MaxSaveFiles];

		for (int i = 0; i < MaxSaveFiles; i++)
		{
			loadButtons[i] = GetNodeOrNull<Button>($"LoadButton{(i == 0 ? "" : (i + 1).ToString())}");
			deleteButtons[i] = GetNodeOrNull<Button>($"DeleteSaveButton{(i == 0 ? "" : (i + 1).ToString())}");
			if (loadButtons[i] != null)
			{
				fileLabels[i] = loadButtons[i].GetNodeOrNull<Label>("Label");
				infoLabels[i] = loadButtons[i].GetNodeOrNull<Label>("Label2");
				moneyLabels[i] = loadButtons[i].GetNodeOrNull<Label>("Label3");
			}
		}

		for (int i = 0; i < MaxSaveFiles; i++)
		{
			int slot = i; // capture for lambda
			if (FileAccess.FileExists(saveFileNames[i]))
			{
				if (loadButtons[i] != null)
				{
					loadButtons[i].Visible = true;
					loadButtons[i].Disabled = false;
					loadButtons[i].Pressed += () => OnLoadSlotPressed(slot);
				}
				if (deleteButtons[i] != null)
				{
					deleteButtons[i].Visible = true;
					deleteButtons[i].Disabled = false;
					deleteButtons[i].Pressed += () => OnDeleteSlotPressed(slot);
				}
				// Optionally, update infoLabels[i] and moneyLabels[i] with save data summary
				UpdateSlotInfo(i);
			}
			else
			{
				if (loadButtons[i] != null)
				{
					loadButtons[i].Visible = false;
				}
				if (deleteButtons[i] != null)
				{
					deleteButtons[i].Visible = false;
				}
			}
		}
	}

	private void OnLoadSlotPressed(int slot)
	{
		MainGame.CurrentSaveSlot = slot + 1; // Set the slot before loading
		GD.Print($"Loading save slot {slot + 1}");
		GetTree().ChangeSceneToFile("res://scenes/main_game.tscn");
	}

	private void OnDeleteSlotPressed(int slot)
	{
		if (FileAccess.FileExists(saveFileNames[slot]))
		{
			DirAccess.RemoveAbsolute(saveFileNames[slot]);
			GD.Print($"Deleted save slot {slot + 1}");
			// Optionally, refresh the UI
			if (loadButtons[slot] != null) loadButtons[slot].Visible = false;
			if (deleteButtons[slot] != null) deleteButtons[slot].Visible = false;
		}
	}

	private void UpdateSlotInfo(int slot)
	{
		// Optionally, read the save file and update infoLabels[slot] and moneyLabels[slot]
		if (FileAccess.FileExists(saveFileNames[slot]))
		{
			using var file = FileAccess.Open(saveFileNames[slot], FileAccess.ModeFlags.Read);
			var json = file.GetAsText();
			var result = Json.ParseString(json);
			var saveData = result.AsGodotDictionary();
			if (saveData != null)
			{
				if (infoLabels[slot] != null && saveData.ContainsKey("year") && saveData.ContainsKey("day"))
				{
					int year = (int)saveData["year"];
					int day = (int)saveData["day"];
					infoLabels[slot].Text = $"Year {year}, Day {day}";
				}
				if (moneyLabels[slot] != null && saveData.ContainsKey("money"))
				{
					int money = (int)saveData["money"];
					moneyLabels[slot].Text = $"P{money:N0}";
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	private void OnBackButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/title_screen.tscn");
	}
}
