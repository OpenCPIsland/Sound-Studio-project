namespace SoundStudio.Command.MWS
{
	public class GetSongDataCommandPayload
	{
		private long playerID;

		private long trackID;

		public long PlayerID => playerID;

		public long TrackID => trackID;

		public GetSongDataCommandPayload(long playerID, long trackID)
		{
			this.playerID = playerID;
			this.trackID = trackID;
		}
	}
}
