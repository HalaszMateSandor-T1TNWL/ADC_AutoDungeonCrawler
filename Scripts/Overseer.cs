using Godot;

public partial class Overseer : Node2D
{
	public TileMapLayer tilemap;
	public Eye eye;
	public override void _Ready()
	{
		tilemap = GetNodeOrNull<TileMapLayer>($"TileMapLayer");
		eye = GetNodeOrNull<Eye>($"Eye");
		
		UnitNavigation.Instance.Initialize(eye, tilemap);
	}
}
