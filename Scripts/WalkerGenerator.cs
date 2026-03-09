using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


[Tool]
public partial class WalkerGenerator : Node
{
  [Export] public Vector2 mapDimensions = new Vector2(40, 60);
  [Export] public int totalSteps = 600;
  [ExportToolButton("GenerateMap")] public Callable GenerateMapButton => Callable.From(ReGenerate);

  [Export] public Vector2I wallTileTop = new Vector2I(8, 0);
  [Export] public Vector2I wallTileBottom = new Vector2I(0, 0);
  [Export] public Vector2I floorTile = new Vector2I(1, 0);
  private TileMapLayer _tileMapLayer;

  private int _roomSize = 20;
  private int _minRoomSize = 10;
  private const int _width = 100;
  private const int _height = 100;
  private const int _maxRooms = 10;

  //had to rewrite 2 lines for testing
  //basicly just give value to the lists in the beginning without the _Ready func
  private Random _random = new Random();
  public List<List<int>> grid = new List<List<int>>();

  private List<Rect2> rooms;

  public override void _Ready()
  {
	//changed bc the the new code on line 27,28
	_tileMapLayer = GetNode<TileMapLayer>($"../TileMapLayer");
	rooms = new List<Rect2>();

	InitializeGrid();

	GenerateMap();
	
	DrawMap();
  }
	
	public void FlushDungeon()
	{
		//slight change
        grid.Clear();

        InitializeGrid();
		
		rooms = new List<Rect2>();
		
		for (int x = 0; x < _width; x++)
		{
	  		for (int y = 0; y < _height; y++)
			{
				Vector2I tiles = new Vector2I(x, y);
				_tileMapLayer.SetCell(tiles, 0, new Vector2I(-1, -1));
			}
		}
	}
	
	public void OnRegenerate()
	{
		FlushDungeon();
		
		ReGenerate();
	}
	
	public void ReGenerate()
	{
		GenerateMap();
		
		DrawMap();
	}

  public void InitializeGrid()
  {
	for (int x = 0; x < _width; x++)
	{
	  grid.Add(new List<int>());
	  for (int y = 0; y < _height; y++)
	  {
		grid[x].Add(1);
	  }
	}

  }

  public void GenerateMap()
  {
	for (int i = 0; i < _maxRooms; i++)
	{
	  Rect2 room = GenerateRoom();

	  if (PlaceRoom(room))
	  {
		if (rooms.Count > 0)
		{
		  ConnectRooms(rooms[^1], room);
		}
		rooms.Add(room);
		GD.Print(rooms.Count);
	  }
	}
  }

  public Rect2 GenerateRoom()
  {
	int width = _random.Next(_minRoomSize, _roomSize);
	int height = _random.Next(_minRoomSize, _roomSize);

	int x = _random.Next() % (_width - width - 1) + 1;
	int y = _random.Next() % (_height - height - 1) + 1;

	return new Rect2(x, y, width, height);
  }

  public bool PlaceRoom(Rect2 room)
  {
	for (int x = (int)room.Position.X; x < (int)room.End.X; x++)
	{
	  for (int y = (int)room.Position.Y; y < (int)room.End.Y; y++)
	  {
		if (grid[x][y] == 0)
		{
		  return false;
		}
	  }
	}

	for (int x = (int)room.Position.X; x < (int)room.End.X; x++)
	{
	  for (int y = (int)room.Position.Y; y < (int)room.End.Y; y++)
	  {
		grid[x][y] = 0;
	  }
	}
	return true;
  }

  public void ConnectRooms(Rect2 from, Rect2 to, int corridorWidth = 1)
  {
	Vector2 start = new Vector2((int)(from.Position.X + from.Size.X / 2),
				  (int)(from.Position.Y + from.Size.Y / 2));

	Vector2 end = new Vector2((int)(to.Position.X + to.Size.X / 2),
				  (int)(to.Position.Y + to.Size.Y / 2));

	Vector2 currentPosition = start;


	while (currentPosition.X != end.X)
	{
	  if (end.X > currentPosition.X)
	  {
		currentPosition.X++;
	  }
	  else
	  {
		currentPosition.X--;
	  }

	  for (int i = -(int)(corridorWidth / 2); i < (int)(corridorWidth / 2) + 1; i++)
	  {
		for (int j = -(int)(corridorWidth / 2); j < (int)(corridorWidth / 2) + 1; j++)
		{
		  if (currentPosition.Y + j >= 0 && currentPosition.Y + j < _height
			&& currentPosition.X + i >= 0 && currentPosition.X + i < _width)
		  {
			grid[(int)currentPosition.X + i][(int)currentPosition.Y + j] = 0;
		  }
		}
	  }
	}

	while (currentPosition.Y != end.Y)
	{
	  if (currentPosition.Y < end.Y)
	  {
		currentPosition.Y++;
	  }
	  else
	  {
		currentPosition.Y--;
	  }

	  for (int i = -(int)(corridorWidth / 2); i < (int)(corridorWidth / 2) + 1; i++)
	  {
		for (int j = -(int)(corridorWidth / 2); j < (int)(corridorWidth / 2) + 1; j++)
		{
		  if (currentPosition.X + i >= 0 && currentPosition.X + i < _width
			&& currentPosition.Y + j >= 0 && currentPosition.Y + j < _height)
		  {
			grid[(int)currentPosition.X + i][(int)currentPosition.Y + j] = 0;
		  }
		}
	  }
	}
  }

  public void DrawMap()
  {
	for (int x = 0; x < _width; x++)
	{
	  for (int y = 0; y < _height; y++)
	  {
		Vector2I tiles = new Vector2I(x, y);
		if (grid[x][y] == 0)
		{
		  _tileMapLayer.SetCell(tiles, 1, floorTile);
		}
		else if (grid[x][y] == 1)
		{
		  if (y < _height - 1 && grid[x][y + 1] == 0)
		  {
			_tileMapLayer.SetCell(tiles, 1, wallTileBottom);
		  }
		  else if (y > 0 && grid[x][y - 1] == 0)
		  {
			_tileMapLayer.SetCell(tiles, 1, wallTileTop);
		  }
		  else
		  {
			_tileMapLayer.SetCell(tiles, 1, new Vector2I(-1, -1));
		  }
		}
		else
		{
		  _tileMapLayer.SetCell(tiles, 1, new Vector2I(-1, -1));
		}
	  }
	}
  }



}
