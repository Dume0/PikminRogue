using Godot;
using System;

public partial class Sprout : Object
{
	#region Components
	private AudioStreamPlayer2D audioStream;
	private Area2D area;
	private AnimationPlayer animationPlayer;
	#endregion

	private PackedScene pikminScene = ResourceLoader.Load<PackedScene>("res://Scenes/Pikmins/red_pikmin.tscn");

	public override void _Ready()
	{
		base._Ready();

		audioStream = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		area = GetNode<Area2D>("Area2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		AddToGroup(E_Group.SPROUT);
		area.AddToGroup(Group.E_GroupToString(E_Group.SPROUT));
	}

	public void SpawnPikmin()
	{
		Pikmin pikmin = pikminScene.Instantiate() as Pikmin;
		GetTree().Root.GetChild(0).AddChild(pikmin);
		pikmin.GlobalPosition = GlobalPosition;
	}

	public void Pluck()
	{
		animationPlayer.Play("pull");
	}
}
