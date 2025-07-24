using Disney.ClubPenguin.Login.Authentication;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

public class GuestOopsPopupView : View
{
	public LoginContext loginContextPrefab;

	public GameObject canvas;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	[Inject]
	public ApplicationState applicationState
	{
		get;
		set;
	}

	public void OnBackClick()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void OnLoginClick()
	{
		GameObject gameObject = GameObject.Find("LoginConvas");
		List<object> list = new List<object>();
		list.Add(loginContextPrefab);
		list.Add(canvas);
		dispatcher.Dispatch(ShowLoginEvent.SHOW_LOGIN, list);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
