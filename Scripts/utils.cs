using Godot;
using System;

public static class Utils
{
   public static void SetParent(Node obj, Node newParent)
   {
      obj.GetParent().RemoveChild(obj);
      newParent.AddChild(obj);
   }
}