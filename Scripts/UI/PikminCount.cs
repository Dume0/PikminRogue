using Godot;
using System;

public partial class PikminCount : PanelContainer
{
	public static PikminCount instance;

	private Label pikminCountLabel;

	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

		pikminCountLabel = GetNode<Label>("CenterContainer/Count");
		UpdatePikminCount();
	}


	public void UpdatePikminCount()
	{
		int pikminCount = GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN)).Count / 2;
		int pikminFollowingCaptainCount = GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN_FOLLOWING_CAPTAIN)).Count;

		pikminCountLabel.Text = pikminFollowingCaptainCount + " / " + pikminCount;
	}
}
