using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public partial class Eye : Node
{
	[Signal] public delegate void GridChangedEventHandler();
	
	public TileMapLayer tileMap;
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
	}

	public void AddUnit(Node unit, Vector2I pos)
	{
		tiles.Add(pos, unit);
		GD.Print("Unit Added: " + unit + " At: " + pos + " occupied tiles are now: " + tiles.Count);
		unit.Connect("tree_exited", Callable.From( () => OnTreeExited(pos, unit))); // Creating a new Callable from a lambda expression 'cause you can't bind like in GDScript
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
