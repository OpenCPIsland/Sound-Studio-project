using DevonLocalization.Core;
using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;

public class SongListMediator : EventMediator
{
	[Inject]
	public SongListView view
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
		view.dispatcher.AddListener("new_click", onNewClicked);
		view.dispatcher.AddListener("back_click", onBackClicked);
		base.dispatcher.Dispatch(SoundStudioEvent.RESUME_DMO_ANALYTICS);
		base.dispatcher.AddListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnShareFailed);
		base.dispatcher.AddListener(SongDeleteEvent.SONG_DELETE, OnDeleteSong);
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_GENRE_AUDIO_START, OnGenreLoadStart);
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnGenreAudioManagerLoadComplete);
		base.dispatcher.AddListener(SongPlayEvent.SONG_PLAY, OnSongPlay);
		HandleMySongsState();
	}

	private void OnDeleteSong(IEvent payload)
	{
		UpdateSongCountInView();
	}

	private void HandleMySongsState()
	{
		if (application.currentPlayer.MySongsState == MySongsStatus.LOADING)
		{
			base.dispatcher.AddListener(SoundStudioEvent.CONSOLIDATE_TRACKS_COMPLETE, OnConsolidateTracksComplete);
			base.dispatcher.AddListener(SoundStudioEvent.CONSOLIDATE_TRACKS_FAILED, OnConsolidateTracksFailed);
			view.ShowLoadingPanel();
		}
		else
		{
			populateSongList();
		}
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("new_click", onNewClicked);
		view.dispatcher.RemoveListener("back_click", onBackClicked);
		base.dispatcher.RemoveListener(SoundStudioEvent.CONSOLIDATE_TRACKS_COMPLETE, OnConsolidateTracksComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.CONSOLIDATE_TRACKS_FAILED, OnConsolidateTracksFailed);
		base.dispatcher.RemoveListener(SongDeleteEvent.SONG_DELETE, OnDeleteSong);
		base.dispatcher.RemoveListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnShareFailed);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_GENRE_AUDIO_START, OnGenreLoadStart);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnGenreAudioManagerLoadComplete);
		base.dispatcher.RemoveListener(SongPlayEvent.SONG_PLAY, OnSongPlay);
		foreach (SongVO song in application.songData.SongList)
		{
			song.SongSyncEvent -= song_Sync_Handler;
		}
	}

	public void OnDestroy()
	{
		OnRemove();
	}

	private void OnShareFailed()
	{
		base.dispatcher.Dispatch(SoundStudioEvent.SHOW_POPUP, new ShowPopupCommandPayload(view.CanvasObject, "Prefabs/ShareNoWifiPrefab"));
	}

	private void populateSongList()
	{
		List<SongVO> list = new List<SongVO>();
		list.AddRange(application.songData.SongList);
		if (list.Count == 0)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.SHOW_NO_SONGS_POPUP, view.CanvasObject);
		}
		list.Sort((SongVO x, SongVO y) => y.timeStamp.CompareTo(x.timeStamp));
		foreach (SongVO item in list)
		{
			if (!item.cachedDelete)
			{
				view.addSong(item);
				item.SongSyncEvent += song_Sync_Handler;
			}
		}
		UpdateSongCountInView();
	}

	private void song_Sync_Handler()
	{
		UpdateSongCountInView();
	}

	private void UpdateSongCountInView()
	{
		view.SyncCountText.text = Localizer.Instance.GetTokenTranslation("soundstudio.startscreen.songs") + ": " + application.songData.SongList.Count.ToString() + " / " + 24;
	}

	private void onNewClicked()
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Genres);
	}

	private void onBackClicked()
	{
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.StartScreen);
	}

	private void OnConsolidateTracksComplete()
	{
		view.HideLoadingPanel();
		populateSongList();
	}

	private void OnConsolidateTracksFailed()
	{
		view.HideLoadingPanel();
		populateSongList();
	}

	private void OnSongPlay()
	{
	}

	private void OnGenreLoadStart(IEvent payload)
	{
		view.ShowSongLoadingPanel();
	}

	private void OnGenreAudioManagerLoadComplete()
	{
		view.HideSongLoadingPanel();
	}
}
