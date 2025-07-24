#define ANDROID_ONLY
using Disney.ClubPenguin.CPModuleUtils;
using System.Diagnostics;
using UnityEngine;

namespace SoundStudio
{
	public class ApplicationHelper
	{
		public static void OpenCPApp()
		{
			OpenAppAndroid();
		}

		[Conditional("UNITY_EDITOR")]
		private static void OpenAppEditor()
		{
			Application.OpenURL("http://www.clubpenguin.com");
		}

		[Conditional("IOS_ONLY")]
		private static void OpenAppIOS()
		{
			new OpenAppOnDeviceCMD("clubpenguin://", "http://itunes.apple.com/app/id505544063").Execute();
		}

		[Conditional("ANDROID_ONLY")]
		private static void OpenAppAndroid()
		{
			new OpenAppOnDeviceCMD("com.disney.cpcompanion_goo", "https://play.google.com/store/apps/details?id=com.disney.cpcompanion_goo").Execute();
		}
	}
}
