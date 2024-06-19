using Godot;
using System;

public partial class DwarfBulborb : Creature
{
   private PackedScene itemScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ennemies/dwarf_bulborb_item.tscn");

   protected override void Death()
   {
      base.Death();

      Item item = itemScene.Instantiate() as Item;
      GetTree().Root.GetChild(0).AddChild(item);
      item.GlobalPosition = GlobalPosition;

      QueueFree();
   }
}
