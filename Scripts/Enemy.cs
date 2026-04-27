using Godot;
using Godot.Collections;

public partial class Enemy : Entity
{
	[Signal] public delegate void HPChangedEventHandler(float currentHP);

	Array<Node> targets = [];
	Overseer overseer;

	public float MaxHealth = 100.0f;
	
	public override void _Ready()
	{
		AddToGroup("enemy");
		pathfinding = GetNodeOrNull<Pathfinder>($"Pathfinder");
		overseer = GetNode<Overseer>($"..");
		HPChange(5);
	}

	public override void _Process(double delta)
	{
		
		targets = overseer.eye.GetAllUnits();
		if(targets.Count > 0)
		{
			Node2D _target = (Node2D)targets[0];
			UnitNavigation.Instance.GetNextPosition(this, _target);
		}
	}

	public override void OnQueueForFree()
	{
		QueueFree();
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

			if(_currentIdPath.Count > attackRange)
			{
				_targetPosition = _tileMap.MapToLocal(_currentIdPath.First());
			}
			else
				_isMoving = false;
		}
	}

	public void OnAreaEntered(Area2D area)
	{
		if(area.IsInGroup("Player"))
		{
			GD.Print(area.GlobalPosition);
		}
	}
}
