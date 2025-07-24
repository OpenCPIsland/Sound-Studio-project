using SoundStudio.Event;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Command.MWS
{
	public class GetSongDataCommand : MWSCommand
	{
		public override void Execute()
		{
			Retain();
			GetSongDataCommandPayload getSongDataCommandPayload = (GetSongDataCommandPayload)base.evt.data;
			base.dispatcher.AddListener(MWSEvent.GET_TRACK_COMPLETED, OnComplete);
			base.MWSClientService.GetSoundStudioTrack(getSongDataCommandPayload.PlayerID, getSongDataCommandPayload.TrackID);
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				base.dispatcher.RemoveListener(MWSEvent.GET_TRACK_COMPLETED, OnComplete);
			}
			finally
			{
				Release();
			}
		}
	}
}
