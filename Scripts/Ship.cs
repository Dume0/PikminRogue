using Godot;
using System;

public partial class Ship : Node2D
{
   public static Ship instance;

   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton
   }
}