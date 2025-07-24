namespace SoundStudio.Command.MWS
{
	public class GetRadioListCommandPayload
	{
		private RadioCategory radioCategory;

		private int beforeTrackID;

		private int songCount;

		public RadioCategory RadioCategory => radioCategory;

		public int BeforeTrackID => beforeTrackID;

		public int SongCount => songCount;

		public GetRadioListCommandPayload(RadioCategory radioCategory, int songCount, int beforeTrackID)
		{
			this.radioCategory = radioCategory;
			this.beforeTrackID = beforeTrackID;
			this.songCount = songCount;
		}

		public GetRadioListCommandPayload(RadioCategory radioCategory, int songCount)
		{
			this.radioCategory = radioCategory;
			this.songCount = songCount;
		}
	}
}
