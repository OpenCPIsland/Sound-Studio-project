using Disney.ClubPenguin.Login.Authentication;
using Disney.ClubPenguin.Service.MWS;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using System;

namespace SoundStudio.Command
{
	public class LogoutCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public IMWSClient mwsClient
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
			try
			{
				ForgetPlayerPasswordCMD forgetPlayerPasswordCMD = new ForgetPlayerPasswordCMD(application.currentPlayer.DisplayName);
				forgetPlayerPasswordCMD.Execute();
				mwsClient.ClearAuthToken();
				FriendsClientService.Close();
			}
			catch (Exception)
			{
			}
			LoginGuest();
		}

		private void LoginGuest()
		{
			PlayerAccountVO playerAccountVO = new PlayerAccountVO();
			playerAccountVO.DisplayName = "guest";
			playerAccountVO.AccountStatus = MembershipStatus.GUEST;
			playerAccountVO.MySongsState = MySongsStatus.COMPLETE;
			application.currentPlayer = playerAccountVO;
			application.songData = new SongListVO();
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_PLAYER);
		}
	}
}
