using Godot;
using System;

public partial class DayMeter : PanelContainer
{
	#region Components
	private TextureRect sunTexture;
	private Control spacer;
	#endregion

	public override void _Ready()
	{
		sunTexture = GetNode<TextureRect>("MarginContainer/AspectRatioContainer2/Sun");
		spacer = GetNode<Control>("MarginContainer/Spacer");
	}

	public override void _Process(double delta)
	{
		spacer.CustomMinimumSize = new Vector2(spacer.CustomMinimumSize.X + (float)(0.2 * delta), 1);
	}
}
