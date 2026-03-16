using Godot;
using System;

public partial class EnemyHealthBar : ProgressBar
{
	public Enemy enemy;
	public float CurrentHealth;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		enemy = new Enemy();
		enemy.HPChanged += OnEnemyHPChanged;
	}
	public void UpdateHealthBar()
	{
		this.Value = CurrentHealth;
		this.Visible = CurrentHealth > 0 && enemy.MaxHealth != CurrentHealth;
	}

	public void OnEnemyHPChanged(float currentHP)
	{
		CurrentHealth = currentHP;
		UpdateHealthBar();
	}

	

	//MILF : Man I Love Frogs
}
