using Godot;
using System;

public partial class Sprout : Object
{
	#region Components
	private AudioStreamPlayer2D audioStream;
	#endregion

	private PackedScene pikminScene = ResourceLoader.Load<PackedScene>("res://Scenes/red_pikmin.tscn");

	public override void _Ready()
	{
		base._Ready();

		audioStream = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");

		AddToGroup(E_Group.SPROUT);
	}

	public void Pluck()
	{
		Pikmin pikmin = pikminScene.Instantiate() as Pikmin;
		GetTree().Root.GetChild(0).AddChild(pikmin);
		pikmin.GlobalPosition = GlobalPosition;

		audioStream.Play();

		QueueFree();
	}
}
