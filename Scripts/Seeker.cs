using Godot;
using Godot.Collections;
using System.IO;
using System.Linq;

public partial class Seeker : Entity
{
	[Export] public new float movementSpeed = 100.0f;
	[Export] public new int attackRange = 0;
	[Export] public new float maxHealth = 100.0f;
	[Export] public new float CurrentHealth = 100.0f;
	[Export] public new float damage = 2.0f;


	[Signal] public delegate void DealDamageEventHandler(float damage);
	
	
	public override void _Ready()
	{
		
	}

	public void OnQueueForFree()
	{
		this.QueueFree();
	}

	public void OnAreaEntered(Area2D area)
	{
		if(area.IsInGroup("enemy"))
		{
			GD.Print(area.GlobalPosition);
		}
	}
}
