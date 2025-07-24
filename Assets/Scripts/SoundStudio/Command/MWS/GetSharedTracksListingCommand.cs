using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;

namespace SoundStudio.Command.MWS
{
	internal class GetSharedTracksListingCommand : MWSCommand
	{
		public override void Execute()
		{
			List<string> list = base.evt.data as List<string>;
			if (list != null && list.Count > 0)
			{
				BeforeExecute();
				foreach (string item in list)
				{
					base.ApplicationState.currentPlayer.FriendsTrackCollection.SetNoTrack(item);
				}
				base.MWSClientService.dispatcher.AddListener(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, OnComplete);
				base.MWSClientService.GetSharedTracksBySwids(list, Utils.SystemLanguageToMWSLocale());
				return;
			}
			throw new InvalidOperationException("The player ids were not set to get the shared tracks listing");
		}

		private void OnComplete(IEvent evtParams)
		{
			try
			{
				base.MWSClientService.dispatcher.RemoveListener(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, OnComplete);
				List<SoundStudioRadioTrackData> list = evtParams.data as List<SoundStudioRadioTrackData>;
				if (list != null)
				{
					foreach (SoundStudioRadioTrackData item in list)
					{
						base.ApplicationState.currentPlayer.FriendsTrackCollection.AddOrUpdateSoundStudioRadioTrackData(item);
					}
					base.dispatcher.Dispatch(SoundStudioEvent.LIST_FRIEND_SHARED_TRACKS_COMPLETED);
				}
			}
			finally
			{
				Release();
			}
		}
	}
}
