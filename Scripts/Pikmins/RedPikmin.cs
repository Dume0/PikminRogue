using Godot;
using System;
using System.Linq;

public partial class RedPikmin : Pikmin
{
   public override void _Ready()
   {
      base._Ready();

      elementResistance = E_Element.FIRE;
   }
}