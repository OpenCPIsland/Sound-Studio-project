using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowNoSongsPopupCommand : EventCommand
	{
		public override void Execute()
		{
			GameObject gameObject = (GameObject)base.evt.data;
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/NoSongsPrefab")) as GameObject;
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
		}
	}
}
