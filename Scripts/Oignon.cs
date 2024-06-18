using Godot;
using System;

public partial class Oignon : Node2D
{
   public static Oignon instance;

   #region Components
   private Area2D area;
   private Node2D sproutPoint;
   #endregion

   private PackedScene seedScene = ResourceLoader.Load<PackedScene>("res://Scenes/seed.tscn");
   private PackedScene sproutScene = ResourceLoader.Load<PackedScene>("res://Scenes/sprout.tscn");

   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

      area = GetNode<Area2D>("Area2D");
      sproutPoint = GetNode<Node2D>("SproutPoint");
   }

   private void OnArea2dBodyEntered(Node2D body)
   {
      Object obj = (Object)body;
      // Item
      if (obj.IsInGroup(E_Group.ITEM))
      {
         Item item = (Item)obj;
         if (!item.IsPikminFood)
            return;

         SproutSeed(item.Value);
         item.FreeAllPikmin();
         item.QueueFree();
      }
   }

   private void SproutSeed(int nb)
   {
      // Temp
      for (int i = 0; i < nb; i++)
      {
         Sprout sprout = sproutScene.Instantiate() as Sprout;
         AddChild(sprout);
         sprout.Position = new Vector2(sproutPoint.Position.X, sproutPoint.Position.Y + 20);
      }

      /*for (int i = 0; i < nb; i++)
      {
         Seed seed = seedScene.Instantiate() as Seed;
         AddChild(seed);
         seed.Position = sproutPoint.Position;
      }*/
   }
}