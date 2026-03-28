using Godot;
using System;

public partial class SoundSettings : PopupPanel
{
	private int masterBus;
	private int musicBus;
	private int sfxBus;

	private HSlider masterSlider;
	private HSlider musicSlider;
	private HSlider sfxSlider;
	public override void _Ready()
	{
		masterBus = AudioServer.GetBusIndex("Master");
		musicBus = AudioServer.GetBusIndex("Music");
		sfxBus = AudioServer.GetBusIndex("SFX");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private void OnMasterVolumeSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(masterBus, Mathf.LinearToDb(value));
	}

	private void OnMusicVolumeSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(musicBus, Mathf.LinearToDb(value));
	}

	private void OnSFXVolumeSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(sfxBus, Mathf.LinearToDb(value));
	}


}
