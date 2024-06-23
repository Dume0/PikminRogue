using Godot;
using System;

public abstract partial class Ennemy : Creature
{


   public override void _Ready()
   {
      base._Ready();

      spiritScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ennemies/creature_spirit.tscn");

      AddToGroup(E_Group.ENNEMY);
   }

   protected override void Death()
   {
      base.Death();

      foreach (Node node in GetChildren())
      {
         if (node.IsInGroup(Group.E_GroupToString(E_Group.PIKMIN)))
         {
            Pikmin pikmin = (Pikmin)node;
            Utils.SetParent(pikmin, GetTree().Root.GetChild(0));
            pikmin.EndFight();
         }
      }

      deathAudioStream.Play();


   }

   public void OnDamageReceived()
   {
      TakeDamage(1);
   }
}