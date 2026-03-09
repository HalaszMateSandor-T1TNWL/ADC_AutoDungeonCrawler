using Godot;
using System;

public partial class Enemy : Area2D
{
	[Signal] public delegate void DamageEventHandler(float damage);
	
	public float CurrentHealth = 2.0f;
	public float MaxHealth = 100.0f;

	private TileMapLayer _tileMap;
	private AStarGrid2D _astar;
	public float health = 100.0f;

	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>($"../TileMapLayer");

		_astar = new AStarGrid2D();
		_astar.Region = _tileMap.GetUsedRect();
		_astar.CellSize = new Vector2I(32, 32);
		_astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		_astar.Update();

		if(
			_astar.Region.HasPoint(_tileMap.LocalToMap(this.GlobalPosition)) == false || 
			_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)) == null || 
			(bool)_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)).GetCustomData("Walkable") == false
		) 
		{
			QueueFree();
		}
	}
	
	public void OnBodyEntered(Node2D area)
	{
		this.QueueFree();
	}
	
	public void OnDamageDealt(float damage)
	{
		CurrentHealth -= damage;
		EmitSignal(nameof(Damage), damage);
	}
	
}
