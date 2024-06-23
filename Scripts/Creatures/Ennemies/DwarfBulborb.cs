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
   private Area2D areaOfAction;
   private CircleShape2D areaOfActionShape;
   private Timer timer;
   #endregion

   private E_DwarfBulbordState state;

   [Export] private float movementSpeed = 10f;

   [Range(2, 8)] private double timeIdling;

   private PackedScene itemScene = ResourceLoader.Load<PackedScene>("res://Scenes/Ennemies/dwarf_bulborb_item.tscn");

   public override void _Ready()
   {
      base._Ready();

      navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
      areaOfAction = GetNode<Area2D>("AreaOfAction");
      areaOfActionShape = (CircleShape2D)GetNode<CollisionShape2D>("AreaOfAction/CollisionShape2D").Shape;
      timer = GetNode<Timer>("Timer");

      Idle();
   }

   public override void _Process(double delta)
   {
      base._Process(delta);

      switch (state)
      {
         case E_DwarfBulbordState.WALK:
            Move();
            break;
         default:
            break;
      }
      AnimationManager();
   }

   private void OnTimerTimeout()
   {
      navigationAgent.TargetPosition = GetRandomTargetWalkPosition();
      state = E_DwarfBulbordState.WALK;
   }

   private void OnNavigationAgentNavigationFinished()
   {
      Idle();
   }

   private void Idle()
   {
      state = E_DwarfBulbordState.IDLE;

      timeIdling = Utils.GetRandomNumber(2, 8);
      timer.Start(timeIdling);
   }

   private void Move()
   {
      Vector2 velocity = Velocity;
      Vector2 direction = new Vector2();

      direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
      velocity = direction * movementSpeed;
      navigationAgent.SetVelocityForced(velocity);

      FlipSprite(direction);
      ApplyVelocity(velocity);
   }

   private Vector2 GetRandomTargetWalkPosition()
   {
      return Utils.GetRandomPointInsideCircle(GlobalPosition, areaOfActionShape.Radius, areaOfActionShape.Radius / 2);
   }

   protected override void Death()
   {
      base.Death();

      Item item = itemScene.Instantiate() as Item;
      GetTree().Root.GetChild(0).AddChild(item);
      item.GlobalPosition = GlobalPosition;

      QueueFree();
   }

   protected override void AnimationManager()
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
