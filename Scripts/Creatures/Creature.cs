using Godot;
using System;

public abstract partial class Creature : Living
{
   #region Components
   protected Area2D shadowArea;
   protected Area2D creatureArea;
   protected AudioStreamPlayer2D deathAudioStream;
   #endregion

   protected PackedScene spiritScene;

   public override void _Ready()
   {
      base._Ready();

      shadowArea = GetNode<Area2D>("ShadowArea2D");
      creatureArea = GetNode<Area2D>("Sprite2D/CreatureArea2D");
      creatureArea.AddToGroup(Group.E_GroupToString(E_Group.CREATURE));
      deathAudioStream = GetNodeOrNull<AudioStreamPlayer2D>("DeathAudioStreamPlayer2D");

      AddToGroup(E_Group.CREATURE);
   }

   public Creature GetCreatureInCollision()
   {
      Creature creature = null;

      // CreatureArea
      foreach (Area2D area in creatureArea.GetOverlappingAreas())
      {
         if (area.IsInGroup(Group.E_GroupToString(E_Group.CREATURE)) && area != creatureArea)
            creature = (Creature)area.GetParent().GetParent();
      }
      if (creature == null)
         return null;

      // ShadowArea
      foreach (Area2D area in shadowArea.GetOverlappingAreas())
      {
         if (area == creature.creatureArea)
            return creature;
      }
      return null;
   }

   public virtual void FallToTheGround()
   {
      if (sprite.Position.Y >= 0)
         return;
   }

   protected override void Death()
   {
      base.Death();

      Sprite2D spirit = spiritScene.Instantiate() as Sprite2D;
      GetTree().Root.GetChild(0).AddChild(spirit);
      spirit.Position = GlobalPosition;
   }
}
