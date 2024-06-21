using Godot;
using System;

public abstract partial class Living : Object
{
   [Signal] public delegate void DeadEventHandler();

   protected bool isFacingFront = true;

   [Export] public int healthMax;
   private int healthCurrent;

   public override void _Ready()
   {
      base._Ready();
      healthCurrent = healthMax;

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

   /* 

     protected void PlayAnimation(string animationName)
     {
        if (isFacingFront)
           sprite.Play(animationName + "_front");
        else
           sprite.Play(animationName + "_back");
     }*/
}