using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RecordOopsView : View
{
	public GameObject canvas;

	public Button closeButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		HardwareBackButtonDispatcher.SetTargetClickHandler(closeButton, visible: false);
	}

	public void OnBackClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void OnStoreClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		Canvas canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
		dispatcher.Dispatch(ShowStoreEvent.SHOW_STORE, canvas.GetComponent<RectTransform>());
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void OnMoreClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		Canvas canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
		dispatcher.Dispatch(ShowAboutMembershipEvent.SHOW_ABOUT_MEMBERSHIP, canvas.GetComponent<RectTransform>());
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
