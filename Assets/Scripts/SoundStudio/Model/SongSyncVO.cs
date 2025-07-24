using System;

namespace SoundStudio.Model
{
	internal class SongSyncVO
	{
		public const int DEFAULT_RETRY_SYNC_SONG = 1;

		private SongVO songVo;

		private int retryCount = 1;

		public SongVO SongVo
		{
			get
			{
				return songVo;
			}
			set
			{
				songVo = value;
			}
		}

		public int RetryCount => retryCount;

		public SongSyncVO(SongVO songVo)
		{
			if (songVo == null)
			{
				throw new ArgumentNullException("soundVo cannot be null.");
			}
			this.songVo = songVo;
		}

		public void DecrementRetryCount()
		{
			if (--retryCount < 0)
			{
				throw new InvalidOperationException("Count cannot be negative");
			}
		}

		public bool CanRetry()
		{
			return retryCount > 0;
		}
	}
}
