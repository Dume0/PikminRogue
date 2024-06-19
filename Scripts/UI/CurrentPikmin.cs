using Godot;
using System;

public partial class CurrentPikmin : HBoxContainer
{
	#region Components
	private TextureRect nextPikminLeft;
	private TextureRect currentPikmin;
	private TextureRect nextPikminRight;
	#endregion

	public override void _Ready()
	{
		base._Ready();

		nextPikminLeft = GetNode<TextureRect>("NextPikminLeft");
		currentPikmin = GetNode<TextureRect>("CurrentPikmin");
		nextPikminRight = GetNode<TextureRect>("NextPikminRight");
	}
}
