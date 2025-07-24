using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

namespace SoundStudio
{
	public class SoundStudioRoot : ContextView
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher
		{
			get;
			set;
		}

		private void Awake()
		{
			context = new SoundStudioContext(this);
			dispatcher.Dispatch(SoundStudioEvent.APPLICATION_AWAKE);
		}

		public void OnApplicationPause(bool paused)
		{
			dispatcher.Dispatch(SoundStudioEvent.APPLICATION_PAUSED_STATE_CHANGE, (!paused) ? PauseState.UnPaused : PauseState.Paused);
			if (!paused)
			{
				AudioListener.pause = application.IsMute;
			}
		}

		public void OnApplicationQuit()
		{
			dispatcher.Dispatch(SoundStudioEvent.APPLICATION_QUIT);
		}

		public void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				AudioListener.pause = application.IsMute;
			}
			else
			{
				AudioListener.pause = true;
			}
		}
	}
}
