using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowRecordOopsCommand : EventCommand
	{
		public override void Execute()
		{
			GameObject gameObject = (GameObject)base.evt.data;
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/NonMemberRecordPanel")) as GameObject;
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			RecordOopsView recordOopsView = gameObject2.GetComponent("RecordOopsView") as RecordOopsView;
			recordOopsView.canvas = gameObject;
		}
	}
}
