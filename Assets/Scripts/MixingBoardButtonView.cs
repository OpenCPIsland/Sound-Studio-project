using SoundStudio.Model;
using strange.extensions.mediation.impl;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MixingBoardButtonView : View
{
	public delegate void SoundButtonPressed(SoundVO sound);

	private SoundVO sound;

	private Sprite onImage;

	private Sprite offImage;

	private Image image;

	public int groupID;

	public int id;

	public SoundVO Sound
	{
		get
		{
			return sound;
		}
		set
		{
			if (sound != null)
			{
				sound.OnSoundStateUpdated -= OnSoundPlayStateChanged;
			}
			sound = value;
			sound.OnSoundStateUpdated += OnSoundPlayStateChanged;
			onImage = LoadImageForGenre(sound.buttonImageOn);
			offImage = LoadImageForGenre(sound.buttonImageOff);
			image.sprite = offImage;
		}
	}

	public event SoundButtonPressed OnSoundButtonPressed;

	protected override void Awake()
	{
		base.Awake();
		image = base.gameObject.GetComponent<Image>();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (sound != null)
		{
			sound.OnSoundStateUpdated -= OnSoundPlayStateChanged;
		}
	}

	private Sprite LoadImageForGenre(string path)
	{
		Sprite result = null;
		try
		{
			result = Resources.Load<Sprite>(path);
			return result;
		}
		catch (InvalidCastException)
		{
			return result;
		}
	}

	public void OnClick()
	{
		if (this.OnSoundButtonPressed != null)
		{
			this.OnSoundButtonPressed(sound);
		}
	}

	public void OnSoundPlayStateChanged(SoundVO sound)
	{
		if (this.sound.IsLoop() && this.sound.IsPlaying())
		{
			image.sprite = onImage;
		}
		else
		{
			image.sprite = offImage;
		}
	}
}
