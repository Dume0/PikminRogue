using Godot;
using System;


public partial class MusicManager : Node2D
{
	public static MusicManager instance;

	#region Components
	private AudioStreamPlayer2D basicAudioStream;
	private AudioStreamPlayer2D nearEnnemyAudioStream;
	private AudioStreamPlayer2D combatAudioStream;
	#endregion

	[Export] private bool mute;

	public override void _Ready()
	{
		base._Ready();
		if (instance == null) { instance = this; } else { GD.PrintErr("Two or more instance of this object are presents"); } // Singleton

		basicAudioStream = GetNode<AudioStreamPlayer2D>("BasicAudioStreamPlayer2D");
		nearEnnemyAudioStream = GetNode<AudioStreamPlayer2D>("NearEnnemyAudioStreamPlayer2D");
		combatAudioStream = GetNode<AudioStreamPlayer2D>("CombatAudioStreamPlayer2D");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (mute)
		{
			combatAudioStream.VolumeDb = -80;
			nearEnnemyAudioStream.VolumeDb = -80;
			basicAudioStream.VolumeDb = -80;
		}

		combatAudioStream.VolumeDb =
		nearEnnemyAudioStream.VolumeDb = IsThereAnEnnemyNearCaptain() ? 0 : -80;
		basicAudioStream.VolumeDb = !IsThereAnEnnemyNearCaptain() ? 0 : -80;
	}

	private bool IsThereAnEnnemyNearCaptain()
	{
		return Player.instance.NearEnnemyArea.GetOverlappingBodies().Count > 0;
	}
	/*
		private bool ArePikminsFighting()
		{

		}*/
}
