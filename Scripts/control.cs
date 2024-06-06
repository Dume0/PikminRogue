using Godot;
using System;

public partial class control : Control
{
	public static control instance;

	private Label pikminCountLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (instance == null) { instance = this; } // Singleton

		pikminCountLabel = GetNode<Label>("PikminCount");
		UpdatePikminCount();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdatePikminCount()
	{
		int pikminCount = GetTree().GetNodesInGroup("Pikmins").Count;
		int pikminFollowingCaptainCount = GetTree().GetNodesInGroup("PikminsFollowingCaptain").Count;

		pikminCountLabel.Text = pikminFollowingCaptainCount + " / " + pikminCount;
	}
}
