using Godot;
using System;

public partial class Living : CharacterBody2D
{
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

   private void Death()
   {

   }
}