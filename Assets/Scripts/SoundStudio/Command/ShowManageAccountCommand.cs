using DevonLocalization.Core;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowManageAccountCommand : EventCommand
	{
		private const string MANAGE_ACCOUNT_URL_TOKEN = "soundstudio.settings.manageaccounturl";

		public override void Execute()
		{
			string tokenTranslation = Localizer.Instance.GetTokenTranslation("soundstudio.settings.manageaccounturl");
			Application.OpenURL(tokenTranslation);
		}
	}
}
