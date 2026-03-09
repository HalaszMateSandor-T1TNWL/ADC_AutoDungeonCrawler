using Godot;
using System;

public partial class Enemy : Area2D
{
	public float CurrentHealth = 2.0f;
	public float MaxHealth = 100.0f;

	[Signal] public delegate void DamageEventHandler(float damage);
	
	public override void _Ready()
	{
		
	}

	public void OnDamageDealt(float damage)
	{
		CurrentHealth -= damage;
		EmitSignal(nameof(Damage), damage);
	}
}
