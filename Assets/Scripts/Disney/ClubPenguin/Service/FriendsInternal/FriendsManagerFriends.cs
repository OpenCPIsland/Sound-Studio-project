using Disney.ClubPenguin.Service.Friends;
using System.Collections;
using System.Collections.Generic;

namespace Disney.ClubPenguin.Service.FriendsInternal
{
	public class FriendsManagerFriends : IFriendsManagerFriends
	{
		private IInternalFriendsManager manager;

		public Dictionary<string, Friend> friends;

		public FriendsManagerFriends(IInternalFriendsManager aManager)
		{
			manager = aManager;
			friends = new Dictionary<string, Friend>();
		}

		public IEnumerable GetFriends()
		{
			foreach (string key in friends.Keys)
			{
				yield return friends[key];
			}
		}

		public int Count()
		{
			return friends.Count;
		}

		public void Clear()
		{
			friends.Clear();
		}

		public void Unfriend(string aName)
		{
			Friend friend = FindFriendByName(aName);
			if (friend != null)
			{
				friends.Remove(friend.Swid);
				manager.SendRemoveContact(friend.Swid);
			}
		}

		public void SetTrustedFlags(string[] aSwids)
		{
			HashSet<string> hashSet = new HashSet<string>(aSwids);
			foreach (string key in friends.Keys)
			{
				Friend friend = friends[key];
				if (hashSet.Contains(friend.Swid))
				{
					friend.Trusted = true;
				}
				else
				{
					friend.Trusted = false;
				}
			}
			manager.SendOnRefreshFriends();
		}

		public int TrustedFriendCount()
		{
			int num = 0;
			foreach (string key in friends.Keys)
			{
				Friend friend = friends[key];
				if (friend.Trusted)
				{
					num++;
				}
			}
			return num;
		}

		public Friend FindFriendByName(string aName)
		{
			foreach (string key in friends.Keys)
			{
				Friend friend = friends[key];
				if (friend.Name == aName)
				{
					return friend;
				}
			}
			return null;
		}

		public Friend FindFriendBySwid(string aSwid)
		{
			if (friends.ContainsKey(aSwid))
			{
				return friends[aSwid];
			}
			return null;
		}

		public void OnAddFriend(string aSwid, string aName)
		{
			if (FindFriendBySwid(aSwid) == null)
			{
				friends.Add(aSwid, manager.GenerateFriend(aName, aSwid));
				manager.SendOnRefreshFriends();
			}
		}

		public void OnRemoveFriend(string aSwid)
		{
			Friend friend = FindFriendBySwid(aSwid);
			if (friend != null)
			{
				friends.Remove(friend.Swid);
				manager.SendOnRefreshFriends();
			}
		}
	}
}
