using System.Collections.Generic;

namespace SoundStudio.Service
{
	public interface IAnalyticsService
	{
		void ApplicationPause(PauseState pauseState);

		void Flush();

		void LogApplicationQuit();

		void LogAnalyticsEventWithContext(string eventName, Dictionary<string, object> dataDetails);

		void LogGameAction(Dictionary<string, object> gameData);

		void StartAnaylitics();

		void PauseAnalytics();

		void ResumeAnalytics();
	}
}
