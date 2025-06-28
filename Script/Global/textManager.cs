using Godot;
using System;

public partial class textManager : Node
{
    private Label textPopUp;
    private TextureRect textbg;
    private Timer textDuration;

    public static textManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        // Don't get nodes here!
    }

    private void EnsureInitialized()
    {
        if (textPopUp == null)
        {
            textPopUp = GetNode<Label>("/root/MainGame/text/textPopUp");
            textbg = GetNode<TextureRect>("/root/MainGame/text/textbackground");
            textDuration = GetNode<Timer>("/root/MainGame/text/textDuration");
            textDuration.Timeout += OnPopupHideTimerTimeout;
            textPopUp.Visible = textbg.Visible = false;
        }
    }

    public void showPopup(string message, float duration = 1.5f)
    {
        EnsureInitialized(); // Make sure nodes are found (or re-found if the scene is reloaded)
        textPopUp.Text = message;
        textPopUp.Visible = true;
        textbg.Visible = true;
        textDuration.WaitTime = duration;
        textDuration.Stop();
        textDuration.Start();
    }

    private void OnPopupHideTimerTimeout()
    {
        if (textPopUp != null && textbg != null)
        {
            textPopUp.Visible = false;
            textbg.Visible = false;
        }
    }
}