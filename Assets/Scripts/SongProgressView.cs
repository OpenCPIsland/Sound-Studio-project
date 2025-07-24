using SoundStudio.Model;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

public class SongProgressView : EventView
{
	internal const string SONG_LOADED_EVENT = "SONG_LOADED_EVENT";

	public Slider slider;

	public Text elaspedText;

	public Text remainingText;

	public SongVO songVO;

	public void LoadSong(SongVO songVO)
	{
		this.songVO = songVO;
		if (base.dispatcher != null)
		{
			base.dispatcher.Dispatch("SONG_LOADED_EVENT");
		}
	}

	public void setSlider(float percent)
	{
		slider.value = percent;
	}

	public void setElapsedText(string timeText)
	{
		elaspedText.text = timeText;
	}

	public void setRemainingText(string timeText)
	{
		remainingText.text = timeText;
	}
}
