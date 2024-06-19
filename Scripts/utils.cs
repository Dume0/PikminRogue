using Godot;
using System;

public static class Utils
{
   public static void SetParent(Node obj, Node newParent)
   {
      obj.GetParent().RemoveChild(obj);
      newParent.AddChild(obj);
   }

   public static double GetRandomNumber(double minimum, double maximum)
   {
      Random random = new Random();
      return random.NextDouble() * (maximum - minimum) + minimum;
   }

}