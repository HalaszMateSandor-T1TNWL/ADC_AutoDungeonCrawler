using Godot;
using System;

public partial class Enemy : Area2D
{
	private void OnBodyEntered(Node2D body)
	{
		QueueFree();
	}
}
