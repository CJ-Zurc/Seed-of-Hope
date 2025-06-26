using Godot;

public partial class GameState : Node
{
	public static GameState Instance;
	public int SelectedSaveSlot = 0;
	public override void _EnterTree()
	{
		Instance = this;
	}
}
