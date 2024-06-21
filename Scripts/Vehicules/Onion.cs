using Godot;
using System;

public partial class Onion : Node2D
{
   public static Onion instance;

   #region Components
   private Beam beam; public Beam Beam { get { return beam; } private set { } }
   private Node2D sproutPoint;
   private AudioStreamPlayer2D audioPlayer;
   private GpuParticles2D sproutParticles;
   #endregion

   private PackedScene seedScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/seed.tscn");
   private PackedScene sproutScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/sprout.tscn");

   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

      beam = GetNode<Beam>("Beam");
      sproutPoint = GetNode<Node2D>("SproutPoint");
      audioPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
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