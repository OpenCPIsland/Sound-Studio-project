using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class ShareConfirmView : View
{
	public Button invisibleButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		invisibleButton.gameObject.SetActive(value: true);
		HardwareBackButtonDispatcher.SetTargetClickHandler(invisibleButton, visible: false);
	}

	public void OnClick()
	{
		dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
