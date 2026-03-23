using GdUnit4;
using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	//unit testing using AAA method
	[TestSuite]
	public class SeekerTest
	{
		[TestCase]
		public void TestAcquireTargetLogic()
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
		public void TestMovementVelocityPointsToTarget()
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
		public void TestMovement2VelocityPointsToTarget()
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

		//TDD test cases for the target selection:

		[TestCase]
		public void testTargetSelectionByDistance()
		{
			var seeker = new Seeker();

			seeker.GlobalPosition = new Vector2(0, 0);

			var enemyContainer = new Godot.Collections.Array<Node>();

			var unreachableEnemy = new Node2D();
			unreachableEnemy.GlobalPosition = new Vector2(100000000000, 100000000000); //just put it far away
			unreachableEnemy.Name = "Unreachable";
			enemyContainer.Add(unreachableEnemy);

			var reachableEnemy = new Node2D();
			reachableEnemy.GlobalPosition = new Vector2(50, 0);
			reachableEnemy.Name = "Reachable";
			enemyContainer.Add(reachableEnemy);

			seeker.AcquireTarget(enemyContainer);

			AssertThat(seeker.CurrentTarget).IsEqual(reachableEnemy);
			AssertThat(seeker.CurrentTarget).IsNotEqual(unreachableEnemy);

			seeker.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}

		[TestCase]
		public async Task testTargetSelectionIgnoresEnemyBehindWall()
		{
			var tree = (SceneTree)Engine.GetMainLoop();
			var testRoot = new Node2D();
			tree.Root.AddChild(testRoot);

			List<List<int>> grid = new List<List<int>>();
			var tilemap = new TileMapLayer();
			for(int x = 0; x < 1; x++)
			{
				grid.Add(new List<int>());
				for (int y = 0; y < 99; y++)
				{
					grid[x].Add(1);
				}
			}

			for (int x = 0; x < 1; x++)
			{
				for (int y = 0; y < 99; y++)
				{
					Vector2I tiles = new Vector2I(x, y);

					if(y == 50)
					{
						tilemap.SetCell(tiles, 1, new Vector2I(0,0));
					}
					else
					{
						tilemap.SetCell(tiles, 1, new Vector2I(1,0));
					}
				
				}
			}

			testRoot.AddChild(tilemap);
	
			var seeker = new Seeker();
			seeker.GlobalPosition = new Vector2(0, 0);
			testRoot.AddChild(seeker);

			var enemyContainer = new Godot.Collections.Array<Node>();

			var enemyBehindWall = new Node2D();
			enemyBehindWall.GlobalPosition = new Vector2(100, 0);
			enemyContainer.Add(enemyBehindWall);

			//wait a frame so godot can register the wall and the CollisionShape
			await tree.ToSignal(tree, SceneTree.SignalName.PhysicsFrame);

			seeker.AcquireTarget(enemyContainer);

			AssertThat(seeker.GlobalPosition).IsBetween(new Vector2(0, 0), new Vector2(50, 0));

			testRoot.QueueFree();
		}

	}
}
