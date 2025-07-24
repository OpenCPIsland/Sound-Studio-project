using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitConfirmView : View
{
	internal const string CONFIRM_EVENT = "confirm_quit";

	internal const string CANCEL_EVENT = "cancel_quit";

	internal const string BACK_CLICK_EVENT = "back_click_event";

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	private void Update()
	{
		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			ContextDispatcher.Dispatch("back_click_event");
		}
	}

	public void OnCancelClick()
	{
		ContextDispatcher.Dispatch("cancel_quit");
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
	}

	public void OnConfirmClick()
	{
		ContextDispatcher.Dispatch("confirm_quit");
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
	}
}
