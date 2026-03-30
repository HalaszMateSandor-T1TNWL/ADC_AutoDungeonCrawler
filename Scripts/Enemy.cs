using Godot;
using System;

public partial class Enemy : Entity
{
	[Signal] public delegate void HPChangedEventHandler(float currentHP);
	
	public float MaxHealth = 100.0f;
	
	public override void _Ready()
	{
		AddToGroup("enemy");

		HPChange(5);
	}

	public override void OnQueueForFree()
    {
        this.QueueFree();
    }

	public void HPChange(float change)
	{
		CurrentHealth = MaxHealth;
		if(change < 0)
		{
			DamageTaken(change);
		}else if(change >= 0)
		{
			HealthGained(change);
		}
		EmitSignal(nameof(HPChanged), CurrentHealth);
	}
	
	public void DamageTaken(float damage)
	{
		TakeDamage(damage);
		if(CurrentHealth <= 0)
		{
			QueueFree();
		}
	}

    public bool IsSpawnValid()
    {
        return _astar.Region.HasPoint(_tileMap.LocalToMap(this.GlobalPosition)) == false ||
            _tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)) == null ||
            (bool)_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)).GetCustomData("Walkable") == false;
    }

		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, new Enemy().MaxHealth);
	}
	public void Heal(float amount)
	{
		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
	}
}
