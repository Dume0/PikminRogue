using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Item : CharacterBody2D
{
   #region Components
   private NavigationAgent2D navigationAgent;
   private CollisionShape2D collision;
   private CircleShape2D shape;
   #endregion

   // Weight
   [Export] private int weight = 1; public int Weight { get { return weight; } private set { } }
   public int MaxWeight { get { return weight * 2; } private set { } }
   public bool CanNewPikminCarryItem { get { return pikminCarrying.Count < weight * 2; } private set { } }
   public bool IsBeingCarried { get { return pikminCarrying.Count >= weight; } private set { } }

   // Text
   private Label weightLabel;

   // Pikmins
   private List<Pikmin> pikminCarrying = new List<Pikmin>();

   [Export] private bool isPikminFood; public bool IsPikminFood { get { return isPikminFood; } private set { } }
   [Export] private float movementSpeed = 50f; public float MovementSpeed { get { return movementSpeed; } private set { } }

   private List<Vector2> pikminHandlePoints = new List<Vector2>();
   private int pikminHandlePointsIndex = 0;

   public override void _Ready()
   {
      base._Ready();

      // Components
      navigationAgent = new NavigationAgent2D();
      AddChild(navigationAgent);
      collision = GetNode<CollisionShape2D>("CollisionShape2D");
      shape = (CircleShape2D)collision.Shape;

      // Create Label
      weightLabel = new Label();
      AddChild(weightLabel);
      weightLabel.Position = new Vector2(-6, -15);
      weightLabel.Scale = Vector2.One * 0.5f;
      weightLabel.ZIndex = 100;
      weightLabel.Visible = true;

      // Add to Group
      AddToGroup("Items");

      // Set Target of NavigationAgent
      navigationAgent.TargetPosition = isPikminFood ? Oignon.instance.GlobalPosition : Ship.instance.GlobalPosition;

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
         GD.Print(point);
      }
   }

   private void MoveToBaseUpdate()
   {
      Vector2 velocity = Velocity;
      Vector2 direction = new Vector2();

      direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
      velocity = direction * movementSpeed;
      navigationAgent.SetVelocityForced(velocity);

      Velocity = velocity;
      MoveAndSlide();
   }

   public void AddPikminToGroup(Pikmin pikmin)
   {
      pikminCarrying.Add(pikmin);
      Utils.SetParent(pikmin, this);
      pikmin.Position = pikminHandlePoints[pikminHandlePointsIndex];
      pikminHandlePointsIndex++;

      UpdateWeightLabel();
   }

   public void RemovePikminFromGroup(Pikmin pikmin)
   {
      pikminCarrying.Remove(pikmin);
      Utils.SetParent(pikmin, GetTree().Root.GetChild(0));
      pikminHandlePointsIndex--;

      UpdateWeightLabel();
   }

   public void UpdateWeightLabel()
   {
      weightLabel.Text = pikminCarrying.Count + "/" + weight;
      weightLabel.Visible = pikminCarrying.Count > 0;
   }
}
