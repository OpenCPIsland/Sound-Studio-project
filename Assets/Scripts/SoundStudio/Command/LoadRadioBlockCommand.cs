using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Command.MWS;
using SoundStudio.Command.PDR;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Command
{
	public class LoadRadioBlockCommand : EventCommand
	{
		private List<RadioSongVO> pendingBlock;

		private LoadRadioBlockCommandPayload payload;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			payload = (base.evt.data as LoadRadioBlockCommandPayload);
			if (!application.UseOnlineServices || Application.internetReachability == NetworkReachability.NotReachable)
			{
				if (TryDispatchOfflineBlock())
				{
					return;
				}
				OnConnectionFail();
				return;
			}
			Retain();
			CreatePendingBlock();
			GetRadioData();
		}

		private void CreatePendingBlock()
		{
			pendingBlock = new List<RadioSongVO>();
		}

		private void GetRadioData()
		{
			switch (payload.RadioCategory)
			{
			case RadioCategory.RANDOM:
				base.dispatcher.AddListener(MWSEvent.GET_RADIO_LIST_RANDOM_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.AddListener(MWSEvent.GET_RADIO_LIST_RANDOM_FAILED, OnConnectionFail);
				base.dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST, new GetRadioListCommandPayload(RadioCategory.RANDOM, payload.BlockSize));
				break;
			case RadioCategory.NEW:
				base.dispatcher.AddListener(MWSEvent.GET_RADIO_LIST_NEW_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.AddListener(MWSEvent.GET_RADIO_LIST_NEW_FAILED, OnConnectionFail);
				base.dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST, new GetRadioListCommandPayload(RadioCategory.NEW, payload.BlockSize, payload.BeforeTrackID));
				break;
			case RadioCategory.FRIENDS:
				base.dispatcher.AddListener(MWSEvent.GET_SHARED_TRACKS_LISTING_FAILED, OnConnectionFail);
				base.dispatcher.AddListener(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.Dispatch(MWSEvent.GET_SHARED_TRACKS_LISTING, payload.FriendSwids);
				break;
			}
		}

		private void OnGetRadioDataComplete(IEvent evt)
		{
			switch (payload.RadioCategory)
			{
			case RadioCategory.RANDOM:
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_RANDOM_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_RANDOM_FAILED, OnConnectionFail);
				break;
			case RadioCategory.NEW:
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_NEW_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_NEW_FAILED, OnConnectionFail);
				break;
			case RadioCategory.FRIENDS:
				base.dispatcher.RemoveListener(MWSEvent.GET_SHARED_TRACKS_LISTING_FAILED, OnConnectionFail);
				base.dispatcher.RemoveListener(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, OnGetRadioDataComplete);
				break;
			}
			List<SoundStudioRadioTrackData> list = (List<SoundStudioRadioTrackData>)evt.data;
			if (list.Count == 0)
			{
				RemoveAllListeners();
				base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, new LoadRadioBlockCompletePayload(payload.RadioCategory, pendingBlock));
			}
			else
			{
				AddTrackDataToBlock(list);
				LoadPaperDolls();
				LoadSongData();
			}
		}

		private void AddTrackDataToBlock(List<SoundStudioRadioTrackData> radioTrackData)
		{
			for (int i = 0; i < radioTrackData.Count; i++)
			{
				pendingBlock.Add(new RadioSongVO());
				pendingBlock[i].soundStudioRadioTrackData = radioTrackData[i];
			}
		}

		private void LoadPaperDolls()
		{
			base.dispatcher.AddListener(PDREvent.GET_PAPER_DOLL_COMPLETED, HandlePaperDollImage);
			base.dispatcher.AddListener(PDREvent.GET_PAPER_DOLL_FAILED, OnConnectionFail);
			foreach (RadioSongVO item in pendingBlock)
			{
				base.dispatcher.Dispatch(PDREvent.GET_PAPER_DOLL, new GetPaperDollCommandPayload(item.soundStudioRadioTrackData.playerSwid, 300, false, false, "en"));
			}
		}

		private void HandlePaperDollImage(IEvent evt)
		{
			PDRRequest pDRRequest = evt.data as PDRRequest;
			byte[] pdrImageBytes = pDRRequest.pdrImageBytes;
			foreach (RadioSongVO item in pendingBlock)
			{
				if (item.soundStudioRadioTrackData.playerSwid == pDRRequest.payload.Swid)
				{
					item.paperDollImageRaw = pDRRequest.pdrImageBytes;
					break;
				}
			}
			CheckBlockCompletion();
		}

		private void LoadSongData()
		{
			base.dispatcher.AddListener(MWSEvent.GET_TRACK_COMPLETED, OnLoadSongDataComplete);
			base.dispatcher.AddListener(MWSEvent.GET_TRACK_FAILED, OnConnectionFail);
			for (int i = 0; i < pendingBlock.Count; i++)
			{
				base.dispatcher.Dispatch(MWSEvent.GET_TRACK, new GetSongDataCommandPayload(pendingBlock[i].soundStudioRadioTrackData.soundStudioTrackData.PlayerId, pendingBlock[i].soundStudioRadioTrackData.soundStudioTrackData.TrackId));
			}
		}

		private void OnLoadSongDataComplete(IEvent evt)
		{
			SoundStudioTrackData soundStudioTrackData = evt.data as SoundStudioTrackData;
			for (int i = 0; i < pendingBlock.Count; i++)
			{
				if (pendingBlock[i].soundStudioRadioTrackData.soundStudioTrackData.PlayerId == soundStudioTrackData.PlayerId)
				{
					pendingBlock[i].songVO = Utils.ConvertSoundStudioTrackDataToSongVO(soundStudioTrackData);
				}
			}
			CheckBlockCompletion();
		}

		private void CheckBlockCompletion()
		{
			bool flag = true;
			foreach (RadioSongVO item in pendingBlock)
			{
				if (item.paperDollImageRaw == null)
				{
					flag = false;
					break;
				}
				if (item.songVO == null)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				RemoveAllListeners();
				base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, new LoadRadioBlockCompletePayload(payload.RadioCategory, pendingBlock));
			}
		}

		private void OnConnectionFail()
		{
			if (pendingBlock != null)
			{
				RemoveAllListeners();
			}
			if (TryDispatchOfflineBlock())
			{
				return;
			}
			if (payload != null)
			{
				base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, payload.RadioCategory);
			}
		}

		private void RemoveAllListeners()
		{
			switch (payload.RadioCategory)
			{
			case RadioCategory.RANDOM:
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_RANDOM_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_RANDOM_FAILED, OnConnectionFail);
				break;
			case RadioCategory.NEW:
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_NEW_SUCCESS, OnGetRadioDataComplete);
				base.dispatcher.RemoveListener(MWSEvent.GET_RADIO_LIST_NEW_FAILED, OnConnectionFail);
				break;
			case RadioCategory.FRIENDS:
				base.dispatcher.RemoveListener(MWSEvent.GET_SHARED_TRACKS_LISTING_FAILED, OnConnectionFail);
				base.dispatcher.RemoveListener(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, OnGetRadioDataComplete);
				break;
			}
			base.dispatcher.RemoveListener(PDREvent.GET_PAPER_DOLL_COMPLETED, HandlePaperDollImage);
			base.dispatcher.RemoveListener(PDREvent.GET_PAPER_DOLL_FAILED, OnConnectionFail);
			base.dispatcher.RemoveListener(MWSEvent.GET_TRACK_COMPLETED, OnLoadSongDataComplete);
			base.dispatcher.RemoveListener(MWSEvent.GET_TRACK_FAILED, OnConnectionFail);
			Release();
		}

		private bool TryDispatchOfflineBlock()
		{
			List<RadioSongVO> offlineBlock = BuildOfflineBlock();
			if (offlineBlock == null)
			{
				return false;
			}
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, new LoadRadioBlockCompletePayload(payload.RadioCategory, offlineBlock));
			return true;
		}

		private List<RadioSongVO> BuildOfflineBlock()
		{
			if (application == null || application.songData == null || application.songData.Count == 0)
			{
				return null;
			}
			List<SongVO> list = new List<SongVO>(application.songData.SongList);
			if (list.Count == 0)
			{
				return null;
			}
			SortOfflineSongs(list);
			int offlineStartIndex = GetOfflineStartIndex(list);
			if (offlineStartIndex >= list.Count)
			{
				return new List<RadioSongVO>();
			}
			int num = Mathf.Min(GetOfflineBlockSize(), list.Count - offlineStartIndex);
			List<RadioSongVO> list2 = new List<RadioSongVO>(num);
			for (int i = offlineStartIndex; i < offlineStartIndex + num; i++)
			{
				list2.Add(CreateOfflineRadioSong(list[i]));
			}
			return list2;
		}

		private int GetOfflineBlockSize()
		{
			if (payload.BlockSize > 0)
			{
				return payload.BlockSize;
			}
			if (payload.FriendSwids != null && payload.FriendSwids.Count > 0)
			{
				return payload.FriendSwids.Count;
			}
			return 5;
		}

		private int GetOfflineStartIndex(List<SongVO> songs)
		{
			if (payload.BeforeTrackID == 0)
			{
				return 0;
			}
			for (int i = 0; i < songs.Count; i++)
			{
				if (GetOfflineTrackId(songs[i]) == payload.BeforeTrackID)
				{
					return i + 1;
				}
			}
			return songs.Count;
		}

		private void SortOfflineSongs(List<SongVO> songs)
		{
			songs.Sort(delegate(SongVO a, SongVO b)
			{
				int num = 0;
				switch (payload.RadioCategory)
				{
				case RadioCategory.RANDOM:
					num = GetOfflineSortSeed(a).CompareTo(GetOfflineSortSeed(b));
					break;
				case RadioCategory.FRIENDS:
				case RadioCategory.NEW:
					num = b.timeStamp.CompareTo(a.timeStamp);
					break;
				}
				if (num == 0)
				{
					num = GetOfflineTrackId(a).CompareTo(GetOfflineTrackId(b));
				}
				return num;
			});
		}

		private RadioSongVO CreateOfflineRadioSong(SongVO song)
		{
			string text = song.songName;
			if (string.IsNullOrEmpty(text))
			{
				text = ((application.currentPlayer != null && !string.IsNullOrEmpty(application.currentPlayer.DisplayName)) ? application.currentPlayer.DisplayName : "Saved Track");
			}
			return new RadioSongVO
			{
				songVO = song,
				paperDollImageRaw = null,
				soundStudioRadioTrackData = new SoundStudioRadioTrackData
				{
					playerSwid = ((application.currentPlayer != null) ? application.currentPlayer.Swid : string.Empty),
					playerDisplayName = text,
					soundStudioTrackData = new SoundStudioTrackData
					{
						Data = song.rawData,
						LastModified = ConvertToUnixMilliseconds(song.timeStamp),
						Name = song.songName,
						PlayerId = ((song.playerid != 0L) ? song.playerid : ((application.currentPlayer != null) ? application.currentPlayer.ID : 0L)),
						TrackId = GetOfflineTrackId(song),
						TrackShareState = TrackShareState.NOT_SHARED
					}
				}
			};
		}

		private int GetOfflineTrackId(SongVO song)
		{
			if (song.HasServerID && song.serverID <= int.MaxValue && song.serverID >= int.MinValue)
			{
				return (int)song.serverID;
			}
			int offlineSongHash = GetOfflineSongHash(song);
			if (offlineSongHash == 0)
			{
				return int.MinValue + 1;
			}
			return offlineSongHash;
		}

		private int GetOfflineSortSeed(SongVO song)
		{
			unchecked
			{
				int offlineSongHash = GetOfflineSongHash(song);
				offlineSongHash = offlineSongHash * 397 ^ song.GenreID;
				return offlineSongHash;
			}
		}

		private int GetOfflineSongHash(SongVO song)
		{
			unchecked
			{
				int num = 17;
				num = num * 23 + ((song.songName != null) ? song.songName.GetHashCode() : 0);
				num = num * 23 + ((song.rawData != null) ? song.rawData.GetHashCode() : 0);
				num = num * 23 + ((song.FileName != null) ? song.FileName.GetHashCode() : 0);
				num = num * 23 + song.timeStamp.GetHashCode();
				return num;
			}
		}

		private long ConvertToUnixMilliseconds(System.DateTime timeStamp)
		{
			if (timeStamp == default(System.DateTime))
			{
				return 0L;
			}
			return (long)(timeStamp.ToUniversalTime() - new System.DateTime(1970, 1, 1)).TotalMilliseconds;
		}
	}
}
