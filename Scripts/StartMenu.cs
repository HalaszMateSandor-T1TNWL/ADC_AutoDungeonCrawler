using Godot;
using System;

public partial class StartMenu : Control
{
	[Signal]
	public delegate void MainMenuActionEventHandler();

	private PopupPanel settings;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		settings = GetNode<PopupPanel>("SettingsPopup");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnNewGameButtonPressed()
	{
		EmitSignal(SignalName.MainMenuAction,(int) UserInterfaceActions.NewGame);
		GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
	}

	public void OnLoadGameButtonPressed()
	{
		EmitSignal(SignalName.MainMenuAction,(int) UserInterfaceActions.LoadGame);
	}

	public void OnSettingsButtonPressed()
	{
		settings.Popup();
		EmitSignal(SignalName.MainMenuAction,(int) UserInterfaceActions.MainMenuSettings);
	}

	public void OnQuitGameButtonPressed()
	{
		EmitSignal(SignalName.MainMenuAction,(int) UserInterfaceActions.QuitGame);
		GetTree().Quit();
	}

}
