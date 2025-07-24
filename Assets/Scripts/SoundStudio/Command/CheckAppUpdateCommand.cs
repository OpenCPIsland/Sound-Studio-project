using Disney.ClubPenguin.ForcedUpdate;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	internal class CheckAppUpdateCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public IForcedUpdateManager ForcedUpdateManager
		{
			get;
			set;
		}

		public override void Execute()
		{
			GameObject gameObject = base.evt.data as GameObject;
			if (gameObject == null)
			{
				gameObject = GameObject.Find("ForcedUpdateCanvas");
			}
			if (!(gameObject != null))
			{
				return;
			}
			TimeSpan t = new TimeSpan(0, 10, 0);
			DateTime now = DateTime.Now;
			if (application.LastAppUpdateCheck + t < now)
			{
				application.LastAppUpdateCheck = now;
				long value = 0L;
				if (application.currentPlayer != null)
				{
					value = application.currentPlayer.ID;
				}
				ForcedUpdateManager.PlayerId = value;
				ForcedUpdateManager.Init(gameObject, "SoundStudio", "1.2", "https://play.google.com/store/apps/details?id=com.disney.clubpenguinsoundstudio_goo", 0);
				ForcedUpdateManager.CheckVersion();
			}
		}
	}
}
