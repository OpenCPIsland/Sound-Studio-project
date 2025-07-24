using System.Collections.Generic;

namespace SoundStudio.Command
{
	public class LoadRadioBlockCommandPayload
	{
		private RadioCategory radioCategory;

		private int beforeTrackID;

		private int blockSize;

		private List<string> friendSwids;

		public RadioCategory RadioCategory => radioCategory;

		public int BlockSize => blockSize;

		public int BeforeTrackID => beforeTrackID;

		public List<string> FriendSwids => friendSwids;

		public LoadRadioBlockCommandPayload(RadioCategory radioCategory, List<string> friendSwids)
		{
			this.radioCategory = radioCategory;
			this.friendSwids = friendSwids;
		}

		public LoadRadioBlockCommandPayload(RadioCategory radioCategory, int blockSize, int beforeTrackID)
		{
			this.radioCategory = radioCategory;
			this.beforeTrackID = beforeTrackID;
			this.blockSize = blockSize;
		}

		public LoadRadioBlockCommandPayload(RadioCategory radioCategory, int blockSize)
		{
			this.radioCategory = radioCategory;
			this.blockSize = blockSize;
		}
	}
}
