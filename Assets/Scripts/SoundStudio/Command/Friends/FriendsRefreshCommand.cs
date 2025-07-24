using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;

namespace SoundStudio.Command.Friends
{
	internal class FriendsRefreshCommand : EventCommand
	{
		[Inject]
		public FriendsClientService FriendsClientService
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
		}
	}
}
