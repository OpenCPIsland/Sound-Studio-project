using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class MaxSongPopupView : View
{
	public Button backButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	public void OnBackClick()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
	}

	public void OnSongListButtonClick()
	{
		ContextDispatcher.Dispatch(SongStopEvent.SONG_STOP);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		ContextDispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Songs);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
