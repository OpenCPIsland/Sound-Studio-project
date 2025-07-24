using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowGuestOopsPopupCommand : EventCommand
	{
		public override void Execute()
		{
			GameObject canvasObject = (base.evt.data as StartScreenView).canvasObject;
			createGuestOopsPrefab(canvasObject);
		}

		private void createGuestOopsPrefab(GameObject parentObject)
		{
			GameObject gameObject = Object.Instantiate(LoadGuestOopsPrefab(), new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
			gameObject.transform.SetParent(parentObject.transform, worldPositionStays: false);
			GuestOopsPopupView guestOopsPopupView = gameObject.GetComponent("GuestOopsPopupView") as GuestOopsPopupView;
			guestOopsPopupView.canvas = parentObject;
		}

		private GameObject LoadGuestOopsPrefab()
		{
			return Resources.Load("Prefabs/GuestOopsPopupPrefab") as GameObject;
		}
	}
}
