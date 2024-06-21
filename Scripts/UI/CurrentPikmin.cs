using Godot;
using System;
using System.Collections.Generic;

public partial class CurrentPikmin : HBoxContainer
{
	#region Components
	private TextureRect nextPikminLeft;
	private TextureRect currentPikmin;
	private TextureRect nextPikminRight;

	private Label currentPikminNb;
	#endregion

	public override void _Ready()
	{
		base._Ready();

		nextPikminLeft = GetNode<TextureRect>("MarginContainerLeft/AspectRatioContainerLeft/NextPikminLeft");
		currentPikmin = GetNode<TextureRect>("VBoxContainer/CurrentPikmin");
		nextPikminRight = GetNode<TextureRect>("MarginContainerRight/AspectRatioContainerRight/NextPikminRight");
		currentPikminNb = GetNode<Label>("VBoxContainer/Label");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);


	}


}
