using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class ShowReferralStoreCommand : EventCommand
	{
		public override void Execute()
		{
			ReferralStoreManager.Show();
		}
	}
}
