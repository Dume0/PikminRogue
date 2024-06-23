using Godot;
using System;
using System.Linq;

public enum E_PikminState
{
	IDLE,
	FOLLOWING_CAPTAIN,
	MOVING_TO_ITEM,
	THROWED,
	FIGHTING,
	CARRYING,
	GRABED,
	PLUCKED
}

public enum E_Element
{
	NONE,
	FIRE,
	ELECTRICITY,
	WATER,
	POISON
}

public enum E_PikminGrowth
{
	LEAF,
	BURGEON,
	FLOWER
}

public enum E_PikminType
{
	NONE,
	RED,
	YELLOW,
	BLUE,
	VIOLET,
	WHITE
}

public abstract partial class Pikmin : Creature
{
	#region Components
	NavigationAgent2D navigationAgent;
	AudioStreamPlayer2D throwedAudioStream;
	AudioStreamPlayer2D attackAudioStream;
	GpuParticles2D dustParticles;
	GpuParticles2D throwParticles;
	Area2D actionArea;
	#endregion

	#region Enums
	[Export] private E_PikminState state; public E_PikminState State { get { return state; } private set { } }
	protected E_Element elementResistance = E_Element.NONE; public E_Element ElementResistance { get { return elementResistance; } private set { } }
	private E_PikminGrowth growth = E_PikminGrowth.LEAF; public E_PikminGrowth Growth { get { return growth; } private set { } }
	protected E_PikminType pikminType = E_PikminType.NONE; public E_PikminType PikminType { get { return pikminType; } private set { } }
	#endregion

	[Signal] public delegate void InflictDamageEventHandler();
	[Export] public float movementSpeed = 50.0f;

	private float verticalPosition = 0;

	#region Throw
	private Vector3 throwedVelocity;
	private float throwedGravity;
	private float throwedDistance;
	private Vector2 originThrowedPosition;
	#endregion

	private Item itemCarried;

	public override void _Ready()
	{
		base._Ready();

		navigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
		throwedAudioStream = GetNode<AudioStreamPlayer2D>("ThrowedAudioStreamPlayer2D");
		attackAudioStream = GetNode<AudioStreamPlayer2D>("AttackAudioStreamPlayer2D");
		dustParticles = GetNode<GpuParticles2D>("DustParticles2D");
		throwParticles = GetNode<GpuParticles2D>("Sprite2D/ThrowParticles2D");
		actionArea = GetNode<Area2D>("ActionArea2D");
		creatureArea.AddToGroup(Group.E_GroupToString(E_Group.PIKMIN));

		AddToGroup(E_Group.PIKMIN);

		PikminCount.instance.UpdatePikminCount();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		AnimationManager();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		switch (state)
		{
			case E_PikminState.IDLE:
				DoNextActionWhenIdling();
				break;
			case E_PikminState.FOLLOWING_CAPTAIN:
				Move(Player.instance.PikminFollowPoint.GlobalPosition);
				break;
			case E_PikminState.THROWED:
				ThrowUpdate();
				break;
			case E_PikminState.MOVING_TO_ITEM:
				Move(itemCarried.GlobalPosition);
				break;
			default:
				break;
		}
	}

	private void DoNextActionWhenIdling()
	{
		foreach (Object body in actionArea.GetOverlappingBodies())
		{
			// Item
			if (body.IsInGroup(E_Group.ITEM))
			{
				CarryItem((Item)body);
			}
			// Creature
			if (body.IsInGroup(E_Group.ENNEMY))
			{
				FightCreature((Ennemy)body);
			}
		}
	}

	private void Move(Vector2 target)
	{
		Vector2 velocity = Velocity;
		Vector2 direction = new Vector2();

		direction = Position.DirectionTo(navigationAgent.GetNextPathPosition());
		velocity = direction * movementSpeed;
		navigationAgent.SetVelocityForced(velocity);

		FlipSprite(direction);
		ApplyVelocity(velocity);

		navigationAgent.TargetPosition = target;
	}

	#region Throw
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

