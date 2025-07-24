using Disney.ClubPenguin.Service.Friends;
using Disney.ClubPenguin.Service.MWS.Domain;
using System.Collections.Generic;

namespace SoundStudio.Model
{
	public class FriendsTrackCollection
	{
		private class FriendAndStatusWrapper
		{
			public Friend friend;

			public FriendTrackStatus status;

			public SoundStudioRadioTrackData soundStudioRadioTrackData;
		}

		private Dictionary<string, FriendAndStatusWrapper> swidToFriendWrapperMap = new Dictionary<string, FriendAndStatusWrapper>();

		public int Count => swidToFriendWrapperMap.Count;

		public void AddOrUpdateSoundStudioRadioTrackData(SoundStudioRadioTrackData soundStudioRadioTrackData)
		{
			if (swidToFriendWrapperMap.ContainsKey(soundStudioRadioTrackData.playerSwid))
			{
				FriendAndStatusWrapper friendAndStatusWrapper = swidToFriendWrapperMap[soundStudioRadioTrackData.playerSwid];
				friendAndStatusWrapper.soundStudioRadioTrackData = soundStudioRadioTrackData;
				friendAndStatusWrapper.status = FriendTrackStatus.HAS_TRACK;
				return;
			}
			throw new KeyNotFoundException("Swid not found in friend collection for " + soundStudioRadioTrackData.playerDisplayName);
		}

		public void AddOrUpdateFriend(Friend friend)
		{
			FriendAndStatusWrapper friendAndStatusWrapper = null;
			if (swidToFriendWrapperMap.ContainsKey(friend.Swid))
			{
				friendAndStatusWrapper = swidToFriendWrapperMap[friend.Swid];
			}
			if (friendAndStatusWrapper == null)
			{
				friendAndStatusWrapper = new FriendAndStatusWrapper();
			}
			friendAndStatusWrapper.friend = friend;
			swidToFriendWrapperMap[friend.Swid] = friendAndStatusWrapper;
		}

		public void Clear()
		{
			swidToFriendWrapperMap.Clear();
		}

		public bool ContainsFriend(string swid)
		{
			return swidToFriendWrapperMap.ContainsKey(swid);
		}

		public bool ContainsFriend(Friend friend)
		{
			return ContainsFriend(friend.Swid);
		}

		public bool ContainsTrack(string swid)
		{
			if (!ContainsFriend(swid))
			{
				throw new KeyNotFoundException("Could not find swid");
			}
			return swidToFriendWrapperMap[swid].soundStudioRadioTrackData != null;
		}

		public List<Friend> GetFriends()
		{
			List<Friend> list = new List<Friend>();
			foreach (FriendAndStatusWrapper value in swidToFriendWrapperMap.Values)
			{
				list.Add(value.friend);
			}
			list.Sort(Friend.SortNameAscending);
			return list;
		}

		public SoundStudioRadioTrackData GetTrack(string swid)
		{
			if (!ContainsTrack(swid))
			{
				throw new KeyNotFoundException("Could not find swid");
			}
			return swidToFriendWrapperMap[swid].soundStudioRadioTrackData;
		}

		public Friend GetFriend(string swid)
		{
			return swidToFriendWrapperMap[swid].friend;
		}

		public FriendTrackStatus GetFriendTrackStatus(string swid)
		{
			if (!ContainsFriend(swid))
			{
				throw new KeyNotFoundException("Could not find swid");
			}
			return swidToFriendWrapperMap[swid].status;
		}

		public void SetNoTrack(string swid)
		{
			if (!ContainsFriend(swid))
			{
				throw new KeyNotFoundException("Swid not in collection!");
			}
			swidToFriendWrapperMap[swid].status = FriendTrackStatus.NO_TRACK;
		}
	}
}
