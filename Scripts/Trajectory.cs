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

   // Angle is in Radians
   public static float GetAngleBetweenTwoPoints(Vector2 origin, Vector2 target)
   {
      float result;

      result = Mathf.Atan2(target.Y - origin.Y, target.X - origin.Y);

      return result;
   }

   // Angle is in Radians
   public static float GetVerticalAngleToReachTarget(Vector2 target, float speed, float gravity)
   {
      float result = Mathf.Pow(speed, 4) - gravity * (gravity * Mathf.Pow(target.X, 2) + 2 * target.Y * Mathf.Pow(speed, 2));
      result = Mathf.Sqrt(result);
      result = Mathf.Atan((Mathf.Pow(speed, 2) - result) / (gravity * target.X));

      return result;
   }

   // Angle is in Radians
   public static float GetVerticalAngle(float speed, float gravity, float distance, bool getSmallAngle)
   {
      float result;

      result = (gravity * distance) / (speed * speed);
      result = Mathf.Asin(result);
      result = getSmallAngle ? 0.5f * result : Mathf.Pi / 2 - 0.5f * result;

      return result;
   }


   // Angle is in Radians
   public static Vector2 Get2DVelocity(float angle, float speed)
   {
      Vector2 velocity = new Vector2();

      velocity.X = speed * Mathf.Cos(angle);
      velocity.Y = speed * Mathf.Sin(angle);

      return velocity;
   }

   // Angle is in Radians
   public static Vector3 Get3DVelocity(float angleVertical, float angleHorizontal, float speed)
   {
      Vector3 velocity = new Vector3();

      velocity.X = speed * Mathf.Cos(angleVertical) * Mathf.Cos(angleHorizontal);
      velocity.Y = speed * Mathf.Sin(angleVertical);
      velocity.Z = speed * Mathf.Cos(angleVertical) * Mathf.Sin(angleHorizontal);

      return velocity;
   }
}