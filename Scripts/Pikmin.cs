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
	GpuParticles2D throwParticles;
	#endregion

	private E_PikminState state; public E_PikminState State { get { return state; } private set { } }

	[Export] public float movementSpeed = 100.0f;
	[Export] public float throwForce = 100.0f;

	private float verticalPosition = 0;
	private float traveledThrowDistance = 1;

	// Throw
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
		throwParticles = sprite.GetNode<GpuParticles2D>("ThrowParticles2D");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

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

		// Particle
		throwParticles.Emitting = true;

		// Attributs
		originThrowedPosition = GlobalPosition;
		throwedVelocity = velocity;
		throwedGravity = gravity;
		throwedDistance = distance;
	}

	private void ThrowUpdate()
	{
		// Move
		Velocity = new Vector2(throwedVelocity.X, throwedVelocity.Z) * movementSpeed;
		MoveAndSlide();

		// Sprite
		sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + throwedVelocity.Y);
		sprite.Scale = Vector2.One * Mathf.Clamp(-verticalPosition / 15, 1, 1.5f);

		// Flip sprite
		if (throwedVelocity.X < 0) { sprite.FlipH = false; }
		else if (throwedVelocity.X > 0) { sprite.FlipH = true; }

		// Apply gravity to velocity
		throwedVelocity.Y += throwedGravity;
		verticalPosition += throwedVelocity.Y;

		// End condition
		if (sprite.Position.Y >= 0)
		{
			EndThrow();
		}
	}

	private void EndThrow()
	{
		state = E_PikminState.IDLE;
		verticalPosition = 0;
		sprite.Position = Vector2.Zero;
		shadowSprite.Position = new Vector2(0, 6);
		dustParticles.Emitting = true;
		throwParticles.Emitting = false;
	}

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
