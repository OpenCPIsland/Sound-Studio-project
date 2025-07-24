using System.Collections.Generic;

namespace SoundStudio.Model
{
	public class SoundVO
	{
		public class SortByGroupID : IComparer<SoundVO>
		{
			public int Compare(SoundVO soundVO1, SoundVO soundVO2)
			{
				return soundVO1.GroupID.CompareTo(soundVO2.GroupID);
			}
		}

		public delegate void SoundStateUpdated(SoundVO sound);

		public static IComparer<SoundVO> sortByGroupID = new SortByGroupID();

		private SoundPlayState state;

		private LoadState loadState;

		public string type
		{
			get;
			set;
		}

		public string audioPath
		{
			get;
			set;
		}

		public int id
		{
			get;
			set;
		}

		public string buttonImageOn
		{
			get;
			set;
		}

		public string buttonImageOff
		{
			get;
			set;
		}

		public int GroupID
		{
			get
			{
				int num = 5;
				return id / num;
			}
		}

		public string AudioPath => audioPath;

		public SoundPlayState State => state;

		public event SoundStateUpdated OnSoundStateUpdated;

		public SoundVO()
		{
			state = SoundPlayState.STOPPED;
			loadState = LoadState.UNLOADED;
		}

		public bool IsLoop()
		{
			return "LOOPING".Equals(type);
		}

		public bool IsPlaying()
		{
			return state == SoundPlayState.PLAYING;
		}

		public bool IsLoaded()
		{
			return loadState == LoadState.LOADED;
		}

		public void Play()
		{
			if (state != SoundPlayState.PLAYING)
			{
				state = SoundPlayState.PLAYING;
				dispatchChangeState();
			}
		}

		public void Pause()
		{
			if (state != SoundPlayState.PAUSED)
			{
				state = SoundPlayState.PAUSED;
				dispatchChangeState();
			}
		}

		public void Stop()
		{
			if (state != 0)
			{
				state = SoundPlayState.STOPPED;
				dispatchChangeState();
			}
		}

		public void StopOneShot()
		{
			if (!IsLoop())
			{
				state = SoundPlayState.STOPPED;
			}
		}

		public void Mute()
		{
			if (state != SoundPlayState.MUTED)
			{
				state = SoundPlayState.MUTED;
				dispatchChangeState();
			}
		}

		private void dispatchChangeState()
		{
			if (this.OnSoundStateUpdated != null)
			{
				this.OnSoundStateUpdated(this);
			}
		}

		public override string ToString()
		{
			return $"AudioPath: {AudioPath}, State: {State}, GroupID: {GroupID}, id: {id}, type: {type}";
		}
	}
}
