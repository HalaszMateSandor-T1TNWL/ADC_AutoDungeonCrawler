using Godot;
using Godot.Collections;

public partial class UnitNavigationDebug : Node2D
{
	[Export] Array<Color> colorArray;
	[Export] Color color;
	[Export] TileMapLayer tileMap;

	Dictionary<Node, Array<Vector2I>> paths = [];

	public override void _Ready()
	{
		UnitNavigation.Instance.PathCalculated += OnPathCalculated;
		GD.Print("Unit navigation debugger online");
	}

	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("ui_accept"))
		{
			GD.Print("Queueing for redraw...");
			QueueRedraw();
		}
	}

	private void OnPathCalculated(Array<Vector2I> points, Entity movingEntity)
	{
		GD.Print("Path Calculated noting points...");
		paths[movingEntity] = points;
		QueueRedraw();
	}

	public override void _Draw()
	{
		for(int i = 0; i < UnitNavigation.Instance.Astar.Region.Size.X; i++)
		{
			for(int j = 0; j < UnitNavigation.Instance.Astar.Region.Size.Y; j++)
			{
				if(UnitNavigation.Instance.Astar.IsPointSolid(new Vector2I(i,j)))
					DrawRect(new Rect2(new Vector2I(i, j) * new Vector2I(32, 32), new Vector2I(32, 32)), color);
			}
		}

		int index = 0;

		foreach(Array<Vector2I> path in paths.Values)
		{
			DrawPath(path, colorArray[Mathf.Wrap(index, 0, colorArray.Count - 1)]);
			index++;
		}
	}

	private void DrawPath(Array<Vector2I> path, Color color)
	{
		for(int i = 1; i < path.Count; i++)
		{
			var from = tileMap.ToGlobal(tileMap.MapToLocal(path[i-1])) - GlobalPosition;
			var to = tileMap.ToGlobal(tileMap.MapToLocal(path[i])) - GlobalPosition;
			DrawLine(from, to, color);
			GD.Print("Path drawn from: " + from + " to: " + to);
		}
	}
}
