using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Living
{
	public static Player instance;

	#region Components
	private AnimatedSprite2D sprite;
	private AudioStreamPlayer2D whistlingSound;
	private AudioStreamPlayer2D walkingSound;
	public RigidBody2D pikminFollowPoint; public RigidBody2D PikminFollowPoint { get { return pikminFollowPoint; } private set { } }
	private Area2D grabPikminArea;
	private Node2D grabPikminPoint;
	#endregion

	private Vector2 direction = new Vector2();

	[ExportGroup("Whistle")]
	private float whistleRadius = 12; public float WhistleRadius { get { return whistleRadius; } private set { } }
	private const float WHISTLE_SPEED = 1.2f; public float WhistleSpeed { get { return WHISTLE_SPEED; } private set { } }
	[Export] private float whistleMaxRadius = 50; public float WhistleMaxRadius { get { return whistleMaxRadius; } private set { } }
	[Export] private float whistleMinRadius = 12; public float WhistleMinRadius { get { return whistleMinRadius; } private set { } }

	[Export] public float movementSpeed = 100.0f;

	private Pikmin grabedPikmin;
	[Export] private Node2D pikminParent;

	#region States
	public bool isWalking = false; public bool IsWalking { get { return isWalking; } private set { } }
	public bool isWhistling = false; public bool IsWhistling { get { return isWhistling; } private set { } }
	public bool isGrabing = false; public bool IsGrabing { get { return isGrabing; } private set { } }
	#endregion
	private bool canPlayWalkingSound = true;

	private List<Pikmin> pikmins;

	[ExportGroup("Throw")]
	[Export] float throwLaunchMagnitude = 1f;
	[Export] float throwLaunchAngle = 45f;
	[Export] bool throwLobAngle = false;
	[Export] Vector2 throwVelocity = Vector2.Right;
	[Export] Vector2 throwAcceleration = Vector2.Down;


	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } // Singleton

		sprite = GetNode<AnimatedSprite2D>("Sprite2D");
		whistlingSound = GetNode<AudioStreamPlayer2D>("WhistlingSound");
		walkingSound = GetNode<AudioStreamPlayer2D>("WalkingSound");
		pikminFollowPoint = GetNode<RigidBody2D>("PikminFollowPoint");
		grabPikminArea = GetNode<Area2D>("GrabPikminArea2D");
		grabPikminPoint = GetNode<Node2D>("GrabPikminPoint");

		whistleRadius = whistleMinRadius;
	}

	public override void _Process(double delta)
	{
		ReadInput();
		AnimationManager();
		QueueRedraw();
	}

	public override void _PhysicsProcess(double delta)
	{
		Move();
	}

	public override void _Draw()
	{
		base._Draw();

		DrawTrajectory();
		DrawLineCaptainCursor();
	}


	private void DrawTrajectory()
	{
		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		Vector2 point = offsetCaptain;
		Color[] colors = { Colors.Blue, Colors.Red };
		float gravity = -throwAcceleration.Y;
		List<Vector2> path;

		throwVelocity.X = Mathf.Cos(Mathf.DegToRad(throwLaunchAngle)) * throwLaunchMagnitude;
		throwVelocity.Y = Mathf.Sin(Mathf.DegToRad(throwLaunchAngle)) * throwLaunchMagnitude;

		path = Trajectory.CalculateTrajectory(throwVelocity, throwAcceleration, 1);
		for (int i = 0; i < path.Count; i++)
		{
			DrawCircle(path[i], 1, colors[i % 2]);
		}
		//Trajectory.CalculateTrajectory()

		/*
		float step = 1 / (2 * Mathf.Pi);
		float y = 0;
		float offset = 1;
		float b = 1;

		for (var i = 0; i < 40; i++)
		{
			DrawCircle(new Vector2(i, Mathf.Sin(y) * 10), 2, Colors.Violet);
			GD.Print(y);
			y += step;
		}*/

		/*
		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		Vector2 direction;
		float throwForce = -70;
		float gravity = 20;
		float speed = 10;

		Vector2 groundedPoint = Position;
		Vector2 totalPoint;
		float aerialY = 0;

		Color[] colors = { Colors.Blue, Colors.Red };
		float distance;

		// Direction
		Position.DirectionTo(Cursor.instance.Position - GlobalPosition);
		distance = Position.DistanceTo(Cursor.instance.Position - GlobalPosition);
		//throwForce = distance / 2;
		speed = distance / 100;

		//float velocity = -distance / 2;
		float velocity = -distance / 2;
		GD.Print("velocity " + velocity);
		GD.Print("distance " + distance);

		int i = 0;
		float j = -throwForce - distance / 2;

		while (groundedPoint.DistanceTo(offsetCaptain) < offsetCursor.DistanceTo(offsetCaptain))
		{
			groundedPoint = groundedPoint.MoveToward(Cursor.instance.Position - GlobalPosition, speed);
			aerialY = aerialY + (velocity);
			totalPoint = new Vector2(groundedPoint.X, groundedPoint.Y + aerialY);
			//point.Y = Mathf.Sin(i) * 10;
			DrawCircle(totalPoint, 2, colors[i % 2]);
			//gravity++;
			velocity += speed;
			i++;
		}
		GD.Print("iteration " + i);*/
	}

	private void DrawLineCaptainCursor()
	{
		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		Vector2 point;

		point = offsetCaptain;
		while (point.DistanceTo(offsetCaptain) < offsetCursor.DistanceTo(offsetCaptain))
		{
			point = point.MoveToward(offsetCursor, 3);
			DrawCircle(point, 1, new Color(255, 255, 255, 0.2f));
		}
	}

	private void ReadInput()
	{
		// Get inputs
		direction = Input.GetVector("left", "right", "up", "down");

		// Set is_walking
		if (direction != Vector2.Zero) { isWalking = true; }
		else { isWalking = false; }

		// Flip sprite
		if (direction.X < 0) { sprite.FlipH = false; }
		else if (direction.X > 0) { sprite.FlipH = true; }

		// Whistle
		if (Input.IsActionPressed("right_click")) { Whistle(); }
		else { EndWhistle(); }

		// Grab Pikmin
		if (Input.IsActionPressed("left_click")) { GrabPikmin(); }
		if (Input.IsActionJustReleased("left_click")) { ThrowPikmin(); }
	}

	private void Move()
	{
		Vector2 velocity = this.Velocity;

		velocity.X = this.direction.X != 0 ? this.direction.X * this.movementSpeed : Mathf.MoveToward(velocity.X, 0, this.movementSpeed);
		velocity.Y = this.direction.Y != 0 ? this.direction.Y * this.movementSpeed : Mathf.MoveToward(velocity.Y, 0, this.movementSpeed);

		// Play 
		if (velocity != Vector2.Zero && canPlayWalkingSound)
		{
			walkingSound.Play();
			canPlayWalkingSound = false;
		}
		if (!isWalking)
		{
			walkingSound.Playing = false;
			canPlayWalkingSound = true;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	//////// Actions ///////////
	private void Whistle()
	{
		// Increased whistle radius
		if (whistleRadius < whistleMaxRadius) { whistleRadius += WHISTLE_SPEED; }

		whistlingSound.Play();

		isWhistling = true;
	}

	private void EndWhistle()
	{
		whistleRadius = WhistleMinRadius;

		whistlingSound.Playing = false;

		isWhistling = false;
	}

	private void GrabPikmin()
	{
		if (grabedPikmin != null)
			return;

		foreach (Node2D body in grabPikminArea.GetOverlappingBodies())
		{
			// Pikmin
			if (body.IsInGroup("PikminsFollowingCaptain") && grabedPikmin == null)
			{
				Pikmin pikmin = (Pikmin)body;

				pikmin.StopFollowPlayer();
				pikmin.AddToGroup("PikminGrabed");

				pikmin.GetParent().RemoveChild(pikmin);
				AddChild(pikmin);

				pikmin.Position = grabPikminPoint.Position;

				isGrabing = true;
				grabedPikmin = pikmin;
			}
		}
	}

	private void ReleasePikmin()
	{
		if (grabedPikmin == null)
			return;

		grabedPikmin.FollowPlayer();

		grabedPikmin.GetParent().RemoveChild(grabedPikmin);
		pikminParent.AddChild(grabedPikmin);

		isGrabing = false;
		grabedPikmin = null;
	}

	private void ThrowPikmin()
	{
		if (grabedPikmin == null)
			return;

		grabedPikmin.StopFollowPlayer();

		grabedPikmin.GetParent().RemoveChild(grabedPikmin);
		pikminParent.AddChild(grabedPikmin);

		grabedPikmin.RemoveFromGroup("PikminGrabed");

		grabedPikmin.ThrowAtPosition(GetGlobalMousePosition());

		isGrabing = false;
		grabedPikmin = null;

		control.instance.UpdatePikminCount();
	}
	//////////////////////
	private void AnimationManager()
	{
		if (isWhistling && !isWalking)
			sprite.Play("whistling");
		else if (isWhistling && isWalking)
			sprite.Play("whistling_walk");
		else if (isWalking)
			sprite.Play("walk");
		else if (!isWalking)
			sprite.Play("idle");
	}

	public void AddPikminToGroup(Pikmin pikmin)
	{
		pikmins.Add(pikmin);
	}

	private void OnWalkingSoundFinished()
	{
		canPlayWalkingSound = true;
	}

}
