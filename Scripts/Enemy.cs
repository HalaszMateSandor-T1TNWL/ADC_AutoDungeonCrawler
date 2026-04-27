using Godot;
using Godot.Collections;

public partial class Enemy : Entity
{
	[Signal] public delegate void HPChangedEventHandler(float currentHP);

	Array<Node> targets = [];
	Overseer overseer;

	public float MaxHealth = 100.0f;
	
	public override void _Ready()
	{
		AddToGroup("enemy");
		pathfinding = GetNodeOrNull<Pathfinder>($"Pathfinder");
		overseer = GetNode<Overseer>($"..");
		HPChange(5);
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

	public void HPChange(float change)
	{
		CurrentHealth = MaxHealth;
		if(change < 0)
		{
			DamageTaken(change);
		}else if(change >= 0)
		{
			Heal(change);
		}
		EmitSignal(nameof(HPChanged), CurrentHealth);
	}
	
	public void DamageTaken(float damage)
	{
		// TakeDamage(damage);
		if(CurrentHealth <= 0)
		{
			QueueFree();
		}
	}

	public void Heal(float amount)
	{
		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
	}
}
