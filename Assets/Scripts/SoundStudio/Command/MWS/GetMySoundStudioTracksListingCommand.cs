using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;

namespace SoundStudio.Command.MWS
{
	internal class GetMySoundStudioTracksListingCommand : MWSCommand
	{
		private List<SoundStudioTrackData> serverTracks;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			BeforeExecute();
			base.dispatcher.Dispatch(SoundStudioEvent.PAUSE_RETRY_SERVICE);
			base.MWSClientService.dispatcher.AddListener(MWSEvent.GET_MY_TRACKS_LISTING_COMPLETED, OnComplete);
			base.MWSClientService.GetSoundStudioMyTracksListing();
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				base.MWSClientService.dispatcher.RemoveListener(MWSEvent.GET_MY_TRACKS_LISTING_COMPLETED, OnComplete);
				IGetSoundStudioTracksDataResponse getSoundStudioTracksDataResponse = (IGetSoundStudioTracksDataResponse)evt.data;
				if (getSoundStudioTracksDataResponse != null && !getSoundStudioTracksDataResponse.IsError)
				{
					foreach (SoundStudioTrackData track in getSoundStudioTracksDataResponse.Tracks)
					{
						if (application.songData.ContainsSong(track.TrackId))
						{
							application.songData.GetSong(track.TrackId).isShared = (track.TrackShareState == TrackShareState.SHARED);
						}
					}
					serverTracks = getSoundStudioTracksDataResponse.Tracks;
					base.dispatcher.Dispatch(SoundStudioEvent.GET_MY_TRACKS_LISTING_COMMAND_COMPLETE, serverTracks);
				}
			}
			finally
			{
				base.dispatcher.Dispatch(SoundStudioEvent.RESUME_RETRY_SERVICE);
				Release();
			}
		}
	}
}
