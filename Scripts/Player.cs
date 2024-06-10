using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Player : Living
{
	public static Player instance;

	#region Components
	private AudioStreamPlayer2D whistlingSound;
	private AudioStreamPlayer2D walkingSound;
	private RigidBody2D pikminFollowPoint; public RigidBody2D PikminFollowPoint { get { return pikminFollowPoint; } private set { } }
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

	#region States
	public bool isWalking = false; public bool IsWalking { get { return isWalking; } private set { } }
	public bool isWhistling = false; public bool IsWhistling { get { return isWhistling; } private set { } }
	public bool isGrabing = false; public bool IsGrabing { get { return isGrabing; } private set { } }
	#endregion
	private bool canPlayWalkingSound = true;

	private List<Pikmin> pikmins;

	[ExportGroup("Throw")]
	[Export] float throwSpeed = 10f;
	[Export] bool throwLobAngle = true;
	[Export] float throwGravity = 0.5f;
	[Export] bool debugDrawTrajectory = false;


	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

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

		if (debugDrawTrajectory) DrawThrowTrajectory();
		DrawLineCaptainCursor();
	}

	private void DrawThrowTrajectory()
	{
		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		Vector3 point = new Vector3(offsetCaptain.X, 0, offsetCaptain.Y);
		Color[] colors = { Colors.Blue, Colors.Red };

		float distance = offsetCaptain.DistanceTo(offsetCursor);

		Vector3 velocity;
		float angleVertical = Trajectory.GetVerticalAngle(throwSpeed, throwGravity, distance, !throwLobAngle);
		float angleHorizontal = Trajectory.GetAngleBetweenTwoPoints(offsetCaptain, offsetCursor);

		velocity = Trajectory.Get3DVelocity(angleVertical, angleHorizontal, throwSpeed);
		velocity.Y *= -1;

		point += velocity;

		for (var i = 0; i < 200; i++)
		{
			DrawCircle(new Vector2(point.X, point.Y + point.Z), point.Y / 10, colors[i % 2]);
			velocity.Y += throwGravity;
			point += velocity;
			if (new Vector2(point.X, point.Z).DistanceTo(offsetCaptain) > distance)
				break;
		}
	}

	private void DrawLineCaptainCursor()
	{
		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		Vector2 point;

		point = offsetCaptain;
		while (point.DistanceTo(offsetCaptain) < offsetCursor.DistanceTo(offsetCaptain))
		{
			point = point.MoveToward(offsetCursor, 5);
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
		if (direction.Y < 0) { isFacingFront = false; }
		else if (direction.Y > 0) { isFacingFront = true; }
		FlipSprite(direction);

		// Whistle
		if (Input.IsActionPressed("right_click")) { Whistle(); }
		else { EndWhistle(); }

		// Grab Pikmin
		if (Input.IsActionPressed("left_click")) { GrabPikmin(); }
		if (Input.IsActionJustReleased("left_click")) { ThrowPikmin(); }

		// Disband Pikmin
		if (Input.IsActionPressed("disband")) { DisbandAllPikmin(); }

	}

	private void Move()
	{
		Vector2 velocity = this.Velocity;

		// Velocity
		velocity.X = this.direction.X != 0 ? this.direction.X * this.movementSpeed : Mathf.MoveToward(velocity.X, 0, this.movementSpeed);
		velocity.Y = this.direction.Y != 0 ? this.direction.Y * this.movementSpeed : Mathf.MoveToward(velocity.Y, 0, this.movementSpeed);

		// Play Sound
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

		ApplyVelocity(velocity);
	}

	//////// Actions ///////////
	#region Whistle
	private void Whistle()
	{
		// Increased whistle radius
		if (whistleRadius < whistleMaxRadius) { whistleRadius += WHISTLE_SPEED; }

		// Play Sound
		whistlingSound.Play();

		isWhistling = true;
	}

	private void EndWhistle()
	{
		whistleRadius = WhistleMinRadius;

		whistlingSound.Playing = false;

		isWhistling = false;
	}
	#endregion

	#region Throw
	private void GrabPikmin()
	{
		if (grabedPikmin != null)
			return;

		// Itere sur tous les elements à portée de grab
		foreach (Node2D body in grabPikminArea.GetOverlappingBodies())
		{
			// Verifie si l'élément est un Pikmin
			if (!body.IsInGroup("PikminsFollowingCaptain"))
				continue;

			grabedPikmin = (Pikmin)body;

			grabedPikmin.StopFollowPlayer();
			grabedPikmin.AddToGroup("PikminGrabed");

			// Fait du capitaine le parent du Pikmin
			Utils.SetParent(grabedPikmin, grabPikminPoint);

			grabedPikmin.GlobalPosition = grabPikminPoint.GlobalPosition;

			isGrabing = true;
			break;
		}
	}

	// Utilisé lors de l'annulation d'un lancer
	private void ReleasePikmin()
	{
		if (grabedPikmin == null)
			return;

		grabedPikmin.FollowPlayer();

		Utils.SetParent(grabedPikmin, GetTree().Root.GetChild(0));

		isGrabing = false;
		grabedPikmin = null;
	}

	private void ThrowPikmin()
	{
		if (grabedPikmin == null)
			return;

		grabedPikmin.StopFollowPlayer();

		Utils.SetParent(grabedPikmin, GetTree().Root.GetChild(0));

		grabedPikmin.GlobalPosition = GlobalPosition;

		grabedPikmin.RemoveFromGroup("PikminGrabed");

		Vector2 offsetCaptain = Position - GlobalPosition;
		Vector2 offsetCursor = Cursor.instance.Position - GlobalPosition;
		float distance = offsetCaptain.DistanceTo(offsetCursor);
		float angleVertical = Trajectory.GetVerticalAngle(throwSpeed, throwGravity, distance, !throwLobAngle);
		float angleHorizontal = Trajectory.GetAngleBetweenTwoPoints(offsetCaptain, offsetCursor);
		Vector3 velocity = Trajectory.Get3DVelocity(angleVertical, angleHorizontal, throwSpeed);
		velocity.Y *= -1;
		grabedPikmin.Throwed(velocity, throwGravity, distance);

		isGrabing = false;
		grabedPikmin = null;

		control.instance.UpdatePikminCount();
	}
	#endregion

	private void DisbandAllPikmin()
	{
	}
	//////////////////////
	private void AnimationManager()
	{
		if (isWhistling && !isWalking)
			PlayAnimation("whistling");
		else if (isWhistling && isWalking)
			PlayAnimation("whistling_walk");
		else if (isWalking)
			PlayAnimation("walk");
		else if (!isWalking)
			PlayAnimation("idle");
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
