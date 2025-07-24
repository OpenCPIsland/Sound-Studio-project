using Disney.Xmpp;
using System.Collections.Generic;

namespace Disney.ClubPenguin.Service.Friends
{
	public class Friend
	{
		private class SortByNameAscending : IComparer<Friend>
		{
			public int Compare(Friend friendA, Friend friendB)
			{
				return friendA.displayName.CompareTo(friendB.displayName);
			}
		}

		public static IComparer<Friend> SortNameAscending = new SortByNameAscending();

		private string swid;

		private string displayName;

		private bool trusted;

		private string status = "unavailable";

		private Land land;

		public string Swid => swid;

		public string Name => displayName;

		public bool Trusted
		{
			get
			{
				return trusted;
			}
			set
			{
				trusted = value;
			}
		}

		public string Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		public Land Land
		{
			get
			{
				return land;
			}
			set
			{
				land = value;
			}
		}

		public Friend(string aSwid, string aName)
		{
			swid = aSwid;
			displayName = aName;
			Trusted = false;
		}

		public Friend(Friend other)
		{
			swid = other.swid;
			displayName = other.displayName;
			trusted = other.trusted;
			status = other.status;
			land = other.land;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(Friend))
			{
				return false;
			}
			Friend friend = (Friend)obj;
			return swid == friend.swid && displayName == friend.displayName && trusted == friend.trusted && status == friend.status && land == friend.land;
		}

		public override int GetHashCode()
		{
			return ((swid != null) ? swid.GetHashCode() : 0) ^ ((displayName != null) ? displayName.GetHashCode() : 0) ^ trusted.GetHashCode() ^ ((status != null) ? status.GetHashCode() : 0) ^ ((land != null) ? land.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return $"[Friend: swid={swid}, displayName={displayName}, trusted={trusted}, status={status}, land={land}]";
		}
	}
}
