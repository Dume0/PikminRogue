using Godot;
using System;

public abstract partial class Living : Object
{
   #region Components
   protected Sprite2D sprite;
   protected Sprite2D shadowSprite;
   protected CollisionShape2D shadowCollision;
   protected AnimationPlayer animationPlayer;
   private Node2D groundCheck;
   #endregion

   [Signal] public delegate void DeadEventHandler();

   [Export] protected bool isFlying;
   private const float GRAVITY = 0.5f;


   [Export] protected int healthMax;
   private int healthCurrent;

   public override void _Ready()
   {
      base._Ready();
      healthCurrent = healthMax;

      sprite = GetNode<Sprite2D>("Sprite2D");
      shadowSprite = GetNode<Sprite2D>("ShadowSprite2D");
      shadowCollision = GetNode<CollisionShape2D>("ShadowCollisionShape2D");
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
      groundCheck = GetNode<Node2D>("Sprite2D/GroundCheck");
   }

   public override void _PhysicsProcess(double delta)
   {
      base._PhysicsProcess(delta);

      if (!isFlying)
         ApplyGravity();
   }

   public void TakeDamage(int amount)
   {
      healthCurrent -= amount;
      if (healthCurrent <= 0)
         Death();
   }

   protected virtual void Death()
   {
      EmitSignal(SignalName.Dead);
   }


   public void ActivateCollision(bool activate)
   {
      shadowCollision.Disabled = !activate;
   }

   protected void FlipSprite(Vector2 direction)
   {
      if (direction.X < 0) { sprite.Scale = new Vector2(-1, 1); }
      else if (direction.X > 0) { sprite.Scale = Vector2.One; }
   }


   private void ApplyGravity()
   {
      if (IsGrounded())
         return;

      sprite.MoveLocalY(GRAVITY);
   }

   protected bool IsGrounded()
   {
      return groundCheck.Position.Y + sprite.Position.Y >= 0;
   }

   protected abstract void AnimationManager();

   /* 

     protected void PlayAnimation(string animationName)
     {
        if (isFacingFront)
           sprite.Play(animationName + "_front");
        else
           sprite.Play(animationName + "_back");
     }*/
}