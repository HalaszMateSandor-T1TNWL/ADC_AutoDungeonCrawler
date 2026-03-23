using Godot;
using System;

public partial class Enemy : Area2D
{
	[Signal] public delegate void HPChangedEventHandler(float currentHP);
	
	public float MaxHealth = 100.0f;

	public float CurrentHealth;

	public float damage;
	
	private TileMapLayer _tileMap;
	private AStarGrid2D _astar;

	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>($"../TileMapLayer");

		_astar = new AStarGrid2D();
		_astar.Region = _tileMap.GetUsedRect();
		_astar.CellSize = new Vector2I(32, 32);
		_astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		_astar.Update();

		if(IsSpawnValid())
		{
			QueueFree();
		}

		HPChange(5);
	}
	
	public bool IsSpawnValid()
	{
		return _astar.Region.HasPoint(_tileMap.LocalToMap(this.GlobalPosition)) == false || 
			_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)) == null || 
			(bool)_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)).GetCustomData("Walkable") == false;
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

	public void HealthGained(float amount)
	{
		Heal(amount);
	}
	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;

		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, new Enemy().MaxHealth);
	}
	public void Heal(float amount)
	{
		CurrentHealth += amount;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
	}

}
