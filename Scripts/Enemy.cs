using Godot;
using System;

public partial class Enemy : Area2D
{
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

		if(_astar.Region.HasPoint(_tileMap.LocalToMap(this.GlobalPosition)) == false)
		{
			QueueFree();
		}
	}


	private void OnBodyEntered(Node2D body)
	{
		QueueFree();
	}
}
