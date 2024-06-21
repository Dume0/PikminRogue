using Godot;
using System;

public partial class Onion : Vehicule
{
   public static Onion instance;

   #region Components
   private Node2D sproutPoint;
   private GpuParticles2D sproutParticles;
   #endregion

   private PackedScene seedScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/seed.tscn");
   private PackedScene sproutScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/sprout.tscn");

   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

      sproutPoint = GetNode<Node2D>("SproutPoint");
      sproutParticles = GetNode<GpuParticles2D>("SproutParticles2D");
   }

   private void OnBeamItemReceived(Item item)
   {
      SproutSeed(item.Value);

      item.QueueFree();
   }

   private void SproutSeed(int nb)
   {
      audioPlayer.Play();
      sproutParticles.Emitting = true;

      for (int i = 0; i < nb; i++)
      {
         Seed seed = seedScene.Instantiate() as Seed;
         AddChild(seed);
         seed.Position = sproutPoint.Position;
      }
   }
}