		// Deactivate collision
		ActivateCollision(false);
	}

	private void ThrowUpdate()
	{
		// Move
		Velocity = new Vector2(throwedVelocity.X, throwedVelocity.Z) * movementSpeed;
		MoveAndSlide();

		// Sprite
		sprite.Position = new Vector2(sprite.Position.X, sprite.Position.Y + throwedVelocity.Y);
		sprite.Scale = Vector2.One * Mathf.Clamp(-verticalPosition / 15, 1, 1.5f);
		shadowSprite.Scale = Vector2.One * Mathf.Clamp(verticalPosition / 15, 0.5f, 1);

		// Flip sprite
		if (throwedVelocity.X < 0) { sprite.FlipH = false; }
		else if (throwedVelocity.X > 0) { sprite.FlipH = true; }

		// Apply gravity to velocity
		throwedVelocity.Y += throwedGravity;
		verticalPosition += throwedVelocity.Y;

		// Find an ennemy
		if (GetCreatureInCollision() != null)
		{
			if (!GetCreatureInCollision().IsInGroup(E_Group.PIKMIN))
				FightCreature((Ennemy)GetCreatureInCollision());
		}

		// End condition
		if (sprite.Position.Y >= 0)
		{
			EndThrow();
		}
	}

	private void EndThrow()
	{
		// State
		state = E_PikminState.IDLE;

		// Position
		verticalPosition = 0;
		sprite.Position = Vector2.Zero;
		shadowSprite.Position = new Vector2(0, 6);

		// Particles
		dustParticles.Emitting = true;
		throwParticles.Emitting = false;

		// Reactivate collision
		ActivateCollision(true);
	}

	public override void FallToTheGround()
	{
		base.FallToTheGround();
	}
	#endregion

	#region Carry
	private void CarryItem(Item item)
	{
		if (!item.CanNewPikminCarryItem)
			return;

		// State
		state = E_PikminState.MOVING_TO_ITEM;

		itemCarried = item;
	}

	public void EndCarryItem()
	{
		if (state != E_PikminState.CARRYING || itemCarried == null)
			return;

		state = E_PikminState.IDLE;

		itemCarried.RemovePikminFromGroup(this);
		itemCarried = null;
	}
	#endregion

	#region Fight
	private void FightCreature(Ennemy creature)
	{
		if (state == E_PikminState.FIGHTING)
			return;

		state = E_PikminState.FIGHTING;

		GlobalPosition -= creature.GlobalPosition;
		Utils.SetParent(this, creature);

		InflictDamage += creature.OnDamageReceived;
	}
	#endregion
	public void FollowPlayer()
	{
		if (state == E_PikminState.FOLLOWING_CAPTAIN || state == E_PikminState.THROWED || state == E_PikminState.PLUCKED || IsInGroup(E_Group.PIKMIN_GRABED))
			return;

		EndCarryItem();

		AddToGroup(E_Group.PIKMIN_FOLLOWING_CAPTAIN);
		navigationAgent.TargetPosition = Player.instance.PikminFollowPoint.GlobalPosition;
		state = E_PikminState.FOLLOWING_CAPTAIN;
		PikminCount.instance.UpdatePikminCount();

	}

	private void EmitInflictDamage()
	{
		EmitSignal(SignalName.InflictDamage);
	}

	public void StopFollowPlayer()
	{
		if (state != E_PikminState.FOLLOWING_CAPTAIN)
			return;

		RemoveFromGroup(E_Group.PIKMIN_FOLLOWING_CAPTAIN);
		navigationAgent.TargetPosition = Vector2.Zero;
		state = E_PikminState.IDLE;
	}

	public void BeingGrabed()
	{
		if (state != E_PikminState.FOLLOWING_CAPTAIN)
			return;

		StopFollowPlayer();
		AddToGroup(E_Group.PIKMIN_GRABED);
		state = E_PikminState.GRABED;
	}

	public void EndFight()
	{
		state = E_PikminState.IDLE;
	}

	public void PlayPluckedAnimation()
	{
		state = E_PikminState.PLUCKED;
	}

	protected override void AnimationManager()
	{
		switch (state)
		{
			case E_PikminState.IDLE:
				animationPlayer.Play("idle");
				break;
			case E_PikminState.MOVING_TO_ITEM:
			case E_PikminState.CARRYING:
			case E_PikminState.GRABED:
			case E_PikminState.FOLLOWING_CAPTAIN:
				animationPlayer.Play("walk");
				break;
			case E_PikminState.THROWED:
				animationPlayer.Play("throwed");
				break;
			case E_PikminState.FIGHTING:
				animationPlayer.Play("fight");
				break;
			case E_PikminState.PLUCKED:
				animationPlayer.Play("plucked");
				break;
			default:
				GD.PushError("Pikmin AnimationManager : default case reached");
				break;
		}
	}

	#region Signals
	private void OnNavigationAgent2dTargetReached()
	{
		switch (state)
		{
			case E_PikminState.MOVING_TO_ITEM:
				state = E_PikminState.CARRYING;
				itemCarried.AddPikminToGroup(this);
				break;
			default:
				break;
		}
	}

	private void OnDestroy()
	{
		PikminCount.instance.UpdatePikminCount();
	}
	#endregion
}
