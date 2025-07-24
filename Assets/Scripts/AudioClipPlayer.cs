using System;
using System.Collections.Generic;
using UnityEngine;

public static class AudioClipPlayer
{
	private static Dictionary<AudioClip, AudioSource> audioSourceDictionary = new Dictionary<AudioClip, AudioSource>();

	private static AudioListener audioListener;

	private static HashSet<AudioClip> uniqueAudioClips = new HashSet<AudioClip>();

	public static void TrackAudioClipForMemoryUnload(AudioClip audioClip)
	{
		if (audioClip != null)
		{
			uniqueAudioClips.Add(audioClip);
		}
	}

	public static bool IsAudioClipInMemory(string path)
	{
		foreach (AudioClip uniqueAudioClip in uniqueAudioClips)
		{
			if (path.Contains(uniqueAudioClip.name))
			{
				return true;
			}
		}
		return false;
	}

	public static AudioClip FindAudioClipInMemory(string path)
	{
		foreach (AudioClip uniqueAudioClip in uniqueAudioClips)
		{
			if (path.Contains(uniqueAudioClip.name))
			{
				return uniqueAudioClip;
			}
		}
		return null;
	}

	public static AudioSource PlaySoundClip(AudioClip clip, MuteState muteState, LoopState loopState, SoundPauseState soundPauseState = SoundPauseState.UNPAUSED)
	{
		return PlaySoundClip(clip, 1f, 1f, muteState == MuteState.ON, loopState == LoopState.ON, soundPauseState == SoundPauseState.PAUSED);
	}

	public static AudioSource PlaySoundClip(AudioClip clip, float volume, float pitch, bool mute, bool loop, bool paused)
	{
		if (clip == null)
		{
			throw new InvalidOperationException("Cannot play a null audio clip!");
		}
		TrackAudioClipForMemoryUnload(clip);
		if (volume > 0.01f)
		{
			findAudioListener();
			if (audioListener != null && audioListener.enabled && audioListener.gameObject.activeInHierarchy)
			{
				AudioSource audioSource = findAudioSource(clip);
				audioSource.clip = clip;
				audioSource.pitch = pitch;
				audioSource.loop = loop;
				audioSource.volume = volume;
				audioSource.mute = mute;
				if (loop)
				{
					if (!audioSource.isPlaying)
					{
						audioSource.Play();
					}
				}
				else if (paused)
				{
					audioSource.Pause();
				}
				else
				{
					audioSource.Play();
				}
				return audioSource;
			}
		}
		return null;
	}

	public static void ClearMemoryAsNeeded()
	{
		int num = 1;
		if (SystemInfo.systemMemorySize > 600)
		{
			num *= 2;
		}
		if (GetAudioClipCount() > 40 * num)
		{
			ClearAudio();
		}
	}

	public static int GetAudioClipCount()
	{
		return uniqueAudioClips.Count;
	}

	public static void ClearAudio()
	{
		HashSet<AudioSource> hashSet = new HashSet<AudioSource>();
		foreach (AudioSource value in audioSourceDictionary.Values)
		{
			hashSet.Add(value);
		}
		foreach (AudioSource item in hashSet)
		{
			UnityEngine.Object.DestroyImmediate(item);
		}
		foreach (AudioClip uniqueAudioClip in uniqueAudioClips)
		{
			Resources.UnloadAsset(uniqueAudioClip);
		}
		audioSourceDictionary.Clear();
		uniqueAudioClips.Clear();
		Resources.UnloadUnusedAssets();
	}

	public static void StopAllAudio()
	{
		if ((bool)audioListener)
		{
			AudioSource[] components = audioListener.GetComponents<AudioSource>();
			foreach (AudioSource audioSource in components)
			{
				audioSource.Stop();
			}
		}
	}

	private static AudioSource findAudioSource(AudioClip clip)
	{
		AudioSource audioSource = null;
		if (audioSourceDictionary.ContainsKey(clip))
		{
			audioSource = audioSourceDictionary[clip];
		}
		if (audioSource == null)
		{
			AudioSource[] components = audioListener.GetComponents<AudioSource>();
			foreach (AudioSource audioSource2 in components)
			{
				if (audioSource2.clip.GetHashCode().Equals(clip.GetHashCode()))
				{
					audioSource = audioSource2;
					break;
				}
			}
		}
		if (audioSource == null)
		{
			audioSource = audioListener.gameObject.AddComponent<AudioSource>();
			audioSourceDictionary[clip] = audioSource;
		}
		return audioSource;
	}

	private static void findAudioListener()
	{
		if (audioListener == null)
		{
			audioListener = (UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
			if (audioListener == null)
			{
				GameObject gameObject = GameObject.Find("ContextView");
				if (gameObject != null)
				{
					audioListener = gameObject.AddComponent<AudioListener>();
				}
			}
		}
		if (audioListener == null)
		{
			throw new InvalidOperationException("An audio listener was not found and/or could not be added to the scene.");
		}
		UnityEngine.Object.DontDestroyOnLoad(audioListener.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(audioListener);
	}

	public static AudioSource StopSoundClip(AudioClip clip)
	{
		findAudioListener();
		AudioSource audioSource = findAudioSource(clip);
		audioSource.clip = clip;
		audioSource.Stop();
		return audioSource;
	}

	public static AudioSource PauseSoundClip(AudioClip clip)
	{
		findAudioListener();
		AudioSource audioSource = findAudioSource(clip);
		audioSource.clip = clip;
		audioSource.Pause();
		return audioSource;
	}
}
