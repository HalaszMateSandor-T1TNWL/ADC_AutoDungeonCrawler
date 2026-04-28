using Godot;
using System;

public partial class EnemyHealthBar : ProgressBar
{
	private Enemy _enemy;

	public override void _Ready()
	{	
		_enemy = GetNode<Enemy>($"..");
		MaxValue = _enemy.maxHealth;
		Value = _enemy.CurrentHealth;
		Visible = false;

		_enemy.HPChanged += OnEnemyHPChanged;
	}

	public void OnEnemyHPChanged(float currentHP)
	{
		GD.Print($"Health bar received: {currentHP}");
		Value = currentHP;
		Visible = (currentHP > 0 && currentHP < _enemy.maxHealth);
	}
	//MILF : Man I Love Frogs
}
