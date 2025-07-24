using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine.UI;

public class LoginProgressView : View
{
	public Text ProgressText;

	[Inject]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public void SetProgressText(string text)
	{
		ProgressText.text = text;
	}
}
