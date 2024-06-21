using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public enum E_Layer
{
   PIKMIN,
   CAPTAIN,
   ENNEMY
}


public enum E_Group
{
   PIKMIN,
   PIKMIN_FOLLOWING_CAPTAIN,
   PIKMIN_GRABED,
   CREATURE,
   ITEM,
   SPROUT,
   ENNEMY
}

public static class Group
{
   public static string E_GroupToString(E_Group group)
   {
      switch (group)
      {
         case E_Group.PIKMIN:
            return "pikmin";
         case E_Group.PIKMIN_FOLLOWING_CAPTAIN:
            return "pikmin_following_captain";
         case E_Group.PIKMIN_GRABED:
            return "pikmin_grabed";
         case E_Group.CREATURE:
            return "creature";
         case E_Group.ITEM:
            return "item";
         case E_Group.SPROUT:
            return "sprout";
         case E_Group.ENNEMY:
            return "ennemy";
         default:
            return "default";
      }
   }
}