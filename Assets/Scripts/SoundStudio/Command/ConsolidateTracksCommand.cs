using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using System;
using System.Collections.Generic;

namespace SoundStudio.Command
{
	public class ConsolidateTracksCommand : EventCommand
	{
		private List<SoundStudioTrackData> serverTracks;

		private int numTracksToAdd;

		private TracksConsolidator tracksConsolidator = new TracksConsolidator();

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
			serverTracks = (List<SoundStudioTrackData>)base.evt.data;
			DeleteLocalSongs();
			AddServerSongsToLocal();
		}

		private void DeleteLocalSongs()
		{
			ICollection<SongVO> songList = application.songData.SongList;
			List<SongVO> list = tracksConsolidator.CalculateLocalSongsToDelete(serverTracks, songList);
			foreach (SongVO item in list)
			{
				base.dispatcher.Dispatch(SongDeleteEvent.SONG_DELETE, item);
			}
		}

		private void AddServerSongsToLocal()
		{
			ICollection<SongVO> songList = application.songData.SongList;
			List<long> list = tracksConsolidator.CalculateServerTracksToAddLocally(serverTracks, songList);
			if (list.Count > 0)
			{
				numTracksToAdd = list.Count;
				base.dispatcher.AddListener(MWSEvent.GET_MY_TRACK_SUCCESS, OnGetMyTrackSuccess);
				base.dispatcher.AddListener(MWSEvent.GET_MY_TRACK_FAILED, OnGetMyTrackFailed);
				foreach (long item in list)
				{
					MWSClientService.GetMySoundStudioTrack(application.currentPlayer.ID, item);
				}
			}
			else
			{
				base.dispatcher.Dispatch(SoundStudioEvent.CONSOLIDATE_TRACKS_COMPLETE);
				application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
			}
		}

		private void OnGetMyTrackSuccess()
		{
			if (numTracksToAdd == 0)
			{
				throw new InvalidOperationException("All tracks already received. This should not be called.");
			}
			numTracksToAdd--;
			if (numTracksToAdd <= 0)
			{
				RemoveGetMyTrackListeners();
				application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
				base.dispatcher.Dispatch(SoundStudioEvent.CONSOLIDATE_TRACKS_COMPLETE);
			}
		}

		private void OnGetMyTrackFailed()
		{
			RemoveGetMyTrackListeners();
			application.currentPlayer.MySongsState = MySongsStatus.ERROR;
			base.dispatcher.Dispatch(SoundStudioEvent.CONSOLIDATE_TRACKS_FAILED);
		}

		private void RemoveGetMyTrackListeners()
		{
			base.dispatcher.RemoveListener(MWSEvent.GET_MY_TRACK_SUCCESS, OnGetMyTrackSuccess);
			base.dispatcher.RemoveListener(MWSEvent.GET_MY_TRACK_FAILED, OnGetMyTrackFailed);
		}
	}
}
