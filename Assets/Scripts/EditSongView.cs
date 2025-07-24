using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class EditSongView : View
{
	internal const string PLAY_EVENT = "play_song";

	internal const string STOP_EVENT = "stop_song";

	internal const string CANCEL_EVENT = "cancel_click";

	internal const string SAVE_EVENT = "save_click";

	public InputField inputText;

	public GameObject buttonPlay;

	public GameObject buttonStop;

	public Button buttonSave;

	public SongProgressView songProgressView;

	public Button invisibleButton;

	public SongVO song;

	[Inject]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		invisibleButton.gameObject.SetActive(value: true);
		HardwareBackButtonDispatcher.SetTargetClickHandler(invisibleButton, visible: false);
	}

	public void Update()
	{
		if (inputText.text.Length == 0 && buttonSave.enabled)
		{
			buttonSave.enabled = false;
			ColorBlock colors = buttonSave.colors;
			Color normalColor = buttonSave.colors.normalColor;
			normalColor.a = 0.2f;
			colors.normalColor = normalColor;
			buttonSave.colors = colors;
		}
		else if (inputText.text.Length > 0 && !buttonSave.enabled)
		{
			buttonSave.enabled = true;
			ColorBlock colors2 = buttonSave.colors;
			Color normalColor2 = buttonSave.colors.normalColor;
			normalColor2.a = 1f;
			colors2.normalColor = normalColor2;
			buttonSave.colors = colors2;
		}
	}

	public void loadSong(SongVO song)
	{
		this.song = song;
		songProgressView.LoadSong(song);
	}

	public void setSongName(string songName)
	{
		inputText.text = songName;
	}

	public void cancel_Click_Handler()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		dispatcher.Dispatch("cancel_click");
	}

	public void save_Click_Handler()
	{
		dispatcher.Dispatch("save_click", song);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
	}

	public void play_Click_Handler()
	{
		dispatcher.Dispatch("play_song", song);
	}

	public void stop_Click_Handler()
	{
		dispatcher.Dispatch("stop_song");
	}

	public void song_Complete_Handler()
	{
		buttonPlay.SetActive(value: true);
		buttonStop.SetActive(value: false);
	}

	public void Song_Play_Handler()
	{
		buttonPlay.SetActive(value: false);
		buttonStop.SetActive(value: true);
	}
}
