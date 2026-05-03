using System.Collections;
using Godot;
using Godot.Collections;

public partial class Enemy : Entity
{
	Array<Node> targets = [];
	Overseer overseer;
	
	public override void _Ready()
	{
		AddToGroup("enemy");
		pathfinding = GetNodeOrNull<Pathfinder>($"Pathfinder");
		overseer = GetNode<Overseer>($"..");
		movementSpeed = 100.0f;
		maxHealth = 100.0f;
		CurrentHealth = maxHealth;
		damage = 1.0f;
	}

	public override void TakeDamage(float damage)
	{
		base.TakeDamage(damage);
		GD.Print($"HP: {CurrentHealth}");
		EmitSignal(nameof(HPChanged), CurrentHealth);
	}
	public override void _Process(double delta)
	{
		targets = overseer.eye.GetAllUnits();
		if(targets.Count > 0)
		{
			Node2D _target = (Node2D)targets[0];
			UnitNavigation.Instance.GetNextPosition(this, _target);
		}
	}

	public override void OnQueueForFree()
	{
		QueueFree();
	}

	protected override void OnDeath()
	{
		QueueFree();
}
}
