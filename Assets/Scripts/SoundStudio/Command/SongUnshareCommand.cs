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
	public class SongUnshareCommand : EventCommand
	{
		private SongVO songVo;

		[Inject]
		public IMWSClientService MWSClientService
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
			songVo = (base.evt.data as SongVO);
			if (songVo == null)
			{
				throw new InvalidOperationException("The song vo was null");
			}
			if (songVo.HasServerID)
			{
				base.dispatcher.AddListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_SUCCESS, OnComplete);
				base.dispatcher.AddListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnCompleteFailed);
				MWSClientService.UpdateShareStateOfSoundStudioTrack(songVo.serverID, TrackShareState.NOT_SHARED);
			}
		}

		private void RemoveListeners()
		{
			base.dispatcher.RemoveListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_SUCCESS, OnComplete);
			base.dispatcher.RemoveListener(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, OnCompleteFailed);
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
				songVo.isShared = false;
				base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songVo);
				songVo.dispatchShareEvent();
			}
			finally
			{
				Release();
			}
		}

		private void dispatchConnectionError()
		{
			List<object> list = new List<object>();
			list.Add(songVo);
			base.dispatcher.Dispatch(ErrorEvent.ERROR, new ErrorPayload("505", new List<object>
			{
				songVo
			}));
		}
	}
}
