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
		public void TestGridCorrectDimensions()
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
		[TestCase]
		public void TestIfPlaceRoomMakesValidRoom()
		{
			var generator = new WalkerGenerator();
			generator.InitializeGrid();

			var testRoom = new Rect2(10, 10, 10, 10);
			bool result = generator.PlaceRoom(testRoom);

			AssertThat(result).IsEqual(true);

			//check if there is floor where it supposed to be and there is wall where it supposed to be
			AssertThat(generator.grid[10][10]).IsEqual(0);
			AssertThat(generator.grid[19][19]).IsEqual(0);

			AssertThat(generator.grid[9][10]).IsEqual(1);
			AssertThat(generator.grid[20][20]).IsEqual(1);

			generator.QueueFree();
		}

		[TestCase]
		public void TestIfPlaceRoomGivesFalseToOverlappingRooms()
		{
			var generator = new WalkerGenerator();
			generator.InitializeGrid();

			var room1 = new Rect2(10, 10, 10, 10);
			generator.PlaceRoom(room1);

			var room2 = new Rect2(15, 15, 10, 10);
			bool result = generator.PlaceRoom(room2);

			AssertThat(result).IsFalse();

			generator.QueueFree();
		}

		[TestCase]
		public void TestPlaceRoomIfExactNumberOfCellsAreCarved()
		{
			var generator = new WalkerGenerator();
			generator.InitializeGrid();

			var room = new Rect2(10, 10, 10, 10);
			generator.PlaceRoom(room);

			int floorCount = 0;

			for (int x = 0; x < 100; x++)
			{
				for (int y = 0; y < 100; y++)
				{
					if (generator.grid[x][y] == 0)
					{
						floorCount++;
					}
				}
			}

			AssertThat(floorCount).IsEqual(100);

			generator.QueueFree();
		}

		[TestCase]
		public void TestConnectRoomsCreatesFloorPathBetweenRooms()
		{
			var generator = new WalkerGenerator();
			generator.InitializeGrid();

			var room1 = new Rect2(10, 10, 1, 1);
			var room2 = new Rect2(15, 10, 1, 1);

			generator.ConnectRooms(room1, room2, 1);

			for(int x = 11; x <= 14; x++)
			{
				AssertThat(generator.grid[x][10]).IsEqual(0);
			}

			generator.QueueFree();
		}

	}
}
