using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Command.MWS
{
	internal class GetMySoundStudioTracksCommand : MWSCommand
	{
		public override void Execute()
		{
			BeforeExecute();
			base.MWSClientService.dispatcher.AddListener(MWSEvent.GET_MY_TRACKS_COMPLETED, OnComplete);
			base.MWSClientService.GetMySoundStudioTracks();
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				base.MWSClientService.dispatcher.RemoveListener(MWSEvent.GET_MY_TRACKS_COMPLETED, OnComplete);
				IGetSoundStudioTracksDataResponse getSoundStudioTracksDataResponse = (IGetSoundStudioTracksDataResponse)evt.data;
				if (getSoundStudioTracksDataResponse != null && !getSoundStudioTracksDataResponse.IsError)
				{
					foreach (SoundStudioTrackData track in getSoundStudioTracksDataResponse.Tracks)
					{
					}
				}
			}
			finally
			{
				Release();
			}
		}
	}
}
