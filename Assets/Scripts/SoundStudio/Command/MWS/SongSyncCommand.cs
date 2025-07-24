using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;

namespace SoundStudio.Command.MWS
{
	public class SongSyncCommand : MWSCommand
	{
		private SongSyncVO songSyncVo;

		public override void Execute()
		{
			if (base.ApplicationState.currentPlayer.AccountStatus == MembershipStatus.GUEST)
			{
				Release();
				return;
			}
			if (base.ApplicationState.songData.SynchronizedCount >= 24)
			{
				base.dispatcher.Dispatch(ErrorEvent.ERROR, new ErrorPayload("501"));
				Fail();
				return;
			}
			songSyncVo = (base.evt.data as SongSyncVO);
			if (songSyncVo == null)
			{
				throw new ArgumentNullException("The wrong type was dispatched to the song sync command: " + base.evt);
			}
			if (songSyncVo.CanRetry())
			{
				songSyncVo.DecrementRetryCount();
				songSyncVo.SongVo.playerid = base.ApplicationState.currentPlayer.ID;
				SoundStudioTrackData soundStudioTrackData = Utils.ConvertSongVOToSoundStudioTrackData(songSyncVo.SongVo);
				base.dispatcher.AddListener(MWSEvent.ADD_NEW_TRACK_COMPLETED, OnComplete);
				base.MWSClientService.AddSoundStudioTrack(soundStudioTrackData);
				Retain();
			}
			else
			{
				Release();
			}
		}

		private void OnComplete(IEvent paramEvent)
		{
			base.MWSClientService.dispatcher.RemoveListener(MWSEvent.ADD_NEW_TRACK_COMPLETED, OnComplete);
			IGetSoundStudioTrackDataResponse getSoundStudioTrackDataResponse = paramEvent.data as IGetSoundStudioTrackDataResponse;
			if (getSoundStudioTrackDataResponse != null)
			{
				if (getSoundStudioTrackDataResponse.IsError)
				{
					Fail();
					int statusCode = getSoundStudioTrackDataResponse.StatusCode;
					if (statusCode == 401)
					{
						OnUnauthorized();
					}
					else
					{
						OnSyncFailed();
					}
				}
				else
				{
					Release();
					if (getSoundStudioTrackDataResponse.SoundStudioTrackData.PlayerId == base.ApplicationState.currentPlayer.ID)
					{
						SongVO songVO = Utils.ConvertSoundStudioTrackDataToSongVO(getSoundStudioTrackDataResponse.SoundStudioTrackData);
						songSyncVo.SongVo.timeStamp = songVO.timeStamp;
						songSyncVo.SongVo.serverID = songVO.serverID;
						base.ApplicationState.songData.AddReplaceSong(songSyncVo.SongVo);
						base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songSyncVo.SongVo);
						songSyncVo.SongVo.dispatchSyncEvent();
					}
				}
				base.dispatcher.Dispatch(SoundStudioEvent.SONG_SYNC_COMPLETED, songSyncVo);
			}
			base.dispatcher.Dispatch(SoundStudioEvent.RESUME_RETRY_SERVICE);
		}

		private void OnSyncFailed()
		{
			dispatchConnectionError(songSyncVo.SongVo);
			songSyncVo.SongVo.dispatchSyncEvent();
			base.dispatcher.RemoveListener(SoundStudioEvent.RELOGIN_FOR_SAVE_FAILED, OnSyncFailed);
			Release();
		}

		private void OnUnauthorized()
		{
			base.dispatcher.Dispatch(SoundStudioEvent.RELOGIN_FOR_SAVE, songSyncVo);
			base.dispatcher.AddListener(SoundStudioEvent.RELOGIN_FOR_SAVE_FAILED, OnSyncFailed);
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
