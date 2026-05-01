using GdUnit4;
using Godot;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	[TestSuite]
	public class EyeTest
	{
		[TestCase]
		public void GridRegistersUnitPresence()
		{
			var eye = new Eye();
			var dummyUnit = new Node();
			var targetPosition = new Vector2I(3, 3);

			eye.AddUnit(dummyUnit, targetPosition);

			AssertThat(eye.tiles[targetPosition]).IsEqual(dummyUnit);

			dummyUnit.QueueFree();
			eye.QueueFree();
		}

		[TestCase]
		public void GridAutomaticallyClearsAndNotifiesOnUnitDeath()
		{
			var eye = new Eye();
			var dummyUnit = new Node();
			var targetPosition = new Vector2I(5, 5);

			eye.AddUnit(dummyUnit, targetPosition);

			bool wasSignalEmitted = false;
			eye.GridChanged += () => wasSignalEmitted = true;

			dummyUnit.QueueFree();
			dummyUnit.EmitSignal(Node.SignalName.TreeExited);

			AssertThat(eye.tiles.ContainsKey(targetPosition)).IsFalse();
			AssertThat(wasSignalEmitted).IsTrue();

			eye.QueueFree();
		}

		[TestCase]
		public void MapCorrectlyIdentifiesOccupiedTiles()
		{
			var eye = new Eye();
			var dummyUnit = new Node();
			var targetPosition = new Vector2I(1, 1);

			eye.AddUnit(dummyUnit, targetPosition);

			AssertThat(eye.IsTileOccupied(targetPosition)).IsTrue();

			dummyUnit.QueueFree();
			eye.QueueFree();
		}
	}
}
