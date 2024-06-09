using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public partial class Item : CharacterBody2D
{
   // Weight
   [Export] private int weight = 1; public int Weight { get { return weight; } private set { } }
   public bool CanNewPikminCarryItem { get { return pikminCarrying.Count < weight * 2; } private set { } }

   // Text
   private Label weightLabel;

   // Pikmins
   private List<Pikmin> pikminCarrying = new List<Pikmin>();


   public override void _Ready()
   {
      base._Ready();

      // Create Label
      weightLabel = new Label();
      AddChild(weightLabel);
      weightLabel.Position = new Vector2(-6, -15);
      weightLabel.Scale = Vector2.One * 0.5f;
      weightLabel.ZIndex = 100;
      weightLabel.Visible = true;

      // Add to Group
      AddToGroup("Items");
   }

   public void AddPikminToGroup(Pikmin pikmin)
   {
      pikminCarrying.Add(pikmin);

      UpdateWeightLabel();
   }

   public void RemovePikminFromGroup(Pikmin pikmin)
   {
      pikminCarrying.Remove(pikmin);

      UpdateWeightLabel();
   }

   public void UpdateWeightLabel()
   {
      weightLabel.Text = pikminCarrying.Count + "/" + weight;
      weightLabel.Visible = pikminCarrying.Count > 0;
   }
}
