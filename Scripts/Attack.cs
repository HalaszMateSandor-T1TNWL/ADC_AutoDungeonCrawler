using Godot;
using System;

public partial class Attack : Node
{
	private Pathfinder _pathfinder;
	private Timer _cooldownTimer;
	private Entity _parent;
	private bool _onCooldown = false;

	public override void _Ready()
	{
		_parent = GetParent<Entity>();
		_pathfinder = GetParent().GetNode<Pathfinder>("Pathfinder");

		_cooldownTimer = new Timer();
		_cooldownTimer.OneShot = true;
		AddChild(_cooldownTimer);
		_cooldownTimer.Timeout += OnCooldownFisished;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_onCooldown)
		 return;
		if (!IsInstanceValid(_pathfinder.CurrentTarget))
			return;

		if (_pathfinder._currentIdPath.Count <= _parent.attackRange)
		{
			PerformAttack();
		}
	}
	private void PerformAttack()
	{
		GD.Print("Performing attack");
		_onCooldown = true;

		Entity target = _pathfinder.CurrentTarget as Entity;
		if (IsInstanceValid(target))
		{
			target.TakeDamage(GetParent<Entity>().damage);
		}

		_cooldownTimer.WaitTime = _pathfinder._parent.attackspeed;
		_cooldownTimer.Start();
	}

	private void OnCooldownFisished()
	{
		GD.Print("Cooldown finished");
		_onCooldown = false;

		if (!IsInstanceValid(_pathfinder.CurrentTarget))
			_pathfinder.AcquireTarget();
	}

}
