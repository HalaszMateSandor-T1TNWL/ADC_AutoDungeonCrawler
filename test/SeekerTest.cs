using GdUnit4;
using Godot;
using System;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	//unit testing using AAA method
	[TestSuite]
	public class SeekerTest
	{
		[TestCase]
		public void TestAcquireTarget_Logic()
		{
			//establishing the test field 
			var seeker = new Seeker();

			var enemyContainer = new Godot.Collections.Array<Node>();
			var actualEnemy = new Node2D();
			enemyContainer.Add(actualEnemy);

			seeker.AcquireTarget(enemyContainer);

			//finally testing
			AssertThat(seeker.CurrentTarget).IsNotNull();
			AssertThat(seeker.CurrentTarget).IsEqual(actualEnemy);

			seeker.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}
		[TestCase]
		public void TestMovement_VelocityPointsToTarget()
		{
			var seeker = new Seeker();

			seeker.GlobalPosition = new Vector2(0, 0);
			Vector2 dummyTargetPosition = new Vector2(50, 0);

			//this way I don't have to play with Godot's engine
			seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);
			
			//testing
			AssertThat(seeker.Velocity.X).IsGreater(0);
			AssertThat(seeker.Velocity.Y).IsEqual(0);

			seeker.QueueFree();
		}
		[TestCase]
		public void TestMovement2_VelocityPointsToTarget()
		{
			var seeker = new Seeker();

			seeker.GlobalPosition = new Vector2(0, 0);
			Vector2 dummyTargetPosition = new Vector2(-50, -50);

			//this way I don't have to play with Godot's engine
			seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);

			//testing
			AssertThat(seeker.Velocity.X).IsLess(0);
			AssertThat(seeker.Velocity.Y).IsLess(0);

			seeker.QueueFree();
		}
		//edge case? muhaha
		[TestCase]
		public void testWhatIfThereIsNoTarget()
		{
			var seeker = new Seeker();
			var enemyContainer = new Godot.Collections.Array<Node>();

			seeker.AcquireTarget(enemyContainer);

			//testing
			AssertThat(seeker.CurrentTarget).IsNull();

			seeker.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}

		[TestCase]
		public void testWhatIfTheTargetPosIsTheCurrentPos()
		{
			var seeker = new Seeker();

			seeker.GlobalPosition = new Vector2(10, 10);
			Vector2 dummyTargetPosition = new Vector2(10, 10);

			seeker.Velocity = seeker.CalculateVelocityToTarget(seeker.GlobalPosition, dummyTargetPosition);

			//testing
			AssertThat(seeker.Velocity.X).IsEqual(0);
			AssertThat(seeker.Velocity.Y).IsEqual(0);

			seeker.QueueFree();

		}
		//wall and other obsticle collision test coming soon... 
	}
}
