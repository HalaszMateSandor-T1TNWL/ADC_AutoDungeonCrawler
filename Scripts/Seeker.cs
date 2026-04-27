using Godot;
using Godot.Collections;
using System.IO;
using System.Linq;

public partial class Seeker : Entity
{
	[Export] public new float movementSpeed = 100.0f;
	//[Export] public new int attackRange = 0;
	[Export] public new float maxHealth = 100.0f;
	[Export] public new float CurrentHealth = 100.0f;
	[Export] public new float damage = 2.0f;

	[Signal] public delegate void DealDamageEventHandler(float damage);
	
	
	public override void _Ready()
	{
		AddToGroup("player");
		pathfinding = GetNodeOrNull<Pathfinder>($"Pathfinder");
	}

	public override void OnQueueForFree()
	{
		this.QueueFree();
	}

	public void OnBodyEntered(Node2D body)
	{
		if(body.IsInGroup("enemy"))
		{
			GD.Print(body.GlobalPosition);
		}
	}
}
