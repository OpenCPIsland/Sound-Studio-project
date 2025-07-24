using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;

namespace SoundStudio.Command
{
	public class SongShareCommand : EventCommand
	{
		private SongVO songVo;

		[Inject]
		public IMWSClientService mwsClientService
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

		public override void Execute()
		{
			Retain();
			songVo = (base.evt.data as SongVO);
			if (songVo == null)
			{
				throw new InvalidOperationException("The song vo was null");
			}
			if (songVo.HasServerID)
			{
				base.dispatcher.AddListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_SUCCESS, OnComplete);
				base.dispatcher.AddListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnCompleteFailed);
				mwsClientService.UpdateShareStateOfSoundStudioTrack(songVo.serverID, TrackShareState.SHARED);
			}
		}

		private void OnCompleteFailed(IEvent payload)
		{
			RemoveListeners();
			dispatchConnectionError();
			Release();
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				RemoveListeners();
				unshareAllSongs();
				songVo.isShared = true;
				base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songVo);
			}
			finally
			{
				Release();
			}
		}

		private void RemoveListeners()
		{
			base.dispatcher.RemoveListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_SUCCESS, OnComplete);
			base.dispatcher.RemoveListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnCompleteFailed);
		}

		private void unshareAllSongs()
		{
			foreach (SongVO song in application.songData.SongList)
			{
				if (song.isShared)
				{
					song.isShared = false;
					song.dispatchShareEvent();
					base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, song);
				}
			}
		}

		private void dispatchConnectionError()
		{
			List<object> list = new List<object>();
			list.Add(songVo);
			base.dispatcher.Dispatch(ErrorEvent.ERROR, new ErrorPayload("504", list));
		}
	}
}
