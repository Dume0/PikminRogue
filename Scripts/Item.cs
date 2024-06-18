using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Item : Object
{
   #region Components
   private NavigationAgent2D navigationAgent;
   private CollisionShape2D collision;
   private CircleShape2D shape;
   private AnimationPlayer animationPlayer;
   #endregion

   // Weight
   [Export] private int weight = 1; public int Weight { get { return weight; } private set { } }
   public int MaxWeight { get { return weight * 2; } private set { } }
   public bool CanNewPikminCarryItem { get { return pikminCarrying.Count < weight * 2; } private set { } }
   public bool IsBeingCarried { get { return pikminCarrying.Count >= weight; } private set { } }
   public bool canBeCarried = true;

   // Text
   private Label weightLabel;

   // Pikmins
   private List<Pikmin> pikminCarrying = new List<Pikmin>();

   [Export] private bool isPikminFood; public bool IsPikminFood { get { return isPikminFood; } private set { } }
   [Export] private float movementSpeed = 50f; public float MovementSpeed { get { return movementSpeed; } private set { } }
   [Export] private int value = 0; public int Value { get { return value; } private set { } }

   private List<Vector2> pikminHandlePoints = new List<Vector2>();
   private int pikminHandlePointsIndex = 0;

   [Export] public bool HasEndTowed;

   public override void _Ready()
   {
      base._Ready();

      // Components
      navigationAgent = new NavigationAgent2D();
      AddChild(navigationAgent);
      collision = GetNode<CollisionShape2D>("CollisionShape2D");
      shape = (CircleShape2D)collision.Shape;
      animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

      // Create Label
      weightLabel = new Label();
      AddChild(weightLabel);
      weightLabel.Position = new Vector2(-6, -15);
      weightLabel.Scale = Vector2.One * 0.5f;
      weightLabel.ZIndex = 100;
      weightLabel.Visible = true;

      // Add to Group
      AddToGroup(E_Group.ITEM);

      // Set Target of NavigationAgent
      navigationAgent.TargetPosition = isPikminFood ? Onion.instance.Beam.ItemTarget : Ship.instance.Beam.ItemTarget;

      // Set Handles
      SetPikminHandlePoints();
   }
   public override void _PhysicsProcess(double delta)
   {
      base._PhysicsProcess(delta);

      if (IsBeingCarried) MoveToBaseUpdate();
   }

   private void SetPikminHandlePoints()
   {
      Vector2 point;

      for (int i = 0; i < MaxWeight; i++)
      {
         point = new Vector2();
         point.X = shape.Radius * Mathf.Cos(2 * Mathf.Pi * i / MaxWeight);
         point.Y = shape.Radius * Mathf.Sin(2 * Mathf.Pi * i / MaxWeight);
         pikminHandlePoints.Add(point);
      }
   }

   private void MoveToBaseUpdate()
   {
      Vector2 velocity = Velocity;
      Vector2 direction = new Vector2();

      direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
      velocity = direction * (movementSpeed + (pikminCarrying.Count / weight * movementSpeed));
      navigationAgent.SetVelocityForced(velocity);

      Velocity = velocity;
      MoveAndSlide();
   }

   public void FreeAllPikmin()
   {
      for (int i = pikminCarrying.Count - 1; i >= 0; i--)
      {
         RemovePikminFromGroup(pikminCarrying[i]);
      }
   }

   public void AddPikminToGroup(Pikmin pikmin)
   {
      if (!canBeCarried)
         return;

      pikminCarrying.Add(pikmin);
      Utils.SetParent(pikmin, this);

      pikmin.ActivateCollision(false);

      pikmin.Position = pikminHandlePoints[pikminHandlePointsIndex];
      pikminHandlePointsIndex++;

      UpdateWeightLabel();
   }

   public void PlayTowedAnimation()
   {
      animationPlayer.Play("Towed");
   }

   public void RemovePikminFromGroup(Pikmin pikmin)
   {
      pikminCarrying.Remove(pikmin);
      Utils.SetParent(pikmin, GetTree().Root.GetChild(0));

      pikmin.ActivateCollision(true);

      pikminHandlePointsIndex--;
      pikmin.GlobalPosition = pikminHandlePoints[pikminHandlePointsIndex] + GlobalPosition;

      UpdateWeightLabel();
   }

   public void UpdateWeightLabel()
   {
      weightLabel.Text = pikminCarrying.Count + "/" + weight;
      weightLabel.Visible = pikminCarrying.Count > 0;
   }

   public void ActivateCollision(bool activate)
   {
      collision.Disabled = !activate;
   }
}
