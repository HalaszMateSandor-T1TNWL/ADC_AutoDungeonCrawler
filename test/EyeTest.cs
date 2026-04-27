using GdUnit4;
using Godot;
using System.Threading.Tasks;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	[TestSuite]
	public class EyeTest
	{
		[TestCase]
		public void TestGridRegistersUnitPresence()
		{
			var eye = new Eye();
			var dummyUnit = new Entity();
			var targetPosition = new Vector2I(3, 3);

			eye.AddUnit(dummyUnit, targetPosition);

			AssertThat(eye.tiles[targetPosition]).IsEqual(dummyUnit);

			dummyUnit.QueueFree();
			eye.QueueFree();
		}

		[TestCase]
		public async Task TestGridAutomaticallyClearsAndNotifiesOnUnitDeath()
		{
			var tree = (SceneTree)Engine.GetMainLoop();
			var root = new Node();
			tree.Root.CallDeferred(Node.MethodName.AddChild, root);
			await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

			var eye = new Eye();
			root.AddChild(eye);

			var dummyUnit = new Entity();
			root.AddChild(dummyUnit);

			var targetPosition = new Vector2I(5, 5);
			eye.AddUnit(dummyUnit, targetPosition);

			bool wasSignalEmitted = false;
			eye.GridChanged += () => wasSignalEmitted = true;

			dummyUnit.QueueFree();

			await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
			await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);

			AssertThat(eye.tiles[targetPosition]).IsNull();
			AssertThat(wasSignalEmitted).IsTrue();

			root.QueueFree();
		}
	}
}
