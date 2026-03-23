using Godot;
using System;

public partial class UserInterface : Control
{
	[Signal]
	public delegate void UIActionEventHandler(int action);
	private PopupPanel shop;
	private PopupPanel resetShop;
	
	public override void _Ready()
	{
		shop = GetNode<PopupPanel>("Shop");
		resetShop = GetNode<PopupPanel>("ResetLockPopupShopButton");
	}

	public override void _Process(double delta)
	{
	}
	private void _On_Open_Close_Shop_Button_Pressed()
	{
		shop.Popup();
		resetShop.Popup();
	}
	private void _On_Buy_Level_Button_And_Level_Viewer_Pressed()
	{
		EmitSignal(SignalName.UIAction,(int)UserInterfaceActions.BuyLevel );
	}
	private void _On_Start_Next_Level_Button_Pressed()
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.StartNextLevel);
	}
	private void _On_Reset_Shop_Button_Pressed()
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.ResetShop);
	}
	private void _On_Lock_Shop_Button_Toggled(bool toggledOn)
	{
		EmitSignal(SignalName.UIAction,(int) UserInterfaceActions.LockShop);
	}
	
}
