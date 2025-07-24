using System.Collections.Generic;

namespace SoundStudio.Model
{
	public class RadioSongCollection
	{
		private List<RadioSongVO> radioSongList;

		private int index;

		public RadioSongVO Current => radioSongList[index];

		public int RemainingSongs => radioSongList.Count - 1 - index;

		public int LastSongID
		{
			get
			{
				if (radioSongList.Count > 0)
				{
					return (int)radioSongList[radioSongList.Count - 1].soundStudioRadioTrackData.soundStudioTrackData.TrackId;
				}
				return 0;
			}
		}

		public int TotalSongs => radioSongList.Count;

		public RadioSongCollection()
		{
			index = 0;
			radioSongList = new List<RadioSongVO>();
		}

		public void AddSongs(List<RadioSongVO> songs)
		{
			radioSongList.AddRange(songs);
		}

		public void Clear()
		{
			radioSongList.Clear();
			index = 0;
		}

		public RadioSongVO Next()
		{
			index++;
			return radioSongList[index];
		}

		public RadioSongVO Previous()
		{
			index--;
			return radioSongList[index];
		}
	}
}
