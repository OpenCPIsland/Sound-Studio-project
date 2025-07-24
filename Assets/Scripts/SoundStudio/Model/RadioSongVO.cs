using Disney.ClubPenguin.Service.MWS.Domain;

namespace SoundStudio.Model
{
	public class RadioSongVO
	{
		public SoundStudioRadioTrackData soundStudioRadioTrackData
		{
			get;
			set;
		}

		public SongVO songVO
		{
			get;
			set;
		}

		public byte[] paperDollImageRaw
		{
			get;
			set;
		}
	}
}
