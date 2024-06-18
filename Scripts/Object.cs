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
}