using Godot;

public abstract class AnimationHandler
{
    protected AnimatedSprite2D anime {get; set;}

    public AnimationHandler(AnimatedSprite2D sprite)
    {
        anime = sprite;
    }

    public abstract void PlayWalk(string direction);
    public abstract void PlayIdle(string direction);
}
