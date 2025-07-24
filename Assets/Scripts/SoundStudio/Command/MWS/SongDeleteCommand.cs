using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoundStudio.Command.MWS
{
	public class SongDeleteCommand : EventCommand
	{
		private SongVO song;

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
			Retain();
			song = (base.evt.data as SongVO);
			if (song == null)
			{
				throw new ArgumentNullException("Song VO cannot be null");
			}
			if (song.HasServerID)
			{
				base.dispatcher.AddListener(MWSEvent.DELETE_MY_TRACK_SUCCESS, OnMWSDeleteTrackSuccess);
				base.dispatcher.AddListener(MWSEvent.DELETE_MY_TRACK_FAILED, OnMWSDeleteTrackFailed);
				MWSClientService.DeleteMySoundStudioTrack(song.serverID);
			}
			else
			{
				DeleteLocally();
			}
		}

		private void DeleteLocally()
		{
			if (song != null && File.Exists(application.CurrentPlayerDataDirectory + song.FileName))
			{
				try
				{
					File.Delete(application.CurrentPlayerDataDirectory + song.FileName);
					application.songData.Remove(song);
					song.dispatchDeleteEvent();
					song.dispatchSyncEvent();
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
			}
		}

		private void FlagDeleteLocally()
		{
			song.cachedDelete = true;
			song.dispatchDeleteEvent();
			song.dispatchSyncEvent();
		}

		private void OnMWSDeleteTrackFailed(IEvent evtParams)
		{
			FlagDeleteLocally();
			Release();
		}

		private void OnMWSDeleteTrackSuccess(IEvent evt)
		{
			DeleteLocally();
			try
			{
				MWSClientService.dispatcher.RemoveListener(MWSEvent.DELETE_MY_TRACK_SUCCESS, OnMWSDeleteTrackSuccess);
				MWSClientService.dispatcher.RemoveListener(MWSEvent.DELETE_MY_TRACK_FAILED, OnMWSDeleteTrackFailed);
			}
			finally
			{
				Release();
				base.dispatcher.Dispatch(SoundStudioEvent.SONG_DELETE_COMPLETED);
			}
		}

		private void dispatchConnectionError(SongVO song)
		{
			List<object> list = new List<object>();
			list.Add(song);
			base.dispatcher.Dispatch(ErrorEvent.ERROR, new ErrorPayload("502", new List<object>
			{
				song
			}));
		}
	}
}
