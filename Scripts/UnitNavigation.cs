using Godot;
using Godot.Collections;
using System.Linq;

public partial class UnitNavigation : Node
{
	[Signal] public delegate void PathCalculatedEventHandler(Array<Vector2I> points, Entity movingEntity);
	
	public static UnitNavigation Instance { get; private set; }
	
	public Eye Eye { get; set; }
	public AStarGrid2D Astar { get; set; }
	public TileMapLayer TileMap { get; set; }
	public Rect2I GameArea { get; set; }

	public override void _Ready()
	{
		Instance = this;
	}

	public void Initialize(Eye eye, TileMapLayer tilemap)
	{
		Eye = eye;
		TileMap = tilemap;

		GameArea = new Rect2I(Vector2I.Zero, tilemap.GetUsedRect().Size);
		Astar = new AStarGrid2D
		{
			Region = GameArea,
			CellSize = new Vector2I(32,32),
			DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never
		};
		Astar.Update();
		eye.GridChanged += UpdateOccupiedTiles;
		GD.Print("Unit navigation system successfully initialized!");
	}

	public void UpdateOccupiedTiles()
	{
		Astar.FillSolidRegion(GameArea, false);
		foreach(Vector2I pos in Eye.GetAllOccupiedTiles())
		{
			Astar.SetPointSolid(pos);
		}
	}

	public Vector2I GetNextPosition(Node2D movingEntity, Node2D targetEntity)
	{
		Vector2I unitTile = TileMap.LocalToMap(TileMap.ToLocal(movingEntity.GlobalPosition));
		Vector2I targetTile = TileMap.LocalToMap(TileMap.ToLocal(targetEntity.GlobalPosition));

		Astar.SetPointSolid(unitTile, false);

		Array<Vector2I> path = Astar.GetIdPath(unitTile, targetTile, true);
		EmitSignal(nameof(PathCalculated), path, movingEntity);

		if(path.Count() == 1 && path[0] == unitTile)
		{
			Astar.SetPointSolid(unitTile, true);
			return new Vector2I(-1, -1);
		}

		Vector2I nextTile = path[1];
		Eye.RemoveUnit(unitTile);
		Eye.AddUnit(movingEntity, nextTile);
		Astar.SetPointSolid(nextTile, true);

		return (Vector2I)TileMap.ToGlobal(TileMap.MapToLocal(nextTile));
	}
}
