using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GetFriendsPanelView : View
{
	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public void OnAppButtonClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		GameObject gameObject = GameObject.Find("Canvas");
		Object component = gameObject.GetComponent<RectTransform>();
		dispatcher.Dispatch(ShowCPAppEvent.SHOW_CP_APP, component);
	}
}
