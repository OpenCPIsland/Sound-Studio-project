using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;

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
			if (application.currentPlayer.AccountStatus != 0)
			{
				base.dispatcher.Dispatch(SoundStudioEvent.PERFORM_CACHED_ACTIONS);
				AddListener(SoundStudioEvent.GET_MY_TRACKS_LISTING_COMMAND_COMPLETE, OnGetMyTracksListingComplete);
				base.dispatcher.Dispatch(MWSEvent.GET_MY_TRACKS_LISTING);
			}
			else
			{
				application.currentPlayer.MySongsState = MySongsStatus.COMPLETE;
			}
		}

		public void OnGetMyTracksListingComplete(IEvent getTrackListingEvent)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.CONSOLIDATE_TRACKS, (List<SoundStudioTrackData>)getTrackListingEvent.data);
			Release();
		}
	}
}
