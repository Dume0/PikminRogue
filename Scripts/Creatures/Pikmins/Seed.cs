using Godot;
using System;

public partial class Seed : Object
{
   #region Components
   private Sprite2D sprite;
   private Sprite2D shadowSprite;
   #endregion

   private PackedScene sproutScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/sprout.tscn");

   private float initialYVelocity = -15f;
   private Vector2 velocity;
   private Vector2 direction;
   private float speed = 0.5f;
   private const float GRAVITY = 0.5f;

   public Seed()
   {
      GenerateDirection();
      velocity = new Vector2(0, initialYVelocity);
   }

   public override void _Ready()
   {
      base._Ready();

      sprite = GetNode<Sprite2D>("Sprite2D");
      shadowSprite = GetNode<Sprite2D>("ShadowSprite");
   }

   public override void _Process(double delta)
   {
      base._Process(delta);

      if (!IsAirborn())
         Sprout();
   }

   public override void _PhysicsProcess(double delta)
   {
      base._PhysicsProcess(delta);

      Vector2 v = MoveToward(direction, speed, false);

      velocity = new Vector2(v.X, velocity.Y + GRAVITY);

      ApplyVelocity(velocity);
      ApplyGravity();

   }

   private void Sprout()
   {
      Sprout sprout = sproutScene.Instantiate() as Sprout;
      GetTree().Root.GetChild(0).AddChild(sprout);
      sprout.GlobalPosition = shadowSprite.GlobalPosition;

      QueueFree();
   }

   private void ApplyGravity()
   {
      if (!IsAirborn())
         return;

      sprite.MoveLocalY(GRAVITY);
   }

   private bool IsAirborn()
   {
      return sprite.Position.Y + sprite.Texture.GetHeight() <= shadowSprite.Position.Y;
   }

   public void GenerateDirection()
   {
      Random r = new Random();
      direction = (Vector2.Right * r.Next(0, 100)).Rotated((float)Utils.GetRandomNumber(0, Mathf.Pi));
   }

}
