using Godot;
using System;

public abstract partial class Ennemy : Creature
{
   private PackedScene spiritScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ennemies/creature_spirit.tscn");


   public override void _Ready()
   {
      base._Ready();

      AddToGroup(E_Group.ENNEMY);
   }

   protected override void Death()
   {
      base.Death();

      foreach (Node node in GetChildren())
      {
         if (node.IsInGroup(Group.E_GroupToString(E_Group.PIKMIN)))
         {
            Utils.SetParent(node, GetTree().Root.GetChild(0));
         }
      }

      deathAudioStream.Play();

      Sprite2D spirit = spiritScene.Instantiate() as Sprite2D;
      GetTree().Root.GetChild(0).AddChild(spirit);
      spirit.Position = GlobalPosition;
   }

   public void OnDamageReceived()
   {
      TakeDamage(1);
   }
}