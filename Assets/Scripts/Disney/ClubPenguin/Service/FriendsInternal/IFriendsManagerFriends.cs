using Disney.ClubPenguin.Service.Friends;
using System.Collections;

namespace Disney.ClubPenguin.Service.FriendsInternal
{
	public interface IFriendsManagerFriends
	{
		IEnumerable GetFriends();

		int Count();

		void Clear();

		Friend FindFriendBySwid(string aSwid);
	}
}
