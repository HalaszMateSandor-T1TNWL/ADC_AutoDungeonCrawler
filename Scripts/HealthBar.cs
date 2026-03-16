using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	[Export] public float MaxHealth = 100.0f;
	public float CurrentHealth;
	public Area2D Current;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		Current = GetNode<Area2D>($"../../Enemy");
		
		CurrentHealth = new Enemy().CurrentHealth;
		
		this.MaxValue = MaxHealth;
		UpdateHealthBar();
	}

	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

		UpdateHealthBar();
	}

	public void Heal(float amount)
	{
		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

		UpdateHealthBar();
	}

	public void UpdateHealthBar()
	{
		this.Value = CurrentHealth;
		this.Visible = (CurrentHealth > 0 && CurrentHealth < MaxHealth);
	}

	public void OnDamageTaken(float damage)
	{
		TakeDamage(damage);	
	}

	//MILF : Man I Love Frogs
}
