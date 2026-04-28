using Godot;
using System;

public partial class SeekerHealthBar : ProgressBar
{
	private Seeker _seeker;
	
	public override void _Ready()
	{	
		_seeker = GetNode<Seeker>($"..");
		MaxValue = _seeker.maxHealth;
		Value = _seeker.CurrentHealth;
		Visible = false;

		_seeker.HPChanged += OnHPChanged;
	}

	public void OnHPChanged(float currentHP)
	{
		GD.Print($"Health bar received: {currentHP}");
		Value = currentHP;
		Visible = (currentHP > 0 && currentHP < _seeker.maxHealth);
	}
}
