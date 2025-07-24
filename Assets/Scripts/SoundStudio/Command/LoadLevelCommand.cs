using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoundStudio.Command
{
	public class LoadLevelCommand : EventCommand
	{
		public const double MIN_LOAD_TIME = 100.0;

		private static DateTime lastLoadTime = new DateTime(2005, 10, 24);

		private string LastScene = null;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			DateTime now = DateTime.Now;
			if (new TimeSpan(now.Ticks - lastLoadTime.Ticks).TotalMilliseconds > 100.0)
			{
				lastLoadTime = now;
				if (base.evt.data != null)
				{
					SceneNames sceneNames = (SceneNames)(int)base.evt.data;
					if(!string.IsNullOrEmpty(application.CurrentLevel)) {
						SceneManager.UnloadSceneAsync(application.CurrentLevel);
					}
					if (!application.CurrentLevel.Equals(sceneNames.ToString()))
					{
						GameObject gameObject = GameObject.Find("SceneParent");
						SceneManager.LoadScene(sceneNames.ToString(), LoadSceneMode.Additive);
						application.CurrentLevel = sceneNames.ToString();
						LastScene = sceneNames.ToString();
						if (gameObject != null)
						{
							UnityEngine.Object.Destroy(gameObject);
						}
					}
				}
			}
			Release();
		}
	}
}
