using SoundStudio.Model;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class SongProgressMediator : EventMediator
	{
		private const float MILLISECONDS_PER_UPDATE = 50f;

		private SongVO songVO;

		private float elapsedMilliseconds;

		[Inject]
		public SongProgressView view
		{
			get;
			set;
		}

		public override void OnRegister()
		{
			view.dispatcher.AddListener("SONG_LOADED_EVENT", OnSongLoaded);
			if (view.songVO != null)
			{
				OnSongLoaded();
			}
		}

		public override void OnRemove()
		{
			view.dispatcher.RemoveListener("SONG_LOADED_EVENT", OnSongLoaded);
			if (songVO != null)
			{
				songVO.SongPlayEvent -= Song_Play_Handler;
				songVO.SongStopEvent -= Song_Stop_Handler;
			}
		}

		public void OnDestroy()
		{
			OnRemove();
		}

		private void OnSongLoaded()
		{
			songVO = view.songVO;
			songVO.SongPlayEvent += Song_Play_Handler;
			songVO.SongStopEvent += Song_Stop_Handler;
			view.setRemainingText(Utils.secondsToMinutes((int)MillisecondsToSeconds(songVO.LengthInInMilliseconds)));
		}

		private void Song_Play_Handler()
		{
			elapsedMilliseconds = 0f;
			InvokeRepeating("OnUpdateTick", MillisecondsToSeconds(50f), MillisecondsToSeconds(50f));
		}

		private void Song_Stop_Handler()
		{
			CancelInvoke("OnUpdateTick");
			view.setSlider(0f);
			view.setElapsedText("0:00");
			view.setRemainingText(Utils.secondsToMinutes((int)MillisecondsToSeconds(songVO.LengthInInMilliseconds)));
		}

		private void OnUpdateTick()
		{
			if (songVO != null && view != null)
			{
				elapsedMilliseconds += 50f;
				view.setSlider(elapsedMilliseconds / songVO.LengthInInMilliseconds);
				int num = (int)Mathf.Floor(MillisecondsToSeconds(elapsedMilliseconds));
				view.setElapsedText(Utils.secondsToMinutes(num));
				view.setRemainingText(Utils.secondsToMinutes((int)Mathf.Floor(MillisecondsToSeconds(songVO.LengthInInMilliseconds)) - num));
			}
		}

		private float MillisecondsToSeconds(float milliseconds)
		{
			return milliseconds / 1000f;
		}
	}
}
