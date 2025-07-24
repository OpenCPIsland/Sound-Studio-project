using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowProtectedWebPageCommand : EventCommand
	{
		public override void Execute()
		{
			Application.OpenURL((base.evt.data as ShowProtectedWebPagePayload).Url);
		}
	}
}
