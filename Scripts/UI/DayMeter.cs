using Godot;
using System;

public partial class DayMeter : PanelContainer
{
	#region Components
	private TextureRect sunTexture;
	#endregion

	public override void _Ready()
	{
		sunTexture = GetNode<TextureRect>("AspectRatioContainer2/Sun");

		
	}

	public override void _Process(double delta)
	{
	}
}
