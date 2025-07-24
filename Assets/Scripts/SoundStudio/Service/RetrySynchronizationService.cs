using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using UnityEngine;

namespace SoundStudio.Service
{
	public class RetrySynchronizationService
	{
		public const float TIMER_INTERVAL_SECONDS = 30f;

		private Timer timer;

		private bool paused;

		private MonoBehaviour root;

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher
		{
			get;
			set;
		}

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView
		{
			get;
			set;
		}

		[PostConstruct]
		public void Initialize()
		{
			if (root == null)
			{
				root = contextView.GetComponent<SoundStudioRoot>();
			}
			if (root == null)
			{
				throw new InvalidOperationException("The root was not set!");
			}
			timer = new Timer(30f, true, OnTimerTick);
			dispatcher.AddListener(SoundStudioEvent.PAUSE_RETRY_SERVICE, OnPauseService);
			dispatcher.AddListener(SoundStudioEvent.RESUME_RETRY_SERVICE, OnResumeService);
		}

		private void OnResumeService(IEvent payload)
		{
			paused = false;
		}

		private void OnPauseService(IEvent payload)
		{
			paused = true;
		}

		public void Start()
		{
			try
			{
				root.StartCoroutine(timer.Start());
			}
			catch (Exception)
			{
			}
		}

		private void OnTimerTick()
		{
			if (!paused)
			{
				dispatcher.Dispatch(SoundStudioEvent.PERFORM_CACHED_ACTIONS);
			}
		}

		public void Stop()
		{
			timer.Stop();
		}
	}
}
