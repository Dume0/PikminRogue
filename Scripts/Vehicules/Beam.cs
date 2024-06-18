using Godot;
using System;

public partial class Beam : Node2D
{
	#region Components
	Area2D area; public Vector2 ItemTarget { get { return area.Position + GlobalPosition; } private set { } }
	Node2D targetPoint;
	Node2D animatedPoint;
	AnimationPlayer animationPlayer;
	AudioStreamPlayer2D abductionAudioPlayer;
	#endregion

	[Signal]
	public delegate void ItemReceivedEventHandler(Item item);

	private Item towedItem;
	private bool isTowingItem;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		area = GetNode<Area2D>("Area2D");
		targetPoint = GetNode<Node2D>("TargetPoint");
		animatedPoint = GetNode<Node2D>("AnimatedPoint");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		abductionAudioPlayer = GetNode<AudioStreamPlayer2D>("AbductionAudioPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isTowingItem)
		{
			if (towedItem.HasEndTowed)
				EndTowItem();
		}
	}

	private void OnArea2DBodyEntered(Node2D body)
	{
		Object obj = (Object)body;
		if (obj.IsInGroup(E_Group.ITEM))
		{
			TowItem((Item)obj);
		}
	}

	public void TowItem(Item item)
	{
		towedItem = item;
		isTowingItem = true;

		item.canBeCarried = false;

		item.FreeAllPikmin();
		item.ActivateCollision(false);

		item.Position = area.Position + GlobalPosition;

		item.PlayTowedAnimation();

		abductionAudioPlayer.Play();
	}

	public void EndTowItem()
	{
		if (towedItem == null)
			return;

		EmitSignal(SignalName.ItemReceived, towedItem);

		towedItem = null;
		isTowingItem = false;
	}
}
