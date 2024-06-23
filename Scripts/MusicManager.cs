using Godot;
using System;


public partial class MusicManager : Node2D
{
	public static MusicManager instance;

	#region Components
	private AudioStreamPlayer2D basicAudioStream;
	private AudioStreamPlayer2D nearEnnemyAudioStream;
	private AudioStreamPlayer2D combatAudioStream;
	private AudioStreamPlayer2D carryAudioStream;
	#endregion

	[Export] private bool mute;

	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

		basicAudioStream = GetNode<AudioStreamPlayer2D>("BasicAudioStreamPlayer2D");
		nearEnnemyAudioStream = GetNode<AudioStreamPlayer2D>("NearEnnemyAudioStreamPlayer2D");
		combatAudioStream = GetNode<AudioStreamPlayer2D>("CombatAudioStreamPlayer2D");
		carryAudioStream = GetNode<AudioStreamPlayer2D>("CarryAudioStreamPlayer2D");

		basicAudioStream.VolumeDb = 0;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (mute)
		{
			combatAudioStream.VolumeDb = -80;
			nearEnnemyAudioStream.VolumeDb = -80;
			basicAudioStream.VolumeDb = -80;
			carryAudioStream.VolumeDb = -80;
		}
		else
		{
			carryAudioStream.VolumeDb = -80;
			//	ActivateAudio(combatAudioStream, ArePikminsFighting());
			ActivateAudio(nearEnnemyAudioStream, IsThereAnEnnemyNearCaptain());
		}
	}

	private void ActivateAudio(AudioStreamPlayer2D audioStream, bool activate)
	{
		if (audioStream.Stream == null)
			return;

		if (activate && audioStream.VolumeDb < 0)
		{
			audioStream.VolumeDb++;
		}
		else if (!activate && audioStream.VolumeDb > -80)
		{
			audioStream.VolumeDb--;
		}
	}

	private bool IsThereAnEnnemyNearCaptain()
	{
		return Player.instance.NearEnnemyArea.GetOverlappingBodies().Count > 0;
	}

	private bool ArePikminsFighting()
	{
		foreach (Node2D node in GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN)))
		{
			Pikmin pikmin = (Pikmin)node;
			if (pikmin.State == E_PikminState.FIGHTING)
				return true;
		}
		return false;
	}

	private bool ArePikminsCarrying()
	{
		foreach (Node2D node in GetTree().GetNodesInGroup(Group.E_GroupToString(E_Group.PIKMIN)))
		{
			Pikmin pikmin = (Pikmin)node;
			if (pikmin.State == E_PikminState.CARRYING)
				return true;
		}
		return false;
	}
}
