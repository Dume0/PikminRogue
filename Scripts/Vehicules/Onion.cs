using Godot;
using System;

public partial class Onion : Node2D
{
   public static Onion instance;

   #region Components
   Beam beam; public Beam Beam { get { return beam; } private set { } }
   private Node2D sproutPoint;
   #endregion

   private PackedScene seedScene = ResourceLoader.Load<PackedScene>("res://Scenes/seed.tscn");
   private PackedScene sproutScene = ResourceLoader.Load<PackedScene>("res://Scenes/sprout.tscn");

   public override void _Ready()
   {
      base._Ready();
      if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

      beam = GetNode<Beam>("Beam");
      sproutPoint = GetNode<Node2D>("SproutPoint");
   }

   private void OnBeamItemReceived(Item item)
   {
      item.CallDeferred("QueueFree");

   }

   /*private void OnArea2dBodyEntered(Node2D body)
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
   }*/

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