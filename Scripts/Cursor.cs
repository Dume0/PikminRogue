using Godot;
using System;
using System.Linq;

public partial class Cursor : Sprite2D
{
	public static Cursor instance;

	#region Components
	private Area2D area2D;
	private CollisionShape2D collider;
	private CircleShape2D colliderShape;
	private Node2D whistleCircle;
	private Node2D[] whistlePoints;
	#endregion

	[Signal]
	public delegate void IsInWhistleRadiusEventHandler();

	private float whistleRotationSpeed = 1;

	private Vector2[] whistlePointsInitialPosition;
	private Vector2[] whistlePointsDirection;

	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); }// Singleton

		area2D = GetNode<Area2D>("Area2D");
		collider = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
		colliderShape = (CircleShape2D)collider.Shape;
		whistleCircle = GetNode<Node2D>("WhistleCircle");
		whistlePoints = new Node2D[whistleCircle.GetChildCount()];
		whistlePointsInitialPosition = new Vector2[whistleCircle.GetChildCount()];
		whistlePointsDirection = new Vector2[whistleCircle.GetChildCount()];
		for (int i = 0; i < whistleCircle.GetChildCount(); i++)
		{
			whistlePoints[i] = (Node2D)whistleCircle.GetChild(i);
			whistlePointsInitialPosition[i] = whistlePoints[i].Position;
		}
		whistlePointsDirection[0] = Vector2.Up;
		whistlePointsDirection[1] = Vector2.Up / 1.4f + Vector2.Right / 1.4f;
		whistlePointsDirection[2] = Vector2.Right;
		whistlePointsDirection[3] = Vector2.Right / 1.4f + Vector2.Down / 1.4f;
		whistlePointsDirection[4] = Vector2.Down;
		whistlePointsDirection[5] = Vector2.Down / 1.4f + Vector2.Left / 1.4f;
		whistlePointsDirection[6] = Vector2.Left;
		whistlePointsDirection[7] = Vector2.Left / 1.4f + Vector2.Up / 1.4f;
	}

	public override void _Process(double delta)
	{
		// Set Cursor position
		this.Position = GetGlobalMousePosition();

		// Set Collider radius equals to whistle radius
		colliderShape.Radius = Player.instance.WhistleRadius;

		// Turn Cursor
		Rotate(0.5f);
		foreach (Node2D item in whistlePoints)
		{
			item.Rotate(-0.5f);
		}

		if (Player.instance.IsWhistling)
			OnAreaStay();

		// Draw whistle
		if (Player.instance.isWhistling)
			Whistle();
		else
			EndWhistle();
	}

	private void Whistle()
	{
		whistleCircle.Visible = true;

		for (int i = 0; i < whistlePoints.Count(); i++)
		{
			if (Player.instance.WhistleRadius < Player.instance.WhistleMaxRadius)
				whistlePoints[i].Translate(whistlePointsDirection[i]);
		}
	}

	private void EndWhistle()
	{
		whistleCircle.Visible = false;

		for (int i = 0; i < whistlePoints.Count(); i++)
		{
			whistlePoints[i].Transform = new Transform2D(0, whistlePointsInitialPosition[i]);
		}
	}

	private void OnAreaStay()
	{
		foreach (Node2D body in area2D.GetOverlappingBodies())
		{
			// Pikmin
			if (body.IsInGroup("Pikmins"))
			{
				Pikmin pikmin = (Pikmin)body;
				pikmin.FollowPlayer();
			}
		}
	}
}
