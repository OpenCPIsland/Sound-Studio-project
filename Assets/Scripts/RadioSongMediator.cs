using SoundStudio;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class RadioSongMediator : EventMediator
{
	private RadioSongVO radioSong;

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	[Inject]
	public RadioSongView view
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		view.dispatcher.AddListener("RADIO_PLAY_EVENT", View_Play_Handler);
		view.dispatcher.AddListener("RADIO_STOP_EVENT", View_Stop_Handler);
		radioSong = view.radioSong;
		radioSong.songVO.SongPlayEvent += Song_Play_handler;
		radioSong.songVO.SongStopEvent += Song_Stop_Handler;
		view.songProgressView.LoadSong(radioSong.songVO);
		ShowPaperDoll();
		LoadBackground();
		view.ShowLoading();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("RADIO_PLAY_EVENT", View_Play_Handler);
		view.dispatcher.RemoveListener("RADIO_STOP_EVENT", View_Stop_Handler);
		radioSong.songVO.SongPlayEvent -= Song_Play_handler;
		radioSong.songVO.SongStopEvent -= Song_Stop_Handler;
	}

	public void OnDestroy()
	{
		OnRemove();
	}

	public void View_Play_Handler(IEvent evt)
	{
		base.dispatcher.Dispatch(SongPlayEvent.SONG_PLAY, radioSong.songVO);
		base.dispatcher.Dispatch(SoundStudioEvent.RADIO_SONG_PLAY, radioSong.songVO);
	}

	public void View_Stop_Handler(IEvent evt)
	{
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
	}

	private void Song_Stop_Handler()
	{
		view.Song_Stop_Handler();
	}

	private void Song_Play_handler()
	{
		view.Song_Play_Handler();
		view.HideLoading();
	}

	private void ShowPaperDoll()
	{
		Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		texture2D.LoadImage(radioSong.paperDollImageRaw);
		Sprite paperDoll = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0f, 0f));
		view.ShowPaperDoll(paperDoll);
	}

	private void LoadBackground()
	{
		switch (radioSong.songVO.GenreID)
		{
		case 2:
			view.bgImage.color = Utils.HexToColor("aa62ef");
			break;
		case 0:
			view.bgImage.color = Utils.HexToColor("e83a99");
			break;
		case 1:
			view.bgImage.color = Utils.HexToColor("aa62ef");
			break;
		case 4:
			view.bgImage.color = Utils.HexToColor("1cb795");
			break;
		case 5:
			view.bgImage.color = Utils.HexToColor("eaaf4b");
			break;
		case 3:
			view.bgImage.color = Utils.HexToColor("a0f11c");
			break;
		}
	}
}
