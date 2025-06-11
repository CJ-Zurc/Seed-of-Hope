using Godot;
using System;

public partial class WeatherReport : Control
{
    private RichTextLabel dialogueLabel;
    private Button nextButton;

    private string[] dialogueLines = new string[]
	{
		"Welcome to Stardew TV!",
		"Welcome to your local news. I'm your host Bike Menriquez!",
		"I'm here to deliver a weather report. According to our Meteorologist Tide Roxas",
		"Tomorrow will be Sunny!, which is a great day to do laundry.",
		"Stay hydrated and don't forget to water your plants "
    };

    private int currentLineIndex = 0;

    public override void _Ready()
    {
        dialogueLabel = GetNode<RichTextLabel>("CanvasLayer/Control/weatherReport"); // Adjust path
        nextButton = GetNode<Button>("CanvasLayer/Control/next Button"); // Adjust path

        nextButton.Pressed += OnNextPressed;
        UpdateDialogue();
    }

    private void OnNextPressed()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            UpdateDialogue();
        }
        else
        {
            // All lines read; close or hide the scene
            QueueFree(); // or emit signal, etc.
        }
    }

    private void UpdateDialogue()
    {
        dialogueLabel.Text = dialogueLines[currentLineIndex];
    }
}