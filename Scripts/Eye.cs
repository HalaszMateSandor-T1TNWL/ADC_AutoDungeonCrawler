using Godot;
using Godot.Collections;
using System.Linq;

public partial class Eye : Node
{
	[Signal] public delegate void GridChangedEventHandler();
	
	public TileMapLayer tileMap;
	public AStarGrid2D astar;
	public Dictionary<Vector2I, Node> tiles = [];

	public override void _Ready()
	{
		tileMap = GetNode<TileMapLayer>($"../TileMapLayer");
		for(int x = 0; x < tileMap.GetUsedRect().Size.X; x++)
		{
			for(int y = 0; y < tileMap.GetUsedRect().Size.Y; y++)
			{
				tiles[new Vector2I(x, y)] = null;
			}
		}
		astar = new AStarGrid2D
		{
			Region = tileMap.GetUsedRect(),
			CellSize = new Vector2I(32,32),
			DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never
		};
		astar.Update();
	}

	public void AddUnit(Node unit, Vector2I pos)
	{
		tiles[pos] = unit;
		GD.Print("Unit Added: " + unit + " At: " + pos + " occupied tiles are now: " + tiles.Count);
		unit.Connect("tree_exited", Callable.From( () => OnTreeExited(pos, unit) )); // Creating a new Callable from a lambda expression 'cause you can't bind like in GDScript
		EmitSignal(nameof(GridChanged));
	}
	
	public void RemoveUnit(Vector2I pos)
	{
		Node unit = tiles[pos];

		if(unit == null)
		{
			return ;
		}
		
		unit.Disconnect("tree_exited", Callable.From( () => OnTreeExited(pos, unit) ));
		tiles[pos] = null;
		GD.Print("Unit Removed: " + unit + " At: " + pos + " occupied tiles are now: " + tiles.Count);
		EmitSignal(nameof(GridChanged));
	}

	public bool IsTileOccupied(Vector2I pos)
	{
		return tiles[pos] != null;
	}

	public bool IsMapFull()
	{
		return tiles.Keys.All(IsTileOccupied);
	}

	public Vector2I GetFirstEmptyTile()
	{
		foreach(Vector2I pos in tiles.Keys)
		{
			if(!IsTileOccupied(pos))
			{
				return pos;
			}
		}

		return new Vector2I(-1,-1);
	}

	public Array<Node> GetAllUnits()
	{
		Array<Node> units = [];
		foreach(Node unit in tiles.Values)
		{
			if(unit != null)
				units.Append(unit);
		}

		return units;
	}

	public Array<Vector2I> GetAllOccupiedTiles()
	{
		Array<Vector2I> occupiedTiles = [];

		foreach(Vector2I pos in tiles.Keys)
		{
			if(tiles[pos] != null)
				occupiedTiles.Append(pos);
		}

		return occupiedTiles;
	}
	
	private void OnTreeExited(Vector2I pos, Node unit)
	{
		if(unit.IsQueuedForDeletion())
		{
			tiles[pos] = null;
			tiles.Remove(pos);
			EmitSignal(nameof(GridChanged));
		}
	}
}
