using Godot;
using System;

public abstract partial class DialogueBase : Control
{
    protected Label questionLabel;
    protected Button yesButton;
    protected Button noButton;

    public override void _Ready()
    {
        questionLabel = GetNode<Label>("CanvasLayer/Control/questionSave");
        yesButton = GetNode<Button>("CanvasLayer/Control/yesButton");
        noButton = GetNode<Button>("CanvasLayer/Control/noButton");

        yesButton.Pressed += OnYesPressed;
        noButton.Pressed += OnNoPressed;
    }

    protected abstract void OnYesPressed();
    protected virtual void OnNoPressed()
    {
        QueueFree();
    }
}
