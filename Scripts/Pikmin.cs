using Godot;
using System;
using System.Linq;

public enum E_PikminState
{
	IDLE,
	FOLLOWING,
	THROWED
}

public partial class Pikmin : Living
{
	#region Components
	Sprite2D shadowSprite;
	CollisionShape2D collider;
	NavigationAgent2D navigationAgent;
	AudioStreamPlayer2D throwedAudioStream;
	GpuParticles2D dustParticles;
	#endregion

	private E_PikminState state; public E_PikminState State { get { return state; } private set { } }

	[Export] public float movementSpeed = 100.0f;
	[Export] public float throwForce = 100.0f;

	private float verticalPosition = 0;
	private float distanceThrowedPositonTargetPosition;
	private float traveledThrowDistance = 1;
	private bool hasReachedMidwayPoint;

	private Vector3 throwedVelocity;
	private float throwedGravity;
	private float throwedDistance;
	private Vector2 originThrowedPosition;

	public override void _Ready()
	{
		base._Ready();

		shadowSprite = GetNode<Sprite2D>("ShadowSprite2D");
		collider = GetNode<CollisionShape2D>("CollisionShape2D");
		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		throwedAudioStream = GetNode<AudioStreamPlayer2D>("ThrowedAudioStreamPlayer2D");
		dustParticles = GetNode<GpuParticles2D>("DustParticles2D");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		/*if (state == E_PikminState.THROWED)
		{
			// Move sprite
			if (!hasReachedMidwayPoint)
			{
				sprite.GlobalPosition = sprite.GlobalPosition.MoveToward(midwayThrowedPosition, (float)delta * movementSpeed);
				if (sprite.GlobalPosition.DistanceTo(midwayThrowedPosition) <= 10f)
					hasReachedMidwayPoint = true;
			}
			else
			{
				GD.Print("b");
				sprite.GlobalPosition = sprite.GlobalPosition.MoveToward(throwedPosition, (float)delta * movementSpeed);
			}
			shadowSprite.GlobalPosition = new Vector2(sprite.GlobalPosition.X, shadowSprite.GlobalPosition.Y);
		}
		*/
		AnimationManager();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (state == E_PikminState.FOLLOWING) Move();
		if (state == E_PikminState.THROWED) ThrowUpdate();
	}

	private void Move()
	{
		Vector2 velocity = Velocity;
		Vector2 direction;

		direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
		velocity = direction * movementSpeed;
		navigationAgent.SetVelocityForced(velocity);

		// Flip sprite
		if (direction.X < 0) { sprite.FlipH = false; }
		else if (direction.X > 0) { sprite.FlipH = true; }

		Velocity = velocity;
		MoveAndSlide();

		navigationAgent.TargetPosition = Player.instance.PikminFollowPoint.GlobalPosition;
	}

	public void Throwed(Vector3 velocity, float gravity, float distance)
	{
		if (state == E_PikminState.THROWED)
			return;

		// State
		state = E_PikminState.THROWED;

		// Audio
		throwedAudioStream.Play();

		// Attributs
		originThrowedPosition = GlobalPosition;
		throwedVelocity = velocity;
		throwedGravity = gravity;
		throwedDistance = distance;
	}

	public void ThrowUpdate()
	{
		// Move
		Velocity = new Vector2(throwedVelocity.X, throwedVelocity.Z) * movementSpeed;
		MoveAndSlide();

		// Sprite
		sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + throwedVelocity.Y);
		sprite.Scale = Vector2.One * Mathf.Clamp(-verticalPosition / 15, 1, 1.5f);

		// Apply gravity to velocity
		throwedVelocity.Y += throwedGravity;
		verticalPosition += throwedVelocity.Y;
		if (sprite.Position.Y >= 0)
		{
			state = E_PikminState.IDLE;
			verticalPosition = 0;
			sprite.Position = Vector2.Zero;
			shadowSprite.Position = new Vector2(0, 6);
		}
	}
	/*
	public void ThrowAtPosition(Vector2 position)
	{
		state = E_PikminState.THROWED;

		if (throwedPosition == Vector2.Inf)
		{
			throwedPosition = position;
			throwedAudioStream.Play();

			// Set midwayThrowedPosition
			midwayThrowedPosition = throwedPosition / 2 + Position / 2;
			midwayThrowedPosition.Y -= throwForce - Mathf.Abs(throwedPosition.Y - Position.Y);
			hasReachedMidwayPoint = false;

			distanceThrowedPositonTargetPosition = sprite.GlobalPosition.DistanceTo(throwedPosition);
		}

		navigationAgent.TargetPosition = throwedPosition;

		Vector2 velocity = Velocity;
		Vector2 direction;

		direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
		velocity = GlobalPosition.Slerp(throwedPosition, 0.5f);
		navigationAgent.SetVelocityForced(velocity);

		// Flip sprite
		if (direction.X < 0) { sprite.FlipH = false; }
		else if (direction.X > 0) { sprite.FlipH = true; }

		Velocity = velocity;
		MoveAndSlide();

		// End throw
		if (sprite.GlobalPosition.DistanceTo(throwedPosition) <= 1f && hasReachedMidwayPoint)
		{
			state = E_PikminState.IDLE;
			throwedPosition = Vector2.Inf;
			dustParticles.Emitting = true;
		}
	}*/

	public void FollowPlayer()
	{
		if (state == E_PikminState.FOLLOWING || state == E_PikminState.THROWED || IsInGroup("PikminGrabed"))
			return;

		AddToGroup("PikminsFollowingCaptain");
		navigationAgent.TargetPosition = Player.instance.PikminFollowPoint.GlobalPosition;
		state = E_PikminState.FOLLOWING;
		control.instance.UpdatePikminCount();
	}

	public void StopFollowPlayer()
	{
		if (state != E_PikminState.FOLLOWING)
			return;

		RemoveFromGroup("PikminsFollowingCaptain");
		navigationAgent.TargetPosition = Vector2.Zero;
		state = E_PikminState.IDLE;
	}

	private void AnimationManager()
	{
		switch (state)
		{
			case E_PikminState.IDLE:
				sprite.Play("idle");
				break;
			case E_PikminState.FOLLOWING:
				sprite.Play("idle");
				break;
			case E_PikminState.THROWED:
				sprite.Play("throwed");
				break;
			default:
				GD.PushError("Pikmin AnimationManager : default case reached");
				break;
		}
	}
}
