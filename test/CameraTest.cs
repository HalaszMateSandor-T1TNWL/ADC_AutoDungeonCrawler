using GdUnit4;
using Godot;
using System.Collections.Generic;
using static GdUnit4.Assertions;

namespace ADC.Tests
{
	[TestSuite]
	public class CameraTest
	{
		[TestCase]
		public void TestOnAddAndClearRoomsInList()
		{
			var controller = new CameraController();

			controller.OnAddRoomToList(new Rect2(0, 0, 10, 10));
			controller.OnAddRoomToList(new Rect2(20, 20, 15, 15));

			AssertThat(controller._rooms.Count).IsEqual(2);

			controller.OnClearRoomsInList();

			AssertThat(controller._rooms.Count).IsEqual(0);

			controller.QueueFree();
		}

		[TestCase]
		public void TestZoomDoesNotGoBelowMinimum()
		{
			var controller = new CameraController();
			controller._camera = new Camera2D();
			controller._camera.Zoom = new Vector2(0.5f, 0.5f);

			var wheelDownEvent = new InputEventMouseButton();
			wheelDownEvent.ButtonIndex = MouseButton.WheelDown;
			wheelDownEvent.Pressed = true;

			controller._Input(wheelDownEvent);

			AssertThat(controller._camera.Zoom.X).IsEqual(0.5f);
			AssertThat(controller._camera.Zoom.Y).IsEqual(0.5f);

			controller._camera.QueueFree();
			controller.QueueFree();
		}

		[TestCase]
		public void TestZoomIncreasesOnWheelUp()
		{
			var controller = new CameraController();
			controller._camera = new Camera2D();
			controller._camera.Zoom = new Vector2(1.0f, 1.0f);

			var wheelUpEvent = new InputEventMouseButton();
			wheelUpEvent.ButtonIndex = MouseButton.WheelUp;
			wheelUpEvent.Pressed = true;

			controller._Input(wheelUpEvent);

			AssertThat(controller._camera.Zoom.X).IsEqual(1.01f);
			AssertThat(controller._camera.Zoom.Y).IsEqual(1.01f);

			controller._camera.QueueFree();
			controller.QueueFree();
		}

		[TestCase]
		public void TestZoomDecreasesOnWheelDown()
		{
			var controller = new CameraController();
			controller._camera = new Camera2D();
			controller._camera.Zoom = new Vector2(1.0f, 1.0f);

			var wheelDownEvent = new InputEventMouseButton();
			wheelDownEvent.ButtonIndex = MouseButton.WheelDown;
			wheelDownEvent.Pressed = true;

			controller._Input(wheelDownEvent);

			AssertThat(controller._camera.Zoom.X).IsEqual(0.99f);
			AssertThat(controller._camera.Zoom.Y).IsEqual(0.99f);

			controller._camera.QueueFree();
			controller.QueueFree();
		}

		[TestCase]
		public void TestZoomIgnoresUnrelatedInput()
		{
			var controller = new CameraController();
			controller._camera = new Camera2D();
			controller._camera.Zoom = new Vector2(1.0f, 1.0f);

			var keyEvent = new InputEventKey();
			keyEvent.Keycode = Key.Space;
			keyEvent.Pressed = true;

			controller._Input(keyEvent);

			AssertThat(controller._camera.Zoom.X).IsEqual(1.0f);
			AssertThat(controller._camera.Zoom.Y).IsEqual(1.0f);

			controller._camera.QueueFree();
			controller.QueueFree();
		}

		[TestCase]
		public void TestZoomDoesNotGoAboveMaximum()
		{
			var controller = new CameraController();
			controller._camera = new Camera2D();
			controller._camera.Zoom = new Vector2(3.0f, 3.0f);

			var wheelUpEvent = new InputEventMouseButton();
			wheelUpEvent.ButtonIndex = MouseButton.WheelUp;
			wheelUpEvent.Pressed = true;

			controller._Input(wheelUpEvent);

			AssertThat(controller._camera.Zoom.X).IsEqual(3.0f);
			AssertThat(controller._camera.Zoom.Y).IsEqual(3.0f);

			controller._camera.QueueFree();
			controller.QueueFree();
		}

		[TestCase]
		public void TestCameraFloat()
		{
			
		}
	}
}
