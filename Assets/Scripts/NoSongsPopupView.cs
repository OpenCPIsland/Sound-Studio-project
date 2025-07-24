using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class NoSongsPopupView : View
{
	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public void OnRecordButtonClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Genres);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
