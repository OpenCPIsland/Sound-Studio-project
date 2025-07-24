using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RecordCountdown : View
{
	public Text countdownText;

	private AudioClip audioClip3;

	private AudioClip audioClip2;

	private AudioClip audioClip1;

	private int currentCount = 3;

	private AudioClip theSoundClip;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Start()
	{
		base.Start();
		audioClip1 = (Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_CountIn01") as AudioClip);
		audioClip2 = (Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_CountIn02") as AudioClip);
		audioClip3 = (Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_CountIn03") as AudioClip);
		if (audioClip1 == null || audioClip2 == null || audioClip3 == null)
		{
			completeCountdown();
			return;
		}
		AudioClipPlayer.TrackAudioClipForMemoryUnload(audioClip1);
		AudioClipPlayer.TrackAudioClipForMemoryUnload(audioClip2);
		AudioClipPlayer.TrackAudioClipForMemoryUnload(audioClip3);
		tick();
		InvokeRepeating("tick", 0.8f, 0.8f);
	}

	private void tick()
	{
		countdownText.text = currentCount.ToString();
		switch (currentCount)
		{
		case 3:
			AudioClipPlayer.PlaySoundClip(audioClip3, MuteState.OFF, LoopState.OFF);
			break;
		case 2:
			AudioClipPlayer.PlaySoundClip(audioClip2, MuteState.OFF, LoopState.OFF);
			break;
		case 1:
			AudioClipPlayer.PlaySoundClip(audioClip1, MuteState.OFF, LoopState.OFF);
			break;
		}
		if (currentCount-- < 1)
		{
			completeCountdown();
		}
	}

	private void completeCountdown()
	{
		CancelInvoke();
		ContextDispatcher.Dispatch(SoundStudioEvent.RECORD_COUNTDOWN_COMPLETE);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
