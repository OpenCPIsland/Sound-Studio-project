using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GenericPopupView : View
{
	public Button closeButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		closeButton.gameObject.SetActive(value: true);
		HardwareBackButtonDispatcher.SetTargetClickHandler(closeButton, visible: false);
	}

	public void OnClickCloseButton()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
