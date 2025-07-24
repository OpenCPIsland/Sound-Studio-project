using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RadioSongView : EventView
{
	internal const string PLAY_EVENT = "RADIO_PLAY_EVENT";

	internal const string STOP_EVENT = "RADIO_STOP_EVENT";

	internal const string RADIO_SONG_COMPLETE_EVENT = "RADIO_SONG_COMPLETE_EVENT";

	internal const string SONG_LOAD_START = "SONG_LOAD_START";

	internal const string SONG_LOAD_COMPLETE = "SONG_LOAD_COMPLETE";

	internal const string TRIGGER_EXIT_LEFT = "SlideOutLeft";

	internal const string TRIGGER_EXIT_RIGHT = "SlideOutRight";

	internal const string TRIGGER_ENTER_LEFT = "SlideInLeft";

	internal const string TRIGGER_ENTER_RIGHT = "SlideInRight";

	internal const string TRIGGER_PLAY = "StartSong";

	internal const string TRIGGER_DEFAULT = "Default";

	public Text nameText;

	public GameObject buttonPlay;

	public GameObject buttonStop;

	public Image paperDollImage;

	public SongProgressView songProgressView;

	public GameObject loadingPanel;

	public GameObject lowerPanel;

	public Image bgImage;

	public Animator animator;

	private bool isUserStopped;

	public RadioSongVO radioSong;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher contextDispatcher
	{
		get;
		set;
	}

	public void loadData(RadioSongVO radioSong)
	{
		this.radioSong = radioSong;
		nameText.text = radioSong.soundStudioRadioTrackData.playerDisplayName;
	}

	public void playButton_Click_Handler()
	{
		base.dispatcher.Dispatch("RADIO_PLAY_EVENT");
	}

	public void stopButton_Click_Handler()
	{
		isUserStopped = true;
		base.dispatcher.Dispatch("RADIO_STOP_EVENT");
	}

	public void Song_Stop_Handler()
	{
		buttonPlay.SetActive(value: true);
		buttonStop.SetActive(value: false);
		if (isUserStopped)
		{
			isUserStopped = false;
		}
		else
		{
			contextDispatcher.Dispatch("RADIO_SONG_COMPLETE_EVENT", this);
		}
	}

	public void Song_Play_Handler()
	{
		buttonPlay.SetActive(value: false);
		buttonStop.SetActive(value: true);
	}

	public void ShowPaperDoll(Sprite paperDoll)
	{
		paperDollImage.sprite = paperDoll;
		paperDollImage.enabled = true;
	}

	public void AnimateEnterLeft()
	{
		animator.SetTrigger("SlideInLeft");
	}

	public void AnimateEnterRight()
	{
		animator.SetTrigger("SlideInRight");
	}

	public void AnimateExitLeft()
	{
		animator.SetTrigger("SlideOutLeft");
	}

	public void AnimateExitRight()
	{
		animator.SetTrigger("SlideOutRight");
	}

	public void AnimatePlay()
	{
		animator.SetTrigger("StartSong");
	}

	public void AnimateDefault()
	{
		animator.SetTrigger("Default");
	}

	public void ShowLoading()
	{
		loadingPanel.SetActive(value: true);
		lowerPanel.SetActive(value: false);
		contextDispatcher.Dispatch("SONG_LOAD_START");
	}

	public void HideLoading()
	{
		loadingPanel.SetActive(value: false);
		lowerPanel.SetActive(value: true);
		contextDispatcher.Dispatch("SONG_LOAD_COMPLETE");
	}
}
