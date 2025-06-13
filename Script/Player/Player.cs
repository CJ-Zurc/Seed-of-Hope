using Godot;
using System;

public partial class Player : CharacterBody2D
{
    private AnimatedSprite2D anime;
    private Vector2 velocity = Vector2.Zero;
    private string lastDirection = "Down";
    private AnimationHandler animationHandler;

    public override void _Ready()
    {
        anime = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animationHandler = new SpriteAnimationHandler(anime);
        
    }

    public override void _PhysicsProcess(double delta)
    {
        int speed = 60;
        velocity = Vector2.Zero;

        // Movement input and direction tracking
        if (Input.IsKeyPressed(Key.W))
        {
            velocity.Y -= 1;
            lastDirection = "Up";
        }
        else if (Input.IsKeyPressed(Key.S))
        {
            velocity.Y += 1;
            lastDirection = "Down";
        }

        if (Input.IsKeyPressed(Key.A))
        {
            velocity.X -= 1;
            lastDirection = "Left";
        }
        else if (Input.IsKeyPressed(Key.D))
        {
            velocity.X += 1;
            lastDirection = "Right";
        }

        // Handle movement and animation
        if (velocity != Vector2.Zero)
        {
            velocity = velocity.Normalized() * speed;
            animationHandler.PlayWalk(lastDirection);
        }
        else
        {
            animationHandler.PlayIdle(lastDirection);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}

