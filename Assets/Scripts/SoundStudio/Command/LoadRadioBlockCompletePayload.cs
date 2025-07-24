using SoundStudio.Model;
using System.Collections.Generic;

namespace SoundStudio.Command
{
	public class LoadRadioBlockCompletePayload
	{
		private RadioCategory radioCategory;

		private List<RadioSongVO> radioBlock;

		public RadioCategory RadioCategory => radioCategory;

		public List<RadioSongVO> RadioBlock => radioBlock;

		public LoadRadioBlockCompletePayload(RadioCategory radioCategory, List<RadioSongVO> radioBlock)
		{
			this.radioCategory = radioCategory;
			this.radioBlock = radioBlock;
		}
	}
}
