using Disney.ClubPenguin.Login;
using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Command
{
	public class LoadPlayerCommand : AutoListenerCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			Retain();
			if (!application.UseOnlineServices)
			{
				PrepareOfflinePlayer();
				AddListener(SoundStudioEvent.LOAD_SONGS_COMPLETED, OnLoadSongsComplete);
				base.dispatcher.Dispatch(LoadSongListEvent.LOAD_SONG_LIST);
				base.dispatcher.Dispatch(SoundStudioEvent.LOAD_ACCOUNT_COMPLETE);
				return;
			}
			if (application.currentPlayer.AccountStatus != 0)
			{
				AddListener(MWSEvent.GET_ACCOUNT_COMPLETED, OnLoadAccountComplete);
				AddListener(MWSEvent.GET_ACCOUNT_FAILED, OnLoadAccountFailed);
				base.dispatcher.Dispatch(MWSEvent.GET_ACCOUNT);
			}
			else
			{
				OnLoadAccountFailed();
			}
		}

		private void PrepareOfflinePlayer()
		{
			if (application.currentPlayer == null)
			{
				application.currentPlayer = new PlayerAccountVO();
			}
			if (string.IsNullOrEmpty(application.currentPlayer.Username))
			{
				SavedPlayerCollection savedPlayerCollection = new SavedPlayerCollection();
				if (savedPlayerCollection.ExistsOnDisk())
				{
					savedPlayerCollection.LoadFromDisk();
					SavedPlayerData mostRecentlyLoggedInPlayer = savedPlayerCollection.GetMostRecentlyLoggedInPlayer();
					if (mostRecentlyLoggedInPlayer != null && !string.IsNullOrEmpty(mostRecentlyLoggedInPlayer.UserName))
					{
						application.currentPlayer.Username = mostRecentlyLoggedInPlayer.UserName;
						application.currentPlayer.DisplayName = string.IsNullOrEmpty(mostRecentlyLoggedInPlayer.DisplayName) ? mostRecentlyLoggedInPlayer.UserName : mostRecentlyLoggedInPlayer.DisplayName;
						application.currentPlayer.Swid = mostRecentlyLoggedInPlayer.Swid;
						if (application.currentPlayer.ID == 0L)
						{
							application.currentPlayer.ID = 1L;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(application.currentPlayer.Username))
			{
				application.currentPlayer.Username = "guest";
			}
			if (string.IsNullOrEmpty(application.currentPlayer.DisplayName))
			{
				application.currentPlayer.DisplayName = application.currentPlayer.Username;
			}
			application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
		}

		public void OnLoadAccountFailed()
		{
			base.dispatcher.Dispatch(SoundStudioEvent.CHANGE_USER);
			AddListener(SoundStudioEvent.LOAD_SONGS_COMPLETED, OnLoadSongsComplete);
			base.dispatcher.Dispatch(LoadSongListEvent.LOAD_SONG_LIST);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_ACCOUNT_COMPLETE);
		}

		public void OnLoadAccountComplete(IEvent evt)
		{
			Account account = (Account)evt.data;
			if (!account.Member)
			{
				application.currentPlayer.AccountStatus = MembershipStatus.NONMEMBER;
			}
			application.currentPlayer.PenguinColor = account.Colour;
			AddListener(SoundStudioEvent.LOAD_SONGS_COMPLETED, OnLoadSongsComplete);
			base.dispatcher.Dispatch(LoadSongListEvent.LOAD_SONG_LIST);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_ACCOUNT_COMPLETE);
		}

		public void OnLoadSongsComplete()
		{
			if (!application.UseOnlineServices)
			{
				application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
				Release();
				return;
			}
			if (application.currentPlayer.AccountStatus != 0 && Application.internetReachability != 0)
			{
				base.dispatcher.Dispatch(SoundStudioEvent.PERFORM_CACHED_ACTIONS);
				AddListener(SoundStudioEvent.GET_MY_TRACKS_LISTING_COMMAND_COMPLETE, OnGetMyTracksListingComplete);
				AddListener(SoundStudioEvent.CONSOLIDATE_TRACKS_FAILED, OnGetMyTracksListingFailed);
				base.dispatcher.Dispatch(MWSEvent.GET_MY_TRACKS_LISTING);
			}
			else
			{
				application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
				Release();
			}
		}

		public void OnGetMyTracksListingComplete(IEvent getTrackListingEvent)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.CONSOLIDATE_TRACKS, (List<SoundStudioTrackData>)getTrackListingEvent.data);
			Release();
		}

		public void OnGetMyTracksListingFailed()
		{
			application.currentPlayer.MySongsState = MySongsStatus.ERROR;
			Release();
		}
	}
}
