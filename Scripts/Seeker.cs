using Godot;
using System;
using System.Threading.Tasks;

public partial class Seeker : CharacterBody2D
{
	[Export] public float moveSpeed = 50.0f;
	
	private Node2D _target = null;
	
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
	
	public void AcquireTarget()
	{
		if(GetTree().GetNodesInGroup("enemy").Count != 0)
		{
			var targetContainer = GetTree().GetNodesInGroup("enemy")[0];
		
			var targets = targetContainer.GetChildren();
			
			if(!(targets == null) || !(targets.Count == 0))
			{
				var newTarget = targets[0];
				_target = (Node2D)newTarget;
			}
		}
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
		Vector2 newVelocity = currentAgentPosition.DirectionTo(nextPathPosition) * moveSpeed;
		
		if(_navigationAgent.AvoidanceEnabled)
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
