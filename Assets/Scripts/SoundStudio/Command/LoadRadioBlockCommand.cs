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
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
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
	}
}
