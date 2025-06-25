using Godot;
using System;

public partial class LoadUi : Control
{
	private Button backButton;
	private Button[] loadButtons = new Button[3];
	private Button[] deleteButtons = new Button[3];
	private string[] saveFiles = { "user://savegame1.json", "user://savegame2.json", "user://savegame3.json" };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		backButton = GetNode<Button>("BackButton");
		backButton.Pressed += OnBackButtonPressed;

		loadButtons[0] = GetNode<Button>("LoadButton1");
		loadButtons[1] = GetNode<Button>("LoadButton2");
		loadButtons[2] = GetNode<Button>("LoadButton3");
		deleteButtons[0] = GetNode<Button>("DeleteSaveButton1");
		deleteButtons[1] = GetNode<Button>("DeleteSaveButton2");
		deleteButtons[2] = GetNode<Button>("DeleteSaveButton3");

		UpdateSlots();
	}

	private void UpdateSlots()
	{
		bool[] exists = new bool[3];
		for (int i = 0; i < 3; i++)
		{
			exists[i] = FileAccess.FileExists(saveFiles[i]);
		}

		for (int i = 0; i < 3; i++)
		{
			int slot = i;
			loadButtons[i].Visible = exists[i];
			deleteButtons[i].Visible = exists[i];
			loadButtons[i].Disabled = !exists[i];
			loadButtons[i].Text = $"Load File {i+1}";

			// Disconnect previous signals to avoid stacking
			loadButtons[i].Pressed -= null;
			deleteButtons[i].Pressed -= null;

			if (exists[i])
			{
				loadButtons[i].Pressed += () => OnLoadButtonPressed(slot);
				deleteButtons[i].Pressed += () => OnDeleteSaveButtonPressed(slot);
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

	private void OnLoadButtonPressed(int slot)
	{
		GameState.Instance.SelectedSaveSlot = slot;
		GetTree().ChangeSceneToFile("res://scenes/main_game.tscn");
	}

	private void OnDeleteSaveButtonPressed(int slot)
	{
		string savePath = saveFiles[slot];
		if (FileAccess.FileExists(savePath))
		{
			DirAccess.RemoveAbsolute(savePath);
		}
		UpdateSlots();
	}
}
