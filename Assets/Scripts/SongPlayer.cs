using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Text;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
	private const string DEFAULT_GRID_STATUS = "0000000000000000000000000000000000000000";

	private SongVO _currentSong;

	private int _currentDataIndex;

	private float _totalTime;

	private SongPlayerState songPlayerState;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	[Inject]
	public GenreAudioManager genreAudioManager
	{
		get;
		set;
	}

	public SongPlayerState SongPlayerState => songPlayerState;

	public SongVO CurrentSong => _currentSong;

	public void Start()
	{
		songPlayerState = new SongPlayerState();
	}

	public void Play(SongVO song)
	{
		_currentSong = song;
		CancelInvoke();
		switch (songPlayerState.PlayState)
		{
		case PlayState.STOPPED:
			TrackOneShotsForMemoryUnload(_currentSong.GenreID);
			StartGenreLoops(_currentSong.GenreID);
			_currentDataIndex = 0;
			break;
		case PlayState.PAUSED:
			TrackOneShotsForMemoryUnload(_currentSong.GenreID);
			StartGenreLoops(_currentSong.GenreID);
			break;
		}
		_currentSong.dispatchPlayEvent();
		dispatchSongPlay();
		songPlayerState.SwitchState(PlayState.PLAYING);
		Invoke("nextRecordData", _currentSong.recordDataList[_currentDataIndex].timeStamp / 1000f);
	}

	public void Stop()
	{
		CancelInvoke();
		AudioClipPlayer.StopAllAudio();
		songPlayerState.SwitchState(PlayState.STOPPED);
		if (_currentSong != null)
		{
			_currentSong.dispatchStopEvent();
			_currentSong = null;
		}
	}

	public void Pause()
	{
		CancelInvoke();
		songPlayerState.SwitchState(PlayState.PAUSED);
	}

	private void nextRecordData()
	{
		SongVO currentSong = _currentSong;
		if (currentSong != null)
		{
			if (songPlayerState.PlayState != PlayState.PLAYING)
			{
				throw new InvalidOperationException("The current song is playing but should be stopped");
			}
			evalulateGridStatus(currentSong.recordDataList[_currentDataIndex].GridValue);
			if (currentSong.recordDataList == null)
			{
				throw new InvalidOperationException("The current song's data list is null");
			}
			if (_currentDataIndex < currentSong.recordDataList.Count - 1)
			{
				_currentDataIndex++;
				Invoke("nextRecordData", (currentSong.recordDataList[_currentDataIndex].timeStamp - currentSong.recordDataList[_currentDataIndex - 1].timeStamp) / 1000f);
			}
		}
	}

	private void evalulateGridStatus(string currentStatus)
	{
		if (currentStatus.IndexOf("1111111111111111") != -1)
		{
			Stop();
			dispatchSongComplete();
			return;
		}
		char[] array = currentStatus.ToCharArray();
		char[] array2 = "0000000000000000000000000000000000000000".ToCharArray();
		if (_currentDataIndex > 0)
		{
			array2 = _currentSong.recordDataList[_currentDataIndex - 1].gridStatus.ToCharArray();
		}
		for (int i = 0; i < 40; i++)
		{
			if (array[i] != array2[i] && i < 25)
			{
				evaluateGridChange(array[i], i, looping: true);
			}
			else if (i >= 25 && array[i] == '1')
			{
				evaluateGridChange(array[i], i, looping: false);
			}
		}
	}

	private void evalulateGridStatus(long gridValue)
	{
		if (gridValue == 65535)
		{
			Stop();
			dispatchSongComplete();
			return;
		}
		long value = 0L;
		if (_currentDataIndex > 0)
		{
			value = _currentSong.recordDataList[_currentDataIndex - 1].GridValue;
		}
		for (int i = 0; i < 40; i++)
		{
			if (RecordDataVO.IsBitSetOn(i, gridValue) != RecordDataVO.IsBitSetOn(i, value) && i < 25)
			{
				evaluateGridChange(gridValue, i, looping: true);
			}
			else if (i >= 25 && RecordDataVO.IsBitSetOn(i, gridValue))
			{
				evaluateGridChange(gridValue, i, looping: false);
			}
		}
	}

	private void evaluateGridChange(long gridValue, int gridID, bool looping)
	{
		MuteState muteState = RecordDataVO.IsBitSetOn(gridID, gridValue) ? MuteState.OFF : MuteState.ON;
		if (looping)
		{
			AudioClipPlayer.PlaySoundClip(genreAudioManager.getAudioClip(_currentSong.GenreID, gridID), muteState, LoopState.ON);
		}
		else
		{
			AudioClipPlayer.PlaySoundClip(genreAudioManager.getAudioClip(_currentSong.GenreID, gridID), muteState, LoopState.OFF);
		}
	}

	private void evaluateGridChange(char gridValue, int gridID, bool looping)
	{
		if (looping)
		{
			switch (gridValue)
			{
			case '0':
				AudioClipPlayer.PlaySoundClip(genreAudioManager.getAudioClip(_currentSong.GenreID, gridID), MuteState.ON, LoopState.ON);
				break;
			case '1':
				AudioClipPlayer.PlaySoundClip(genreAudioManager.getAudioClip(_currentSong.GenreID, gridID), MuteState.OFF, LoopState.ON);
				break;
			}
		}
		else if (gridValue == '1')
		{
			AudioClipPlayer.PlaySoundClip(genreAudioManager.getAudioClip(_currentSong.GenreID, gridID), MuteState.OFF, LoopState.OFF);
		}
	}

	private void dispatchSongComplete()
	{
		dispatcher.Dispatch(SongCompleteEvent.SONG_COMPLETE, _currentSong);
	}

	private void dispatchSongPlay()
	{
		dispatcher.Dispatch(SongPlayerStartEvent.SONGPLAYER_START, _currentSong);
	}

	public string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Play State: ");
		stringBuilder.Append(songPlayerState);
		return stringBuilder.ToString();
	}

	public void StartGenreLoops(int genreID)
	{
		for (int i = 0; i < 25; i++)
		{
			AudioClip audioClip = genreAudioManager.getAudioClip(genreID, i);
			if (audioClip != null)
			{
				AudioSource audioSource = AudioClipPlayer.PlaySoundClip(audioClip, MuteState.ON, LoopState.ON);
				audioSource.Pause();
				audioSource.timeSamples = 0;
			}
		}
		for (int j = 0; j < 25; j++)
		{
			AudioClip audioClip2 = genreAudioManager.getAudioClip(genreID, j);
			if (audioClip2 != null)
			{
				AudioSource audioSource2 = AudioClipPlayer.PlaySoundClip(audioClip2, MuteState.ON, LoopState.ON);
				audioSource2.Play();
			}
		}
	}

	public void TrackOneShotsForMemoryUnload(int genreID)
	{
		for (int i = 25; i < 40; i++)
		{
			AudioClipPlayer.TrackAudioClipForMemoryUnload(genreAudioManager.getAudioClip(genreID, i));
		}
	}
}
