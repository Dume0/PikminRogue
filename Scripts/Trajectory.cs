using Godot;
using System;
using System.Collections.Generic;


/*
   Acceleration :

      Vecteur égal au changement de vitesse, dù à une vitesse augmentant ou diminuant, où à un changement de direction.
      Pour la ballistique, est toujours égal à : (Acceleration.X = 0; Acceleration.Y = 9.8f).
      le X est à 0 car la vitesse du projectile est constante, et le Y est égal à la force gravitationnelle.

   Velocité (ou vecteur vitesse):

      Représente la vitesse et la direction d'un objet, le tout sous la forme d'un vecteur.


   La formule pour trouver l'angle pour atteindre une cible à 0, 1 ou 2 solutions.
   Les 2 solutions donnent un angle "optimal" (pour toucher la cible le plus rapidement possible)
   et un angle lobé.
*/
public static class Trajectory
{
   private static int splits = 3;
   static float pathLength = 0;

   public static float GetAngleToReachTarget(Vector2 target, float speed, float gravity)
   {
      float result = Mathf.Pow(speed, 4) - gravity * (gravity * Mathf.Pow(target.X, 2) + 2 * target.Y * Mathf.Pow(speed, 2));
      GD.Print(result);
      result = Mathf.Sqrt(result);
      result = Mathf.Atan((Mathf.Pow(speed, 2) - result) / (gravity * target.X));

      return result;
   }

   // Angle is in Radians
   public static Vector2 GetVelocity(float angle, float speed)
   {
      Vector2 velocity = new Vector2();

      velocity.X = speed * Mathf.Cos(angle);
      velocity.Y = speed * Mathf.Sin(angle);

      return velocity;
   }

   private static float GetDistance(float velocityX, float acceleration, float time)
   {
      return velocityX * time + 0.5f * acceleration * time * time;
   }
   private static float GetVelocity(float distance, float acceleration, float time)
   {
      return (distance - GetDistance(0, acceleration, time)) / time;
   }
   public static float GetMagnitudeToReachTarget(Vector2 target, float gravity, float angle)
   {
      float sin2Theta = Mathf.Sin(2 * Mathf.DegToRad(angle));
      float cosTheta = Mathf.Cos(Mathf.DegToRad(angle));
      float inner = (target.X * target.X * gravity) / (target.X * sin2Theta - 2 * target.Y * cosTheta * cosTheta);
      if (inner < 0)
      {
         return float.NaN;
      }
      float res = Mathf.Sqrt(inner);
      return res;
   }
   public static float GetAngleToReachTarget(Vector2 target, float gravity, float magnitude, bool smallAngle)
   {
      float innerSq = Mathf.Pow(magnitude, 4) - gravity * (gravity * target.X * target.X + 2 * target.Y * magnitude * magnitude);
      if (innerSq < 0)
      {
         return float.NaN;
      }
      float innerATan;
      if (smallAngle)
      {
         innerATan = (magnitude * magnitude - Mathf.Sqrt(innerSq)) / (gravity * target.X);
      }
      else
      {
         innerATan = (magnitude * magnitude + Mathf.Sqrt(innerSq)) / (gravity * target.X);
      }

      float res = Mathf.RadToDeg(Mathf.Atan(innerATan));
      return res;
   }

   public static List<Vector2> CalculateTrajectory(Vector2 velocity, Vector2 acceleration, float time)
   {
      List<Vector2> trajectoryPath = new List<Vector2>();
      pathLength = 0;

      float deltaTime = 0;
      Vector2 distance;
      for (int i = 0; i < splits; i++)
      {
         deltaTime = (time / (splits - 1)) * i;
         distance.X = GetDistance(velocity.X, acceleration.X, deltaTime);
         distance.Y = GetDistance(velocity.Y, acceleration.Y, deltaTime);
         trajectoryPath.Add(distance);
      }

      return trajectoryPath;
   }
   /*
      public Vector2 CalculateVelocity(Vector2 target, Vector2 acceleration, float time)
      {
         Vector2 velocity;
         Vector2 distance = (target - Position);

         velocity.X = GetVelocity(distance.X, acceleration.X, time);
         velocity.Y = GetVelocity(distance.Y, acceleration.Y, time);

         return velocity;
      }*/
}