using Godot;
using System;

public partial class TileMapLayer : Godot.TileMapLayer
{
	private PackedScene _sceneEnemy;
	private PackedScene _scenePlayer;
	
	private Node2D _cameraController;
	private Camera2D _camera;
	private Label _debugCoords;
	
	private float _viewportWidth;
	private float _viewportHeight;
	
	
	public override void _Ready()
	{
		_sceneEnemy = GD.Load<PackedScene>("res://Scenes/enemy.tscn");
		_scenePlayer = GD.Load<PackedScene>("res://Scenes/Player.tscn");
		
		_camera = GetNode<Camera2D>($"../CameraController/Camera2D");
		_cameraController = GetNode<Node2D>($"../CameraController");
		_debugCoords = GetNode<Label>($"Label");
		
		_viewportWidth = (float)ProjectSettings.GetSetting("display/window/viewport_width");
		_viewportHeight = (float)ProjectSettings.GetSetting("display/window/viewport_height");
		
	}
	
	public override void _Process(double delta)
	{
		Vector2 tile = LocalToMap(GetGlobalMousePosition());
		_debugCoords.Position = GetGlobalMousePosition() + new Vector2(5,5);
		_debugCoords.Text = tile.ToString();
	}
	
	public override void _Input(InputEvent @event)
	{
		Node2D instanceEnemy = (Node2D)_sceneEnemy.Instantiate();
		Node2D instancePlayer = (Node2D)_scenePlayer.Instantiate();
		
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			switch(mouseEvent.ButtonIndex)
			{
				case MouseButton.Left:
					instanceEnemy.Position = ((mouseEvent.GetGlobalPosition() - new Vector2(_viewportWidth/2, _viewportHeight/2)) / _camera.Zoom) + _cameraController.Position;
					GetNode<Node2D>("/root/Main").AddChild(instanceEnemy);
					break;
				case MouseButton.Right:
					instancePlayer.Position = ((mouseEvent.GetGlobalPosition() - new Vector2(_viewportWidth/2, _viewportHeight/2)) / _camera.Zoom) + _cameraController.Position;
					GetNode<Node2D>("/root/Main").AddChild(instancePlayer);
					break;
					
			}
		}
	}
}
