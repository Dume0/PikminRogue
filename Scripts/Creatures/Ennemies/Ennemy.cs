using Godot;
using System;

public abstract partial class Ennemy : Creature
{
   public override void _Ready()
   {
      base._Ready();

      AddToGroup(E_Group.ENNEMY);
   }
}