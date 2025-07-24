using System.Collections.Generic;
using UnityEngine;

namespace Disney.DMOAnalytics.Framework
{
	public class DMOBinderAndroid : IDMOAnalyticsBinder
	{
		private static AndroidJavaObject _plugin;

		private static AndroidJavaObject playerActivityContext;

		public DMOBinderAndroid()
		{
			if (Application.platform == RuntimePlatform.Android && (playerActivityContext == null || _plugin == null))
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					playerActivityContext = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dismo.AnalyticsPlugin"))
				{
					if (androidJavaClass2 != null)
					{
						_plugin = androidJavaClass2.CallStatic<AndroidJavaObject>("instance", new object[0]);
						if (_plugin != null)
						{
							_plugin.Call("setContext", playerActivityContext);
						}
					}
				}
			}
		}

		public void Init(MonoBehaviour gameObj, string appKey, string appSecret)
		{
			if (_plugin != null)
			{
				_plugin.Call("init", appKey, appSecret);
			}
		}

		public void LogEvent(string appEvent)
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsLogEvent", appEvent);
			}
		}

		public void LogAppStart()
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsLogAppStart");
			}
		}

		public void LogAppForeground()
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsLogAppForeground");
			}
		}

		public void LogAppBackground()
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsLogAppBackground");
			}
		}

		public void LogAppEnd()
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsLogAppEnd");
			}
		}

		public void LogEventWithContext(string eventName, Dictionary<string, object> data)
		{
			if (_plugin != null)
			{
				string stringFromDictionary = DMOAnalyticsHelper.GetStringFromDictionary(data);
				_plugin.Call("dmoAnalyticsLogEventWithContext", eventName, stringFromDictionary);
			}
		}

		public void FlushAnalyticsQueue()
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsflushAnalyticsQueue");
			}
		}

		public void LogGameAction(Dictionary<string, object> gameData)
		{
			if (_plugin != null)
			{
				string stringFromDictionary = DMOAnalyticsHelper.GetStringFromDictionary(gameData);
				_plugin.Call("dmoAnalyticsLogGameAction", stringFromDictionary);
			}
		}

		public void LogMoneyAction(Dictionary<string, object> moneyData)
		{
			if (_plugin != null)
			{
				string stringFromDictionary = DMOAnalyticsHelper.GetStringFromDictionary(moneyData);
				_plugin.Call("dmoAnalyticsLogMoneyAction", stringFromDictionary);
			}
		}

		public void SetDebugLogging(bool isEnable)
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsSetDebugLogging", isEnable);
			}
		}

		public void SetCanUseNetwork(bool isEnable)
		{
			if (_plugin != null)
			{
				_plugin.Call("dmoAnalyticsSetCanUseNetwork", isEnable);
			}
		}
	}
}
