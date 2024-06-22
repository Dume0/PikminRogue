using Godot;
using System;

public partial class Wall : Living
{
	#region Components
	private Sprite2D sprite;
	#endregion

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

}
