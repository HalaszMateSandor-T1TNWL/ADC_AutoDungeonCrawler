using Godot;
using Godot.Collections;
using Microsoft.CodeAnalysis.Emit;
using System.Linq;

public partial class Pathfinder : Node
{
	[Signal] public delegate void QueueForFreeEventHandler();
	[Signal] public delegate void SetParamsEventHandler();

	private AStarGrid2D _astar = null;
	private TileMapLayer _tilemap;
	private Node2D _target;
	public Node2D CurrentTarget => _target;
	private Node2D _body = null;

	public Entity _parent;

	private Vector2 _targetPosition;
	public Array<Vector2I> _currentIdPath;

	private bool _isMoving;
	private bool _isColliding;

	public override void _Ready()
	{
		_parent = GetParent<Entity>();
		_tilemap = GetNode<TileMapLayer>($"../../TileMapLayer");

		_currentIdPath = new Array<Vector2I>();
		_astar = new AStarGrid2D
		{
			Region = _tilemap.GetUsedRect(),
			CellSize = new Vector2I(32,32),
			DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never
		};
		_astar.Update();

		EmitSignal(nameof(SetParams));

		QueueForFree += _parent.OnQueueForFree;

		if(CheckValidPlacement())
		{
			EmitSignal(nameof(QueueForFree));
		}
		
		SetTileMapData();
	}

	public bool CheckValidPlacement()
	{
		if(_astar != null)
			return !_astar.Region.HasPoint(_tilemap.LocalToMap(_parent.GlobalPosition)) ||
				_tilemap.GetCellTileData(_tilemap.LocalToMap(_parent.GlobalPosition)) == null ||
				(bool)_tilemap.GetCellTileData(_tilemap.LocalToMap(_parent.GlobalPosition)).GetCustomData("Walkable") == false;
		else
			return true;
	}

	public void SetTileMapData()
	{
		for(int x = 0; x < _tilemap.GetUsedRect().Size.X; x++)
		{
			for(int y = 0; y < _tilemap.GetUsedRect().Size.Y; y++)
			{
				Vector2I tilePosition = new Vector2I(
					x + _tilemap.GetUsedRect().Position.X,
					y + _tilemap.GetUsedRect().Position.Y
				);

				TileData tileData = _tilemap.GetCellTileData(tilePosition);

				if(tileData == null || (bool)tileData.GetCustomData("Walkable") == false)
				{
					_astar.SetPointSolid(tilePosition);
				}
			}
		}
	}

	public void AcquireTarget(Array<Node> customTargetContainer = null)
	{
		Array<Node> targetContainer = customTargetContainer;

		if(targetContainer == null)
		{
			SceneTree tree = GetTree();
			Array<Node> enemyGroup = tree.GetNodesInGroup(_parent.GetGroups().Contains("enemy") ? "player" : "enemy"); //there has to be a better way, but I don't care
			if(tree != null && enemyGroup.Count > 0)
			{
				targetContainer = enemyGroup;
			}
		}

		if(targetContainer != null)
		{
			Array<Node> targets = targetContainer;
			Node2D bestCandidate = null;
			int shortestDistance = int.MaxValue;
			if(targets != null && targets.Count > 0)
			{
				foreach(Node target in targets)
				{
					int currentDistance = CalcDistance((Node2D)target);
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

	private int CalcDistance(Node2D target)
	{
		return (int)Mathf.Sqrt(Mathf.Pow(_parent.GlobalPosition.X - target.GlobalPosition.X, 2.0f) + 
						  Mathf.Pow(_parent.GlobalPosition.Y - target.GlobalPosition.Y, 2.0f));
	}

	public override void _Process(double delta)
	{
		Array<Vector2I> idPath = [];

		if(IsInstanceValid(_target) && _isMoving == false)
		{
			Vector2I currentAgentPosition = _tilemap.LocalToMap(_parent.GlobalPosition);
			Vector2I targetPosition = _tilemap.LocalToMap(_target.GlobalPosition);

			idPath = _astar.GetIdPath(currentAgentPosition, targetPosition, true).Slice(0);
		}
		else if(IsInstanceValid(_target) && _isMoving == true)
		{
			AcquireTarget();

			Vector2I currentAgentPosition = _tilemap.LocalToMap(_parent.GlobalPosition);
			Vector2I targetPosition = _tilemap.LocalToMap(_target.GlobalPosition);

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

	public void UpdatePath()
	{
		if(!IsInstanceValid(_target) && !_isMoving)
			return;
		
		Vector2I currentAgentPosition = _tilemap.LocalToMap(_parent.GlobalPosition);
		Vector2I targetPosition = _tilemap.LocalToMap(_target.GlobalPosition);

		_currentIdPath = _astar.GetIdPath(currentAgentPosition, targetPosition, true).Slice(0);
		_currentIdPath.Remove(_currentIdPath.Last());
	}

	public override void _PhysicsProcess(double delta)
	{
		if(_currentIdPath.Count <= 0)
			return;

		if(_isMoving == false)
		{
			_targetPosition = _tilemap.MapToLocal(_currentIdPath.First());
			_isMoving = true;
		}
		
		UpdatePath();

		if(_currentIdPath.Count > _parent.attackRange)
		{
			_parent.GlobalPosition = _parent.GlobalPosition.MoveToward(_targetPosition, _parent.movementSpeed * (float)delta);

			if(_parent.GlobalPosition == _targetPosition)
			{
				_currentIdPath.Remove(_currentIdPath.First());

				if(_currentIdPath.Count <= 0)
					return;
				else
					_targetPosition = _tilemap.MapToLocal(_currentIdPath.First());
			}
		}
		else
			_isMoving = false;
	}
}
