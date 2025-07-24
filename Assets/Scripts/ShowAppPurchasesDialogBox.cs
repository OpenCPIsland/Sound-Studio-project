using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

internal class ShowAppPurchasesDialogBox : View
{
	public Button okButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		okButton.gameObject.SetActive(value: true);
		HardwareBackButtonDispatcher.SetTargetClickHandler(okButton, visible: false);
	}

	public void OnClickOkay()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		ContextDispatcher.Dispatch(SoundStudioEvent.SHOW_APP_USES_DATA_CLOSED);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
