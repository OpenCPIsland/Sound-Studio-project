using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class ChangeUserCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public FriendsClientService FriendsClientService
		{
			get;
			set;
		}

		public override void Execute()
		{
			FriendsClientService.Close();
			AuthData authData = base.evt.data as AuthData;
			if (authData != null)
			{
				LoginPenguin(authData);
			}
			else
			{
				LoginGuest();
			}
		}

		private void LoginPenguin(AuthData authData)
		{
			PlayerAccountVO playerAccountVO = new PlayerAccountVO();
			playerAccountVO.DisplayName = authData.Username;
			playerAccountVO.Username = authData.Username;
			playerAccountVO.Swid = authData.PlayerSwid;
			playerAccountVO.ID = authData.PlayerId;
			playerAccountVO.Password = "client";
			playerAccountVO.AuthToken = authData.AuthToken;
			playerAccountVO.AccountStatus = MembershipStatus.MEMBER;
			application.currentPlayer = playerAccountVO;
			application.songData = new SongListVO();
			base.dispatcher.Dispatch(LoadPlayerPurchasesEvent.LOAD_PLAYER_PURCHASES);
		}

		private void LoginGuest()
		{
			PlayerAccountVO playerAccountVO = new PlayerAccountVO();
			playerAccountVO.DisplayName = "guest";
			playerAccountVO.AccountStatus = MembershipStatus.GUEST;
			playerAccountVO.FriendCollectionStatus = FriendCollectionStatus.INITIALIZED;
			application.currentPlayer = playerAccountVO;
			application.songData = new SongListVO();
			base.dispatcher.Dispatch(LoadPlayerPurchasesEvent.LOAD_PLAYER_PURCHASES);
		}
	}
}
