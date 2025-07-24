using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

internal class AssetBundleFailedDialogBox : View
{
	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	public void OnRetryDownloadClicked()
	{
		ContextDispatcher.Dispatch(SoundStudioEvent.RETRY_GET_ASSET_BUNDLE);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
