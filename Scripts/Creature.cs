using Godot;
using System;

public abstract partial class Creature : Living
{
   #region Components
   protected CollisionShape2D shadowCollider;
   protected Area2D shadowArea;
   protected Sprite2D shadowSprite;
   protected Area2D creatureArea;
   #endregion

   public override void _Ready()
   {
      base._Ready();

      shadowCollider = GetNode<CollisionShape2D>("ShadowCollision2D");
      shadowArea = GetNode<Area2D>("ShadowArea2D");
      shadowSprite = GetNode<Sprite2D>("ShadowSprite2D");
      creatureArea = GetNode<Area2D>("CreatureArea2D");

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
      foreach (Object body in creatureArea.GetOverlappingBodies())
      {
         if (body.IsInGroup(E_Group.CREATURE) && body != this)
            creature = (Creature)body;
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
}
