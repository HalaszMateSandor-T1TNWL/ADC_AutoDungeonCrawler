using Godot;
using Godot.Collections;
using System;

public partial class RoomDebug : Node2D
{
	private Array<Rect2> _rooms;
	private TileMapLayer _tilemap;

	public override void _Ready()
	{
		_rooms = new Array<Rect2>();
		_tilemap = GetNode<TileMapLayer>($"../../TileMapLayer");
	}


	public override void _Draw()
	{
		foreach(Rect2 room in _rooms)
		{
			Vector2 position = _tilemap.MapToLocal((Vector2I)room.Position);
			Vector2 size = _tilemap.MapToLocal((Vector2I)room.Size);
			Rect2 rect = new Rect2(
				position,
				size
			);
			DrawRect(rect, Colors.Red, false, 1);
		}
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}



	public void OnDebugRoom(Rect2 room)
	{
		_rooms.Add(room);
	}
	
	public void OnDungeonFlushed()
	{
		_rooms.Clear();
		QueueRedraw();
	}

}
