namespace SoundStudio.Command.PDR
{
	public class GetPaperDollCommandPayload
	{
		private string swid;

		private int size;

		private bool flag;

		private bool photo;

		private string language;

		public string Swid => swid;

		public int Size => size;

		public bool Flag => flag;

		public bool Photo => photo;

		public string Language => language;

		public GetPaperDollCommandPayload(string swid, int size, bool flag, bool photo, string language)
		{
			this.swid = swid;
			this.size = size;
			this.flag = flag;
			this.photo = photo;
			this.language = language;
		}
	}
}
