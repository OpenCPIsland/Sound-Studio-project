using Disney.ClubPenguin.Service.Friends;
using System.Collections;
using System.Collections.Generic;

namespace Disney.ClubPenguin.Service.FriendsInternal
{
	public class FriendsManagerInvitations : IFriendsManagerInvitations
	{
		private IInternalFriendsManager manager;

		private IFriendsManagerFriends friendsManagerFriends;

		private List<string> invitations;

		private string found = string.Empty;

		public FriendsManagerInvitations(IInternalFriendsManager aManager, IFriendsManagerFriends aFriends)
		{
			manager = aManager;
			friendsManagerFriends = aFriends;
			invitations = new List<string>();
		}

		public IEnumerable GetInvitations()
		{
			foreach (string invitation in invitations)
			{
				yield return invitation;
			}
		}

		public int Count()
		{
			return invitations.Count;
		}

		public void Clear()
		{
			invitations.Clear();
		}

		public void Accept(string aName)
		{
			invitations.Remove(aName);
			manager.SendAcceptInvitation(aName);
		}

		public void Decline(string aName)
		{
			invitations.Remove(aName);
			manager.SendDeclineInvitation(aName);
		}

		public void Find(string aName)
		{
			found = string.Empty;
			manager.SendFindUser(aName);
		}

		public void Invite()
		{
			if (found != string.Empty)
			{
				manager.SendInvite(found);
			}
		}

		public void OnFound(string aSwid)
		{
			found = aSwid;
			if (aSwid == string.Empty)
			{
				manager.SendOnFound(FindResults.NOT_FOUND);
			}
			else if (manager.GetInvitationSent(aSwid))
			{
				manager.SendOnFound(FindResults.PENDING);
			}
			else if (friendsManagerFriends.FindFriendBySwid(aSwid) != null)
			{
				manager.SendOnFound(FindResults.FRIEND);
			}
			else
			{
				manager.SendOnFound(FindResults.FOUND);
			}
		}

		public void OnInvitation(string aName)
		{
			invitations.Add(aName);
			manager.SendOnRefreshFriends();
		}
	}
}
