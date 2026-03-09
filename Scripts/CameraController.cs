using Godot;
using System;

public partial class CameraController : Node2D
{
	[Export] public float moveSpeed = 100.0f;
	[Export] public float panningSpeed = 1.0f;
	private Variant _viewportWidth = ProjectSettings.GetSetting("display/window/size/viewport_width");
	private Variant _viewportHeight = ProjectSettings.GetSetting("display/window/size/viewport_height");

	private Vector2 _motionVector;

	private Camera2D _camera;

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>($"Camera2D");
		_motionVector = Vector2.Zero;
	}


	public override void _PhysicsProcess(double delta)
	{
		if(Input.MouseMode == Input.MouseModeEnum.Confined)
		{
			Vector2 currentMousePosition = GetViewport().GetMousePosition();
			if(currentMousePosition.X >= (float)_viewportWidth - 20.0f)
			{
				_motionVector.X += panningSpeed;
			}
			else if(currentMousePosition.Y >= (float)_viewportHeight - 20.0f)
			{
				_motionVector.Y += panningSpeed;
			}

			if(currentMousePosition.X >= (float)_viewportWidth - 20.0f && currentMousePosition.Y >= (float)_viewportHeight - 20.0f)
			{
				_motionVector.X += panningSpeed;
				_motionVector.Y += panningSpeed;
			}

			if(currentMousePosition.X <= 0.0f + 20.0f)
			{
				_motionVector.X -= panningSpeed;
			}
			else if(currentMousePosition.Y <= 0.0f + 20.0f)
			{
				_motionVector.Y -= panningSpeed;
			}

			if(currentMousePosition.X <= 0.0f + 20.0f && currentMousePosition.Y <= 0.0f + 20.0f)
			{
				_motionVector.X -= panningSpeed;
				_motionVector.Y -= panningSpeed;
			}   
		}
		this.GlobalPosition = this.GlobalPosition.MoveToward(_motionVector, (float)delta * moveSpeed);
	}

	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if(@event is InputEventMouseButton button)
		{
			switch(button.ButtonIndex)
			{
				case MouseButton.Middle:
					Input.MouseMode = Input.MouseModeEnum.Confined;
					break;
				case MouseButton.WheelUp:
					_camera.Zoom += new Vector2(0.01f, 0.01f);
					break;
				case MouseButton.WheelDown:
					if(_camera.Zoom <= new Vector2(0.5f, 0.5f))
					{
						break;
					}
					_camera.Zoom -= new Vector2(0.01f, 0.01f);
					break;
			}
		}
	}


}
