using SoundStudio.Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowMembershipPromptCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			GameObject gameObject = base.evt.data as GameObject;
			GameObject gameObject2 = Object.Instantiate(Resources.Load("Prefabs/MemberCollectGenre")) as GameObject;
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			gameObject2.GetComponent<MembershipPromptView>().SetGenreText(application.CurrentGenre.id);
		}
	}
}
