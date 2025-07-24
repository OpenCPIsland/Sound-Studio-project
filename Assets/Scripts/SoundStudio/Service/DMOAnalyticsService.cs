using Disney.DMOAnalytics;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Service
{
	public class DMOAnalyticsService : IAnalyticsService
	{
		public string DMOAnalyticsKey = "167BD86B-3162-4E36-A414-957098A2CE9E";

		public string DMOAnalyticsSecret = "88899AF1-4D72-462E-A84C-084BA241032F";

		public string ApplicationBundleIdentifer = "com.disney.clubpenguinsoundstudio";

		public string ApplicationVersion = "1.0";

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
		}

		public void Flush()
		{
			DMOAnalytics.SharedAnalytics.FlushAnalyticsQueue();
		}

		public void LogGameAction(Dictionary<string, object> gameData)
		{
			DMOAnalytics.SharedAnalytics.LogGameAction(gameData);
		}

		public void LogAnalyticsEventWithContext(string eventName, Dictionary<string, object> dataDetails)
		{
			DMOAnalytics.SharedAnalytics.LogAnalyticsEventWithContext(eventName, dataDetails);
		}

		public void PauseAnalytics()
		{
			DMOAnalytics.SharedAnalytics.CanUseNetwork = false;
		}

		public void ResumeAnalytics()
		{
			DMOAnalytics.SharedAnalytics.CanUseNetwork = true;
		}

		public void StartAnaylitics()
		{
			DMOAnalyticsKey = "A9FE18BD-7216-4120-8C84-0805795B7432";
			DMOAnalyticsSecret = "BC31A833-8597-4905-AC61-2332E1D5AEDC";
			DMOAnalytics.SharedAnalytics.initWithAnalyticsKeySerect(root, DMOAnalyticsKey, DMOAnalyticsSecret);
			DMOAnalytics.SharedAnalytics.LogAppStart();
		}

		public void ApplicationPause(PauseState pauseState)
		{
			switch (pauseState)
			{
			case PauseState.Paused:
				DMOAnalytics.SharedAnalytics.LogAppBackground();
				break;
			case PauseState.UnPaused:
				DMOAnalytics.SharedAnalytics.LogAppForeground();
				break;
			}
		}

		public void LogApplicationQuit()
		{
			DMOAnalytics.SharedAnalytics.LogAppEnd();
		}
	}
}
