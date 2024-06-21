using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public enum E_DwarfBulbordState
{
   IDLE,
   WALK
}

public partial class DwarfBulborb : Ennemy
{
   #region Components
   private NavigationAgent2D navigationAgent;
   private AnimationPlayer animationPlayer;
   private Area2D areaOfAction;
   #endregion

   private E_DwarfBulbordState state = E_DwarfBulbordState.IDLE;

   [Range(2, 8)] private double timeIdling;

   private PackedScene itemScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ennemies/dwarf_bulborb_item.tscn");

   public override void _Ready()
   {
      base._Ready();

      navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
      areaOfAction = GetNode<Area2D>("AreaOfAction");
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

      timeIdling = Utils.GetRandomNumber(2, 8);
   }

   public override void _Process(double delta)
   {
      base._Process(delta);
      // AnimationManager();
   }

   protected override void Death()
   {
      base.Death();

      Item item = itemScene.Instantiate() as Item;
      GetTree().Root.GetChild(0).AddChild(item);
      item.GlobalPosition = GlobalPosition;

      QueueFree();
   }

   private void AnimationManager()
   {
      switch (state)
      {
         case E_DwarfBulbordState.IDLE:
            animationPlayer.Play("idle");
            break;
         case E_DwarfBulbordState.WALK:
            animationPlayer.Play("walk");
            break;
         default:
            GD.PrintErr("default case reached");
            break;
      }
   }
}
