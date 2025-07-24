using Disney.ClubPenguin.Service.Friends;

namespace Disney.ClubPenguin.Service.FriendsInternal
{
	public interface IInternalFriendsManager
	{
		void SendFindUser(string aName);

		void SendInvite(string aSwid);

		void SendAcceptInvitation(string aName);

		void SendDeclineInvitation(string aName);

		bool GetInvitationSent(string aSwid);

		void SendRemoveContact(string aSwid);

		Friend GenerateFriend(string aName, string aSwid);

		void SendOnRefreshFriends();

		void SendOnFound(FindResults aResults);
	}
}
