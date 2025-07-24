using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowPopupCommand : EventCommand
	{
		public override void Execute()
		{
			ShowPopupCommandPayload showPopupCommandPayload = (ShowPopupCommandPayload)base.evt.data;
			GameObject gameObject = Object.Instantiate(Resources.Load(showPopupCommandPayload.PopupPath)) as GameObject;
			gameObject.transform.SetParent(showPopupCommandPayload.ParentObject.transform, worldPositionStays: false);
		}
	}
}
