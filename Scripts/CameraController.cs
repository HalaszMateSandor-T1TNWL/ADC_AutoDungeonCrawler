using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class CameraController : Node2D
{
	[Export] public float moveSpeed = 200.0f;
	[Export] public float panningSpeed = 1.0f;
	private Variant _viewportWidth = ProjectSettings.GetSetting("display/window/size/viewport_width");
	private Variant _viewportHeight = ProjectSettings.GetSetting("display/window/size/viewport_height");

	private Vector2 _motionVector;
	private Vector2 _roomFeed;
	public List<Rect2> _rooms = new List<Rect2>(); //made it public for testing

	public Camera2D _camera; //same here
	private TileMapLayer _tilemap;

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>($"Camera2D");
		_tilemap = GetNode<TileMapLayer>($"../TileMapLayer");
		_motionVector = GetViewport().GetMousePosition();
	}

	public override void _Process(double delta)
	{
		_roomFeed = _rooms.First().GetCenter();
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

			this.GlobalPosition = this.GlobalPosition.MoveToward(_motionVector, (float)delta * moveSpeed);
		}
		else
			/* Use this for the camera to pan towards a room */
			//this.GlobalPosition = this.GlobalPosition.MoveToward(_tilemap.MapToLocal((Vector2I)_roomFeed), (float)delta * 500.0f);
			this.GlobalPosition = new Vector2(_tilemap.MapToLocal((Vector2I)_roomFeed).X, _tilemap.MapToLocal((Vector2I)_roomFeed).Y);
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
				//added this so can't zoom forever (makes for a good test :))
                case MouseButton.WheelUp:
                    if (_camera.Zoom >= new Vector2(3.0f, 3.0f))
                    {
                        break;
                    }
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
	
	public void OnClearRoomsInList()
	{
		_rooms.Clear();
	}

	public void OnAddRoomToList(Rect2 room)
	{
		_rooms.Add(room);
	}
}
