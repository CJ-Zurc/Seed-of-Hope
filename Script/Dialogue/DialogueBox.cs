using Godot;
using System;

public partial class DialogueBox : Control
{
    [Export] public NodePath questionLabelPath;
    [Export] public NodePath optionsContainerPath;
    [Export] public PackedScene optionButtonScene;

    private Label _questionLabel;
    private VBoxContainer _optionsContainer;
    private Action<int> _onOptionSelected;

    public override void _Ready()
    {
        _questionLabel = GetNode<Label>(questionLabelPath);
        _optionsContainer = GetNode<VBoxContainer>(optionsContainerPath);
    }

    public void ShowDialogue(string question, string[] options, Action<int> callback)
    {
        _onOptionSelected = callback;
        _questionLabel.Text = question;
        
        // Clear existing options
        foreach (Node child in _optionsContainer.GetChildren())
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
            _optionsContainer.AddChild(optionButton);
        }

        Show();
    }

    private void OnOptionButtonPressed(int optionIndex)
    {
        _onOptionSelected?.Invoke(optionIndex);
        QueueFree();
    }
} 
