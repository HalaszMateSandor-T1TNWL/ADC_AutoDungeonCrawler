using Godot;
using System;

public partial class Entity : CharacterBody2D
{

	[Signal] public delegate void HPChangedEventHandler(float currentHP);
	public float damage = 0.0f;
	public float maxHealth = 0.0f;
	public float CurrentHealth = 0.0f;
	public float movementSpeed = 100.0f;
	public float attackspeed = 1.0f;
	public int attackRange = 0;
	public Pathfinder pathfinding;

	public virtual void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			OnDeath();
		}
	}
	protected virtual void OnDeath()
	{
		QueueFree();
	}

	public virtual void OnQueueForFree(){}
}
