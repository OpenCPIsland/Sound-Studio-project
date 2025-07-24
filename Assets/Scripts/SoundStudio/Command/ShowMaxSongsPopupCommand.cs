using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowMaxSongsPopupCommand : EventCommand
	{
		public override void Execute()
		{
			GameObject parentObject = (GameObject)base.evt.data;
			createMaxSongsPrefab(parentObject);
		}

		private void createMaxSongsPrefab(GameObject parentObject)
		{
			GameObject gameObject = Object.Instantiate(loadMaxSongsPrefab(), new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
			gameObject.transform.SetParent(parentObject.transform, worldPositionStays: false);
		}

		private GameObject loadMaxSongsPrefab()
		{
			return Resources.Load("Prefabs/MaxSongsPopupPrefab") as GameObject;
		}
	}
}
