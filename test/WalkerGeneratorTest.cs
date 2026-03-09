using GdUnit4;
using Godot;
using System;
using System.Threading.Tasks;
using static GdUnit4.Assertions;


namespace ADC.Tests
{
	[TestSuite]
	public class WalkerGeneratorTest
	{
		[TestCase]
		public void TestIfInitializeGridGeneratesCorrectDimensions()
		{
			var generator = new WalkerGenerator();

			generator.InitializeGrid();

			//is the grid rly 100*100
			AssertThat(generator.grid.Count).IsEqual(100);
			AssertThat(generator.grid[1].Count).IsEqual(100);

			//are the corners actually walls
			AssertThat(generator.grid[0][0]).IsEqual(1);
			AssertThat(generator.grid[50][50]).IsEqual(1);
			AssertThat(generator.grid[99][99]).IsEqual(1);

			generator.QueueFree();
		}
		[TestCase]
		public void TestIfGenerateRoomRespectsSizeBoundaries()
		{
			
			var generator = new WalkerGenerator();

			for (int i = 0; i < 100; i++)
			{
				Rect2 room = generator.GenerateRoom();

				AssertThat(room.Size.X).IsBetween(10, 20);
				AssertThat(room.Size.Y).IsBetween(10, 20);

				AssertThat(room.Position.X).IsGreaterEqual(1);
				AssertThat(room.Position.Y).IsGreaterEqual(1);
			}

			generator.QueueFree();
		}
	}
}
