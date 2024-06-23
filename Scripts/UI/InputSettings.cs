using Godot;
using System;
using System.Collections.Generic;

public partial class InputSettings : PanelContainer
{
	#region Components
	private VBoxContainer actionList;
	#endregion

	private PackedScene inputButtonScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/input_button.tscn");


	public override void _Ready()
	{
		actionList = GetNode<VBoxContainer>("MarginContainer/ActionList");

		CreateActionList();
	}

	public override void _Process(double delta)
	{
	}

	private void CreateActionList()
	{
		InputMap.LoadFromProjectSettings();

		DeleteActionList();

		foreach (string action in InputMap.GetActions())
		{
			InitAction(action);
		}
	}

	private void DeleteActionList()
	{
		foreach (Node item in actionList.GetChildren())
		{
			item.QueueFree();
		}
	}

	private void InitAction(string action)
	{
		Button button = (Button)inputButtonScene.Instantiate();
		Label actionLabel = (Label)button.FindChild("ActionLabel");
		Label inputLabel = (Label)button.FindChild("InputLabel");
		actionLabel.Text = action;

		Godot.Collections.Array<InputEvent> events = InputMap.ActionGetEvents(action);
		inputLabel.Text = events.Count > 0 ? events[0].AsText() : "";

		actionList.AddChild(button);
	}
}
