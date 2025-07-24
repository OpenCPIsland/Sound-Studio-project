using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System.Collections.Generic;

public class PerformCachedActionsCommand : EventCommand
{
	private int cachedActionIndex;

	private List<SongVO> cachedActions;

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	public override void Execute()
	{
		if (application.songData != null)
		{
			FindUnSyncedActions(application.songData.SongList);
		}
	}

	private void FindUnSyncedActions(ICollection<SongVO> songs)
	{
		cachedActions = new List<SongVO>();
		foreach (SongVO song in songs)
		{
			if (song.cachedDelete || !song.HasServerID)
			{
				song.playerid = application.currentPlayer.ID;
				cachedActions.Add(song);
			}
		}
		cachedActionIndex = 0;
		if (cachedActions.Count > 0)
		{
			PerformCachedAction(cachedActions[cachedActionIndex]);
		}
		else
		{
			base.dispatcher.Dispatch(SoundStudioEvent.PERFORM_CACHED_ACTIONS_COMPLETE);
		}
	}

	private void PerformCachedAction(SongVO songVo)
	{
		base.dispatcher.AddListener(SoundStudioEvent.SONG_DELETE_COMPLETED, OnCachedActionComplete);
		base.dispatcher.AddListener(SoundStudioEvent.SONG_SYNC_COMPLETED, OnCachedActionComplete);
		if (songVo.cachedDelete)
		{
			base.dispatcher.Dispatch(SongDeleteEvent.SONG_DELETE, songVo);
		}
		else if (!songVo.HasServerID)
		{
			base.dispatcher.Dispatch(SongSyncEvent.SONG_SYNC, new SongSyncVO(songVo));
		}
	}

	private void OnCachedActionComplete()
	{
		base.dispatcher.RemoveListener(SoundStudioEvent.SONG_DELETE_COMPLETED, OnCachedActionComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.SONG_SYNC_COMPLETED, OnCachedActionComplete);
		cachedActionIndex++;
		if (cachedActionIndex > cachedActions.Count - 1)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.PERFORM_CACHED_ACTIONS_COMPLETE);
		}
		else
		{
			PerformCachedAction(cachedActions[cachedActionIndex]);
		}
	}
}
