using GdUnit4;
using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using static GdUnit4.Assertions;
using System.IO;

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
			var pathfinder = new Pathfinder();
			seeker.AddChild(pathfinder);
			pathfinder._parent = pathfinder.GetParent<Seeker>();

			var enemyContainer = new Godot.Collections.Array<Node>();
			var actualEnemy = new Node2D();
			actualEnemy.AddToGroup("enemy");

			enemyContainer.Add(actualEnemy);

			pathfinder.AcquireTarget(enemyContainer);

			//finally testing
			AssertThat(pathfinder.CurrentTarget).IsNotNull();
			AssertThat(pathfinder.CurrentTarget).IsEqual(actualEnemy);

			seeker.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}

		//edge case? muhaha
		[TestCase]
		public void testWhatIfThereIsNoTarget()
		{
			var pathfinder = new Pathfinder();
			var enemyContainer = new Godot.Collections.Array<Node>();

			pathfinder.AcquireTarget(enemyContainer);

			//testing
			AssertThat(pathfinder.CurrentTarget).IsNull();

			pathfinder.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}

		//TDD test cases for the target selection:

		[TestCase]
		public void testTargetSelectionByDistance()
		{
			var seeker = new Seeker();
			var pathfinder = new Pathfinder();

			seeker.AddChild(pathfinder);
			pathfinder._parent = pathfinder.GetParent<Seeker>();

			pathfinder._parent.GlobalPosition = new Vector2(0, 0);

			var enemyContainer = new Godot.Collections.Array<Node>();

			var unreachableEnemy = new Node2D
			{
				GlobalPosition = new Vector2(float.MaxValue, float.MaxValue), //just put it far away
				Name = "Unreachable"
			};
			unreachableEnemy.AddToGroup("enemy");

			enemyContainer.Add(unreachableEnemy);

			var reachableEnemy = new Node2D
			{
				GlobalPosition = new Vector2(50, 0),
				Name = "Reachable"	
			};
			reachableEnemy.AddToGroup("enemy");
			enemyContainer.Add(reachableEnemy);

			pathfinder.AcquireTarget(enemyContainer);

			AssertThat(pathfinder.CurrentTarget).IsEqual(reachableEnemy);
			AssertThat(pathfinder.CurrentTarget).IsNotEqual(unreachableEnemy);

			seeker.QueueFree();
			foreach(Node node in enemyContainer)
			{
				node.QueueFree();
			}
		}

		[TestCase]
		public async Task testWhatIfTheTargetPosIsTheCurrentPos()
		{
			var seeker = new Seeker
			{
				GlobalPosition = new Vector2(10, 10)
			};
			var pathfinder = new Pathfinder();

			seeker.AddChild(pathfinder);
			pathfinder._parent = pathfinder.GetParent<Seeker>();

			Node2D dummyTarget = new Node2D
            {
				GlobalPosition = new Vector2(10, 10)
			};
			dummyTarget.AddToGroup("enemy");

			var enemyContainer = new Godot.Collections.Array<Node>
            {
                dummyTarget
            };

			pathfinder.AcquireTarget(enemyContainer);
			pathfinder._PhysicsProcess(0.16528);

			//testing
			AssertThat(seeker.GlobalPosition.X).IsEqual(0);
			AssertThat(seeker.GlobalPosition.Y).IsEqual(0);

			seeker.QueueFree();
			dummyTarget.QueueFree();
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
			var pathfinder = new Pathfinder();

			seeker.AddChild(pathfinder);
			pathfinder._parent = pathfinder.GetParent<Seeker>();

			pathfinder._parent.GlobalPosition = new Vector2(0, 0);
			testRoot.AddChild(pathfinder);

			var enemyContainer = new Godot.Collections.Array<Node>();

			var enemyBehindWall = new Node2D
			{
				GlobalPosition = new Vector2(100, 0)	
			};
			enemyBehindWall.AddToGroup("enemy");
			enemyContainer.Add(enemyBehindWall);

			//wait a frame so godot can register the wall and the CollisionShape
			await tree.ToSignal(tree, SceneTree.SignalName.PhysicsFrame);

			pathfinder.AcquireTarget(enemyContainer);
			
			await tree.ToSignal(tree, SceneTree.SignalName.PhysicsFrame);

			AssertThat(pathfinder._parent.GlobalPosition).IsBetween(new Vector2(0, 0), new Vector2(50, 0));

			testRoot.QueueFree();
		}

	}
}
