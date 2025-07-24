using SoundStudio;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SongListItemMediator : EventMediator
{
	[Inject]
	public SongListItemView view
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

	[Inject]
	public SongPlayer songPlayer
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		view.dispatcher.AddListener("DELETE_EVENT", view_Delete_Handler);
		view.dispatcher.AddListener("RENAME_EVENT", view_Rename_Handler);
		view.dispatcher.AddListener("PLAY_EVENT", view_Play_Handler);
		view.dispatcher.AddListener("STOP_EVENT", view_Stop_Handler);
		view.dispatcher.AddListener("SHARE_EVENT", OnViewShare);
		view.dispatcher.AddListener("UNSHARE_EVENT", OnViewUnshare);
		if (view.song != null)
		{
			view.song.SongSyncEvent += song_Sync_Handler;
			view.song.SongNameEvent += song_Name_Handler;
			view.song.SongStopEvent += Song_Stop_Handler;
			view.song.SongPlayEvent += Song_Play_handler;
			view.song.SongDeleteEvent += song_Delete_Handler;
			view.song.SongShareEvent += song_Shared_Handler;
		}
		base.dispatcher.AddListener(ErrorEvent.ERROR, onError);
		base.dispatcher.AddListener(SoundStudioEvent.SHOW_UNSHARED_STATE_IN_VIEW, OnShowUnsharedStateInView);
		if (view.song.HasServerID)
		{
			view.ShowSongSynced();
		}
		else
		{
			view.ShowSongUnsynced();
		}
		if (view.song.isShared)
		{
			view.showSongShared();
		}
		else
		{
			view.showSongUnshared();
		}
		LoadBackground();
	}

	private void OnShowUnsharedStateInView(IEvent payload)
	{
		SongVO songVO = payload.data as SongVO;
		if (view.song.serverID != songVO.serverID)
		{
			view.showSongUnshared();
		}
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("DELETE_EVENT", view_Delete_Handler);
		view.dispatcher.RemoveListener("RENAME_EVENT", view_Rename_Handler);
		view.dispatcher.RemoveListener("PLAY_EVENT", view_Play_Handler);
		view.dispatcher.RemoveListener("STOP_EVENT", view_Stop_Handler);
		view.dispatcher.RemoveListener("SHARE_EVENT", OnViewShare);
		view.dispatcher.RemoveListener("UNSHARE_EVENT", OnViewUnshare);
		if (view.song != null)
		{
			view.song.SongSyncEvent -= song_Sync_Handler;
			view.song.SongNameEvent -= song_Name_Handler;
			view.song.SongStopEvent -= Song_Stop_Handler;
			view.song.SongPlayEvent -= Song_Play_handler;
			view.song.SongDeleteEvent -= song_Delete_Handler;
			view.song.SongShareEvent -= song_Shared_Handler;
		}
		base.dispatcher.RemoveListener(ErrorEvent.ERROR, onError);
		base.dispatcher.RemoveListener(SoundStudioEvent.SHOW_UNSHARED_STATE_IN_VIEW, OnShowUnsharedStateInView);
	}

	public void OnDestroy()
	{
		OnRemove();
	}

	public void view_Play_Handler(IEvent evt)
	{
		SongVO songVO = (SongVO)evt.data;
		if (songVO.Equals(view.song))
		{
			base.dispatcher.Dispatch(SongPlayEvent.SONG_PLAY, songVO);
		}
	}

	public void view_Stop_Handler(IEvent evt)
	{
		SongVO songVO = (SongVO)evt.data;
		if (songVO.Equals(view.song))
		{
			base.dispatcher.Dispatch(SongStopEvent.SONG_STOP, songVO);
		}
	}

	public void view_Delete_Handler(IEvent evt)
	{
		SongVO songVO = (SongVO)evt.data;
		if (songVO.Equals(view.song))
		{
			base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
			base.dispatcher.Dispatch(SongDeleteEvent.SONG_DELETE, songVO);
			song_Delete_Handler();
		}
	}

	public void view_Rename_Handler(IEvent evt)
	{
	}

	public void OnViewShare()
	{
		if (view != null && view.song != null && view.song.HasServerID)
		{
			view.showSongShared();
			ShowSharePromptFirstTime();
			base.dispatcher.Dispatch(SoundStudioEvent.SHOW_UNSHARED_STATE_IN_VIEW, view.song);
			base.dispatcher.Dispatch(SongShareEvent.SONG_SHARE, view.song);
		}
	}

	private void ShowSharePromptFirstTime()
	{
		if (Application.internetReachability != 0)
		{
			try
			{
				string @string = PlayerPrefs.GetString(application.currentPlayer.Swid + "share_prompt");
				if (string.IsNullOrEmpty(@string))
				{
					base.dispatcher.Dispatch(ShowShareConfirmEvent.SHOW_SHARE_CONFIRM, view.CanvasObject);
					PlayerPrefs.SetString(application.currentPlayer.Swid + "share_prompt", "yes");
				}
			}
			catch (PlayerPrefsException)
			{
			}
		}
	}

	public void OnViewUnshare()
	{
		view.showSongUnshared();
		base.dispatcher.Dispatch(SongUnshareEvent.SONG_UNSHARE, view.song);
	}

	private string parseSongData(string rawData)
	{
		return rawData.Substring(rawData.IndexOf(','));
	}

	private void song_Sync_Handler()
	{
		if (view.song.HasServerID)
		{
			view.ShowSongSynced();
		}
		else if (!view.song.HasServerID)
		{
			view.ShowSongUnsynced();
		}
	}

	private void Song_Stop_Handler()
	{
		view.Song_Stop_Handler();
	}

	private void Song_Play_handler()
	{
		view.Song_Play_Handler();
	}

	private void song_Name_Handler()
	{
		view.loadSong(view.song);
	}

	private void song_Delete_Handler()
	{
		if (songPlayer.CurrentSong == view.song)
		{
			base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void song_Shared_Handler()
	{
		if (!view.song.isShared)
		{
			view.showSongUnshared();
		}
	}

	private void onError(IEvent evt)
	{
		ErrorPayload errorPayload = evt.data as ErrorPayload;
		if (errorPayload != null)
		{
		}
		if (!view.song.isShared)
		{
			view.showSongUnshared();
		}
		else
		{
			view.showSongShared();
		}
	}

	private void LoadBackground()
	{
		switch (view.song.GenreID)
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
