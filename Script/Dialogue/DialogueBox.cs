using Godot;
using System;

public partial class DialogueBox : Control
{
    [Export] public NodePath questionLabelPath;
    [Export] public NodePath optionsContainerPath;
    [Export] public PackedScene optionButtonScene;

    private Label questionLabel;
    private VBoxContainer optionsContainer;
    private Action<int> onOptionSelected;

    public override void _Ready()
    {
        questionLabel = GetNode<Label>(questionLabelPath);
        optionsContainer = GetNode<VBoxContainer>(optionsContainerPath);
    }

    public void ShowDialogue(string question, string[] options, Action<int> callback)
    {
        onOptionSelected = callback;
        questionLabel.Text = question;
        
        // Clear existing options
        foreach (Node child in optionsContainer.GetChildren())
        {
            child.QueueFree();
        }

        // Create new option buttons
        for (int i = 0; i < options.Length; i++)
        {
            int optionIndex = i; // Capture the index for the lambda
            Button optionButton = optionButtonScene.Instantiate<Button>();
            optionButton.Text = options[i];
            optionButton.Pressed += () => OnOptionButtonPressed(optionIndex);
            optionsContainer.AddChild(optionButton);
        }

        Show();
    }

    private void OnOptionButtonPressed(int optionIndex)
    {
        onOptionSelected?.Invoke(optionIndex);
        QueueFree();
    }
} 