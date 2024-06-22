using Godot;
using System;

public abstract partial class Object : CharacterBody2D
{
   #region Group
   public bool IsInGroup(E_Group group)
   {
      return IsInGroup(Group.E_GroupToString(group));
   }

   public void AddToGroup(E_Group group)
   {
      AddToGroup(Group.E_GroupToString(group));
   }

   public void RemoveFromGroup(E_Group group)
   {
      RemoveFromGroup(Group.E_GroupToString(group));
   }
   #endregion


   /// <summary>
   /// 
   /// </summary>
   /// <param name="direction"></param>
   /// <param name="speed"></param>
   /// <returns>Velocity</returns>
   protected Vector2 MoveToward(Vector2 direction, float speed, bool applyVelocity)
   {
      Vector2 velocity = this.Velocity;

      // Velocity
      velocity.X = direction.X != 0 ? direction.X * speed : Mathf.MoveToward(velocity.X, 0, speed);
      velocity.Y = direction.Y != 0 ? direction.Y * speed : Mathf.MoveToward(velocity.Y, 0, speed);

      if (applyVelocity) ApplyVelocity(velocity);
      return velocity;
   }
   protected Vector2 MoveToward(float speed, Vector2 target, bool applyVelocity)
   {
      Vector2 velocity = this.Velocity;
      Vector2 direction = new Vector2(target.X - GlobalPosition.X, target.Y - GlobalPosition.Y);
      direction.Normalized();

      // Velocity
      velocity.X = direction.X != 0 ? direction.X * speed : Mathf.MoveToward(velocity.X, 0, speed);
      velocity.Y = direction.Y != 0 ? direction.Y * speed : Mathf.MoveToward(velocity.Y, 0, speed);

      if (applyVelocity) ApplyVelocity(velocity);
      return velocity;
   }

   protected void ApplyVelocity(Vector2 velocity)
   {
      Velocity = velocity;
      MoveAndSlide();
   }

}