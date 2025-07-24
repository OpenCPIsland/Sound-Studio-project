using DevonLocalization.Core;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System;
using UnityEngine;

public class EditSongMediator : EventMediator
{
	private const char BAD_WORD_DELIMITER = ',';

	private const string BAD_WORDS_TOKEN = "soundstudio.common.badwordlist";

	[Inject]
	public EditSongView view
	{
		get;
		set;
	}

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		view.dispatcher.AddListener("play_song", view_Play_Handler);
		view.dispatcher.AddListener("stop_song", view_Stop_Handler);
		view.dispatcher.AddListener("cancel_click", view_Cancel_Handler);
		view.dispatcher.AddListener("save_click", view_Save_Handler);
		view.song.SongPlayEvent += Song_Play_Handler;
		view.song.SongStopEvent += Song_Stop_Handler;
		setSongName();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("play_song", view_Play_Handler);
		view.dispatcher.RemoveListener("stop_song", view_Stop_Handler);
		view.dispatcher.RemoveListener("cancel_click", view_Cancel_Handler);
		view.dispatcher.RemoveListener("save_click", view_Save_Handler);
		view.song.SongPlayEvent -= Song_Play_Handler;
		view.song.SongStopEvent -= Song_Stop_Handler;
	}

	public void OnDestroy()
	{
		OnRemove();
	}

	private void setSongName()
	{
		if (view.song.songName == "temp")
		{
			view.setSongName(suggestName(view.song));
		}
		else
		{
			view.setSongName(view.song.songName);
		}
	}

	private string suggestName(SongVO loadedSong)
	{
		int num = 1;
		foreach (SongVO song in application.songData.SongList)
		{
			if (song.GenreID == loadedSong.GenreID)
			{
				num++;
			}
		}
		GenreVO genreByID = application.genreData.getGenreByID(loadedSong.GenreID);
		return Localizer.Instance.GetTokenTranslation("soundstudio.genres." + genreByID.name.ToLower()) + " " + num.ToString();
	}

	public void view_Play_Handler(IEvent evt)
	{
		SongVO data = (SongVO)evt.data;
		base.dispatcher.Dispatch(SongPlayEvent.SONG_PLAY, data);
	}

	public void view_Stop_Handler(IEvent evt)
	{
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
	}

	public void view_Cancel_Handler(IEvent evt)
	{
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		base.dispatcher.Dispatch(SoundStudioEvent.SAVE_SONG_DIALOG_CANCEL);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void view_Save_Handler(IEvent evt)
	{
		SongVO songVO = (SongVO)evt.data;
		songVO.songName = view.inputText.text;
		string[] array = Localizer.Instance.GetTokenTranslation("soundstudio.common.badwordlist").Replace(" ", string.Empty).Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (songVO.songName.ToLower().IndexOf(text.ToLower()) > -1)
			{
				view.inputText.text = suggestName(songVO);
				return;
			}
		}
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		base.dispatcher.AddListener(SongSaveEvent.SONG_SAVE_SUCCESS, OnSongSaveSuccess);
		base.dispatcher.Dispatch(SoundStudioEvent.PAUSE_RETRY_SERVICE);
		base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songVO);
		if (application.CurrentLevel.Equals(SceneNames.MixingBoard.ToString()))
		{
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Songs);
		}
		else
		{
			songVO.dispatchNameEvent();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnSongSaveSuccess(IEvent payload)
	{
		base.dispatcher.RemoveListener(SongSaveEvent.SONG_SAVE_SUCCESS, OnSongSaveSuccess);
		SongSyncVO songSyncVO = payload.data as SongSyncVO;
		if (songSyncVO == null)
		{
			throw new InvalidOperationException("The payload for the song save success event was null.");
		}
		base.dispatcher.Dispatch(SongSyncEvent.SONG_SYNC, songSyncVO);
	}

	private void Song_Play_Handler()
	{
		view.Song_Play_Handler();
	}

	private void Song_Stop_Handler()
	{
		view.song_Complete_Handler();
	}
}
