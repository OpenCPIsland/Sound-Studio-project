using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

internal class ModalPopupView : View
{
	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public void OnClosePopupClick()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
