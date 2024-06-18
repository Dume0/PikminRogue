using Godot;
using System;

public partial class PikminCount : PanelContainer
{
	public static PikminCount instance;

	private Label pikminCountLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (instance == null) { instance = this; } // Singleton

		pikminCountLabel = GetNode<Label>("CenterContainer/Count");
		UpdatePikminCount();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdatePikminCount()
	{
		int pikminCount = GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN)).Count;
		int pikminFollowingCaptainCount = GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN_FOLLOWING_CAPTAIN)).Count;

		pikminCountLabel.Text = pikminFollowingCaptainCount + " / " + pikminCount;
	}
}
