using Godot;
using System;

public partial class Ship : Node2D
{
   #region Components
   Beam beam; public Beam Beam { get { return beam; } private set { } }
   #endregion

   public static Ship instance;
   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

      beam = GetNode<Beam>("Beam");
   }

   private void OnBeamItemReceived(Item item)
   {
      item.QueueFree();
   }
}