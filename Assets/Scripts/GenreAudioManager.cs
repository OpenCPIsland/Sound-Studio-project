using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenreAudioManager
{
	private Dictionary<GenreVO, Dictionary<SoundVO, AudioClip>> audioCollection;

	[Inject]
	public ApplicationState applicationState
	{
		get;
		set;
	}

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	public GenreAudioManager()
	{
		audioCollection = new Dictionary<GenreVO, Dictionary<SoundVO, AudioClip>>();
	}

	public void loadGenreAudio(GenreVO genre, ICollection<int> soundIDs = null)
	{
		if (genre == null)
		{
			throw new InvalidOperationException("The GenreVO cannot be null");
		}
		Dictionary<SoundVO, AudioClip> dictionary = new Dictionary<SoundVO, AudioClip>();
		foreach (SoundVO sound in genre.SoundList)
		{
			if (soundIDs == null || soundIDs.Contains(sound.id))
			{
				AudioClip audioClip = AudioClipPlayer.FindAudioClipInMemory(sound.AudioPath);
				if (audioClip == null)
				{
					audioClip = (Resources.Load(sound.AudioPath) as AudioClip);
					if (audioClip == null)
					{
						throw new InvalidOperationException("The configuration for the audioclip is wrong. Cannot load audio for: " + sound);
					}
				}
				dictionary.Add(sound, audioClip);
				sound.OnSoundStateUpdated += soundVO_StateChange_Handler;
			}
		}
		audioCollection[genre] = dictionary;
	}

	public IEnumerator LoadGenreAudioAsync(GenreVO genre, ICollection<int> soundIDs = null)
	{
		if (genre == null)
		{
			throw new InvalidOperationException("The GenreVO cannot be null");
		}
		ContextDispatcher.Dispatch(SoundStudioEvent.LOAD_GENRE_AUDIO_START);
		Dictionary<SoundVO, AudioClip> genreCollection = (!audioCollection.ContainsKey(genre)) ? new Dictionary<SoundVO, AudioClip>() : audioCollection[genre];
		foreach (SoundVO soundVO in genre.SoundList)
		{
			if (soundIDs == null || soundIDs.Contains(soundVO.id))
			{
				AudioClip audioClip = AudioClipPlayer.FindAudioClipInMemory(soundVO.AudioPath);
				if (audioClip == null)
				{
					ResourceRequest request = Resources.LoadAsync(soundVO.AudioPath, typeof(AudioClip));
					yield return null;
					audioClip = (request.asset as AudioClip);
					if (audioClip == null)
					{
						throw new InvalidOperationException("The configuration for the audioclip is wrong. Cannot load audio for: " + soundVO);
					}
				}
				genreCollection[soundVO] = audioClip;
				UnityEngine.Debug.Log("Audio clip name " + audioClip.name + " and length is " + audioClip.length + " and samples are " + audioClip.samples);
				soundVO.OnSoundStateUpdated += soundVO_StateChange_Handler;
			}
		}
		audioCollection[genre] = genreCollection;
		ContextDispatcher.Dispatch(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE);
	}

	public AudioClip getAudioClip(SoundVO soundVO)
	{
		foreach (Dictionary<SoundVO, AudioClip> value in audioCollection.Values)
		{
			foreach (KeyValuePair<SoundVO, AudioClip> item in value)
			{
				if (soundVO == item.Key)
				{
					return item.Value;
				}
			}
		}
		return null;
	}

	public AudioClip getAudioClip(int genreID, int soundID)
	{
		GenreVO genreVO = null;
		foreach (GenreVO key in audioCollection.Keys)
		{
			if (key.id == genreID)
			{
				genreVO = key;
			}
		}
		if (genreVO == null)
		{
		}
		foreach (SoundVO key2 in audioCollection[genreVO].Keys)
		{
			if (key2.id == soundID)
			{
				return audioCollection[genreVO][key2];
			}
		}
		return null;
	}

	private void soundVO_StateChange_Handler(SoundVO soundVO)
	{
		LoopState loopState = LoopState.OFF;
		if (soundVO.IsLoop())
		{
			loopState = LoopState.ON;
		}
		switch (soundVO.State)
		{
		case SoundPlayState.PAUSED:
			break;
		case SoundPlayState.MUTED:
			AudioClipPlayer.PlaySoundClip(getAudioClip(soundVO), MuteState.ON, loopState);
			break;
		case SoundPlayState.PLAYING:
			AudioClipPlayer.PlaySoundClip(getAudioClip(soundVO), MuteState.OFF, loopState);
			break;
		case SoundPlayState.STOPPED:
			AudioClipPlayer.StopSoundClip(getAudioClip(soundVO));
			break;
		}
	}
}
