using Godot;
//Override Methods 
public class SpriteAnimationHandler : AnimationHandler
{
    public SpriteAnimationHandler(AnimatedSprite2D sprite) : base(sprite) { }

    public override void PlayWalk(string direction)
    {
        switch (direction)
        {
            case "Up":
                anime.Animation = "Walkingup";
                anime.FlipH = false;
                break;
            case "Down":
                anime.Animation = "WalkingDown";
                anime.FlipH = false;
                break;
            case "Left":
                anime.Animation = "WalkingLeft_Right";
                anime.FlipH = true;
                break;
            case "Right":
                anime.Animation = "WalkingLeft_Right";
                anime.FlipH = false;
                break;
        }
        anime.Play();
    }

    public override void PlayIdle(string direction)
    {
        switch (direction)
        {
            case "Up":
                anime.Animation = "IdleBack";
                anime.FlipH = false;
                break;
            case "Down":
                anime.Animation = "IdleFront";
                anime.FlipH = false;
                break;
            case "Left":
                anime.Animation = "IdleLeft_Right";
                anime.FlipH = true;
                break;
            case "Right":
                anime.Animation = "IdleLeft_Right";
                anime.FlipH = false;
                break;
        }
        anime.Play();
    }
}
