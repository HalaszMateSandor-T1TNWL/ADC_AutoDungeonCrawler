using Godot;
using Godot.Collections;
using System.IO;
using System.Linq;

public partial class Seeker : Entity
{

	
	
	public override void _Ready()
	{
		AddToGroup("player");
		pathfinding = GetNodeOrNull<Pathfinder>($"Pathfinder");
		movementSpeed = 100.0f;
		maxHealth = 100.0f;
		CurrentHealth = maxHealth;
		damage = 5.0f;
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		GD.Print($"HP: {CurrentHealth}");
		EmitSignal(nameof(HPChanged), CurrentHealth);
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
