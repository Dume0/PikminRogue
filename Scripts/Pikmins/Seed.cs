using Godot;
using System;

public partial class Seed : Object
{
   #region Components
   private Sprite2D sprite;
   private Sprite2D shadowSprite;
   #endregion

   private float initialYVelocity = -10f;
   private Vector2 velocity;
   private Vector2 direction;
   private float speed = 1f;

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

      Vector2 v = MoveToward(direction, speed, false);

      velocity = new Vector2(v.X, velocity.Y + 0.5f);
      ApplyVelocity(velocity);
   }

   public void GenerateDirection()
   {
      Random r = new Random();
      direction = (Vector2.Right * r.Next(0, 100)).Rotated((float)Utils.GetRandomNumber(0, Mathf.Pi));
   }

}
