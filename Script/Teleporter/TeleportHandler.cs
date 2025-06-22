using Godot;
public abstract class TeleportHandler
{
    protected CharacterBody2D Player {get;set;}

    public TeleportHandler(CharacterBody2D player)
    {
        Player = player;
    }

    public abstract void TeleportTo(int x, int y);
}

public class CharacterTeleport : TeleportHandler
{
    public CharacterTeleport(CharacterBody2D player) : base(player) {}

    public override void TeleportTo(int x, int y)
    {
        Player.Position = new Vector2(x, y);    
    }
}

