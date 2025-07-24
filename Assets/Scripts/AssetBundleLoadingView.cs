using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Globalization;
using UnityEngine.UI;

internal class AssetBundleLoadingView : View
{
	public Text percentageText;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Start()
	{
		base.Start();
		ContextDispatcher.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_PROGRESS, OnGetAssetBundleProgress);
	}

	private void OnGetAssetBundleProgress(IEvent payload)
	{
		FormatProgress((float)payload.data);
	}

	private void FormatProgress(float value)
	{
		if (percentageText != null)
		{
			percentageText.text = value.ToString("P0", CultureInfo.InvariantCulture);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		ContextDispatcher.RemoveListener(SoundStudioEvent.GET_ASSET_BUNDLE_PROGRESS, OnGetAssetBundleProgress);
	}
}
