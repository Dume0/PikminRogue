using Godot;
using System;

public abstract partial class Creature : Living
{
   #region Components
   protected Sprite2D sprite;
   protected CollisionShape2D shadowCollider;
   protected Area2D shadowArea;
   protected Sprite2D shadowSprite;
   protected Area2D creatureArea;
   protected AudioStreamPlayer2D deathAudioStream;
   #endregion


   public override void _Ready()
   {
      base._Ready();

      sprite = GetNode<Sprite2D>("Sprite2D");
      shadowCollider = GetNode<CollisionShape2D>("ShadowCollision2D");
      shadowArea = GetNode<Area2D>("ShadowArea2D");
      shadowSprite = GetNode<Sprite2D>("ShadowSprite2D");
      creatureArea = GetNode<Area2D>("Sprite2D/CreatureArea2D");
      creatureArea.AddToGroup(Group.E_GroupToString(E_Group.CREATURE));
      deathAudioStream = GetNodeOrNull<AudioStreamPlayer2D>("DeathAudioStreamPlayer2D");

      AddToGroup(E_Group.CREATURE);
   }

   public void ActivateCollision(bool activate)
   {
      shadowCollider.Disabled = !activate;
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

}
