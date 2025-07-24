using SoundStudio.Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class SongEditCommand : EventCommand
	{
		public override void Execute()
		{
			SongVO song = (SongVO)base.evt.data;
			createEditPrefab(song);
		}

		private void createEditPrefab(SongVO song)
		{
			GameObject gameObject = Object.Instantiate(loadEditSongPrefab(), new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
			GameObject gameObject2 = GameObject.Find("Canvas");
			gameObject.transform.SetParent(gameObject2.transform, worldPositionStays: false);
			EditSongView editSongView = gameObject.GetComponent("EditSongView") as EditSongView;
			editSongView.loadSong(song);
		}

		private GameObject loadEditSongPrefab()
		{
			return Resources.Load("Prefabs/PopupEditSong") as GameObject;
		}
	}
}
