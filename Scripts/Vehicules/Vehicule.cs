using Godot;
using System;

public partial class Vehicule : Node2D
{
   #region Components
   protected Beam beam; public Beam Beam { get { return beam; } private set { } }
   protected AudioStreamPlayer2D audioPlayer;
   #endregion

   public override void _Ready()
   {
      base._Ready();

      beam = GetNode<Beam>("Beam");
      audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
   }
}