using System.Collections;

namespace Disney.ClubPenguin.Service.Friends
{
	public interface IFriendsManager
	{
		IEnumerable Invitations
		{
			get;
		}

		int InvitationCount
		{
			get;
		}

		IEnumerable Friends
		{
			get;
		}

		int FriendCount
		{
			get;
		}

		event OnRefreshFriendsEvent OnRefreshFriends;

		event OnFoundEvent OnFound;

		void AcceptInvitation(string aName);

		void DeclineInvitation(string aName);

		void FindUser(string aName);

		void Invite();

		void Unfriend(string aName);

		void SetTrustedFlags(string[] aSwids);

		int TrustedFriendCount();

		Friend FindFriendByName(string aName);

		Friend FindFriendBySwid(string aSwid);

		void ClearFriendsCache();

		void LoadFriendsCache();

		Friend GenerateFriend(string aName, string aSwid);
	}
}
