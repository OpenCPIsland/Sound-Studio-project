using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	internal class ShowAppUsesDataCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			Retain();
			if (!application.HasSeenDataAndPurchaseWarningPrompt)
			{
				base.dispatcher.AddListener(SoundStudioEvent.SHOW_APP_USES_DATA_CLOSED, OnShowAppUsesDataClosed);
				GameObject original = Resources.Load("Prefabs/PurchaseAndDataPanel") as GameObject;
				GameObject gameObject = Object.Instantiate(original, Vector3.zero, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
				gameObject.name = "MobilePurchasesAndData";
				if (gameObject != null)
				{
					Transform transform = base.evt.data as Transform;
					if (transform != null)
					{
						gameObject.transform.SetParent(transform, worldPositionStays: false);
					}
				}
			}
			else
			{
				Release();
			}
		}

		private void OnShowAppUsesDataClosed()
		{
			base.dispatcher.RemoveListener(SoundStudioEvent.SHOW_APP_USES_DATA_CLOSED, OnShowAppUsesDataClosed);
			Release();
		}
	}
}
