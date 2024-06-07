using Godot;
using System;

public partial class Living : CharacterBody2D
{
   protected AnimatedSprite2D sprite;
   protected bool isFacingFront = true;

   [Export] public int healthMax;
   private int healthCurrent;

   public override void _Ready()
   {
      base._Ready();
      healthCurrent = healthMax;

      sprite = GetNode<AnimatedSprite2D>("Sprite2D");
   }

   public void TakeDamage(int amount)
   {
      healthCurrent -= amount;
      if (healthCurrent <= 0)
         Death();
   }

   private void Death()
   {

   }


   protected void PlayAnimation(string animationName)
   {
      if (isFacingFront)
         sprite.Play(animationName + "_front");
      else
         sprite.Play(animationName + "_back");
   }
}