using SoundStudio.Event;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowShareConfirmCommand : EventCommand
	{
		public override void Execute()
		{
			GameObject parentObject = (GameObject)base.evt.data;
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			createConfirmPrefab(parentObject);
		}

		private void createConfirmPrefab(GameObject parentObject)
		{
			GameObject gameObject = Object.Instantiate(loadShareConfirmPrefab(), new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
			gameObject.transform.SetParent(parentObject.transform, worldPositionStays: false);
		}

		private GameObject loadShareConfirmPrefab()
		{
			return Resources.Load("Prefabs/ShareConfirmPrefab") as GameObject;
		}
	}
}
