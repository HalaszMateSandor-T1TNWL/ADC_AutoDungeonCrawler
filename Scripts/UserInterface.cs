using Godot;
using System;

public partial class UserInterface : Control
{
	[Signal]
	public delegate void UIActionEventHandler(int action);
	private PopupPanel shop;
	private PopupPanel resetShop;
	private PopupPanel settings;
	private PopupPanel savePopup;
	private PopupPanel loadPopup;
	private PopupPanel soundSettings; 
	
	public override void _Ready()
	{
		shop = GetNode<PopupPanel>("Shop");
		resetShop = GetNode<PopupPanel>("ResetLockPopupShopButton");
		settings = GetNode<PopupPanel>("SettingsPopup");
		savePopup = GetNode<PopupPanel>("SavePopup");
		loadPopup = GetNode<PopupPanel>("LoadPopup");
		soundSettings = GetNode<PopupPanel>("SoundPopup");
		
		
	}

	public override void _Process(double delta)
	{
	}

	private void OnOpenCloseShopButtonPressed()
	{
		shop.Popup();
		resetShop.Popup();
	}

	private void OnSettingTexturesButtonPressed()
	{
		settings.Popup();
	}

	private void OnSettingsSaveButtonPressed()
	{
		savePopup.Popup();
		settings.Hide();
	}

	private void OnSettingsLoadButtonPressed()
	{
		loadPopup.Popup();
		settings.Hide();
	}

	private void OnSettingsPopupExitPressed()
	{
		settings.Hide();
	}

	private void OnSettingsSoundButtonPressed()
	{
		settings.Hide();
		soundSettings.Show();

	}

	private void OnSaveReturnToSettingsButtonPressed()
	{
		savePopup.Hide();
		settings.Popup();
	}

	private void OnLoadReturnToSettingsButtonPressed()
	{
		loadPopup.Hide();
		settings.Popup();
	}

	private void OnSoundSettingsExitButtonPressed()
	{
		soundSettings.Hide();
		settings.Popup();
	}

	private void OnBuyLevelButtonAndLevelViewerPressed()
	{
		EmitSignal(SignalName.UIAction,(int)UserInterfaceActions.BuyLevel );
	}

	private void OnStartNextLevelButtonPressed()
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.StartNextLevel);
	}

	private void OnResetShopButtonPressed()
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.ResetShop);
	}

	private void OnLockShopButtonToggled(bool toggledOn)
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.LockShop);
	}

	private void OnSettingsMainMenuButtonPressed()
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.MainMenu);
		GetTree().ChangeSceneToFile("res://Scenes/StartMenu.tscn");
	}

	//Yes i am a SIMP (Sad Internet Meme Person)
	
}
