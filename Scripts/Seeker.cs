using Godot;
using System;
using System.Threading.Tasks;

public partial class Seeker : CharacterBody2D
{
	[Export] public float moveSpeed = 50.0f;
	
	public float CurrentHealth = 100.0f;
	public float MaxHealth = 100.0f;
	public float damage = 2.0f;

	[Signal] public delegate void DealDamageEventHandler(float damage);
	
	
	private Node2D _target = null;
	//needed for testing
	public Node2D CurrentTarget => _target;
	
	private NavigationAgent2D _navigationAgent = null;
	
	public override void _Ready()
	{
		_navigationAgent = GetNode<NavigationAgent2D>($"NavigationAgent2D");
		SeekerSetup();
	}
	
	public async Task SeekerSetup()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
		if(_target != null)
			_navigationAgent.TargetPosition = _target.GlobalPosition;
	}
	
	// Martin: I changed it bc thats the only way I could make the tests work
	public void AcquireTarget(Node customTargetContainer = null)
	{
		Node targetContainer = customTargetContainer;

		if (targetContainer == null)
		{
			var tree = GetTree();
			if (tree != null && tree.GetNodesInGroup("enemy").Count > 0)
			{
				targetContainer = tree.GetNodesInGroup("enemy")[0];
			}
		}
		if (targetContainer != null)
		{
			var targets = targetContainer.GetChildren();
			if (targets != null && targets.Count > 0)
			{
				var newTarget = targets[0];
				_target = (Node2D)newTarget;
			}
		}
	}

	//for testing again
	public Vector2 CalculateVelocityToTarget(Vector2 currentPosition, Vector2 targetPosition)
	{
		return currentPosition.DirectionTo(targetPosition) * moveSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(IsInstanceValid(_target))
		{
			_navigationAgent.TargetPosition = _target.GlobalPosition;
		}
		else
		{
			AcquireTarget();
		}
		
		if(_navigationAgent.IsNavigationFinished())
		{
			return;
		}
		
		var currentAgentPosition = this.GlobalPosition;
		var nextPathPosition = _navigationAgent.GetNextPathPosition();
		//this way the test actually shows if there is a problem
		Vector2 newVelocity = CalculateVelocityToTarget(currentAgentPosition, nextPathPosition);

		if (_navigationAgent.AvoidanceEnabled)
		{
			_navigationAgent.SetVelocity(newVelocity);
		}
		else
		{
			OnNavigationAgent2DVelocityComputed(newVelocity);
		}
		
		MoveAndSlide();
	}
	
	public void OnNavigationAgent2DVelocityComputed(Vector2 safeVelocity)
	{
		Velocity = safeVelocity;
	}
}
