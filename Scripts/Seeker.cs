using Godot;
using Godot.Collections;
using System.Linq;

public partial class Seeker : CharacterBody2D
{
	[Export] public float moveSpeed = 1.0f;
	
	private AStarGrid2D _astar;
	private TileMapLayer _tileMap;
	private bool _isMoving;
	private Vector2 _targetPosition;
	
	private Array<Vector2I> _currentIdPath;
	
	public float CurrentHealth = 100.0f;
	public float MaxHealth = 100.0f;
	public float damage = 2.0f;

	[Signal] public delegate void DealDamageEventHandler(float damage);
	
	
	private Node2D _target = null;
	//needed for testing
	public Node2D CurrentTarget => _target;
	
	public override void _Ready()
	{
		_tileMap = GetNode<TileMapLayer>($"../TileMapLayer");
		if(_tileMap == null)
		{
			GD.Print("Whoops! No tilemap for some reason!");
			return;
		}
		
		_currentIdPath = new Array<Vector2I>();
		
		_astar = new AStarGrid2D();
		_astar.Region = _tileMap.GetUsedRect();
		_astar.CellSize = new Vector2I(32, 32);
		_astar.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		_astar.Update();

		if(!_astar.Region.HasPoint(_tileMap.LocalToMap(this.GlobalPosition)) || _tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)) == null || (bool)_tileMap.GetCellTileData(_tileMap.LocalToMap(this.GlobalPosition)).GetCustomData("Walkable") == false)
		{
			QueueFree();
		}
		SetTileMapData();
	}
	
	public void SetTileMapData()
	{
		for(int x = 0; x < _tileMap.GetUsedRect().Size.X; x++)
		{
			for(int y = 0; y < _tileMap.GetUsedRect().Size.Y; y++)
			{	
				Vector2I tilePosition = new Vector2I(
					x + _tileMap.GetUsedRect().Position.X,
					y + _tileMap.GetUsedRect().Position.Y
				);

				//TileData tileData = _tileMap.GetCellTileData(tilePosition);

				if(_tileMap.GetCellTileData(tilePosition) == null || (bool)_tileMap.GetCellTileData(tilePosition).GetCustomData("Walkable") == false)
				{
					_astar.SetPointSolid(tilePosition);
				}
			}
		}
	}
	
	// Martin: I changed it bc thats the only way I could make the tests work
	public void AcquireTarget(Array<Node> customTargetContainer = null)
	{
		Array<Node> targetContainer = customTargetContainer;

		if(targetContainer == null)
		{
			SceneTree tree = GetTree();
			if(tree != null && tree.GetNodesInGroup("enemy").Count > 0)
			{
				targetContainer = tree.GetNodesInGroup("enemy");
			}
		}
		if(targetContainer != null)
		{
			Array<Node> targets = targetContainer;
			
			Node2D bestCandidate = null;
			float shortestDistance = 10000000000.0f;
			if(targets != null && targets.Count > 0)
			{
				foreach(Node target in targets)
				{
					float currentDistance = CalcDistance((Node2D)target);
					if(currentDistance < shortestDistance)
					{
						shortestDistance = currentDistance;
						bestCandidate = (Node2D)target;
					}
				}
			}
			
			if(bestCandidate != null)
			{
				_target = bestCandidate;
			}
		}
	}

	public float CalcDistance(Node2D target)
	{
		return Mathf.Sqrt(Mathf.Pow(this.GlobalPosition.X - target.GlobalPosition.X, 2.0f) + Mathf.Pow(this.GlobalPosition.Y - target.GlobalPosition.Y, 2.0f));
	}

	//for testing again
	public Vector2 CalculateVelocityToTarget(Vector2 currentPosition, Vector2 targetPosition)
	{
		return currentPosition.DirectionTo(targetPosition) * moveSpeed;
	}

	public override void _Process(double delta)
	{
		Array<Vector2I> idPath = new Array<Vector2I>();

		if(IsInstanceValid(_target) && _isMoving == false)
		{
			Vector2I currentAgentPosition = _tileMap.LocalToMap(this.GlobalPosition);
			Vector2I targetPosition = _tileMap.LocalToMap(_target.GlobalPosition);

			idPath = _astar.GetIdPath(currentAgentPosition, targetPosition, true).Slice(0);
		}
		else if(IsInstanceValid(_target) && _isMoving == true)
		{
			AcquireTarget();

			Vector2I currentAgentPosition = _tileMap.LocalToMap(this.GlobalPosition);
			Vector2I targetPosition = _tileMap.LocalToMap(_target.GlobalPosition);

			idPath = _astar.GetIdPath(currentAgentPosition, targetPosition);
		}
		else
		{
			AcquireTarget();
		}

		if(idPath.Count > 0)
		{
			idPath.Remove(idPath.Last());
			_currentIdPath = idPath;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(_currentIdPath.Count <= 0)
			return;

		if(_isMoving == false)
		{
			_targetPosition = _tileMap.MapToLocal(_currentIdPath.First());
			_isMoving = true;
		}

		//this way the test actually shows if there is a problem
		this.GlobalPosition = this.GlobalPosition.MoveToward(_targetPosition, moveSpeed * (float)delta);
		
		if(this.GlobalPosition == _targetPosition)
		{
			_currentIdPath.Remove(_currentIdPath.First());

			if(_currentIdPath.Count > 0)
			{
				_targetPosition = _tileMap.MapToLocal(_currentIdPath.First());
			}
			else
				_isMoving = false;
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if(area.IsInGroup("enemy"))
		{
			GD.Print(area.GlobalPosition);
		}
	}
}
