using System.Collections.Generic;
using System.Text;

namespace SoundStudio.Model
{
	public class PlayerAccountVO
	{
		private FriendsTrackCollection friendsTrackCollection;
		private HashSet<int> genresCollected;

		public string DisplayName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Swid { get; set; }
		public long ID { get; set; }

		private MembershipStatus _accountStatus; // backing field

		public MembershipStatus AccountStatus
		{
			get => MembershipStatus.MEMBER; // Always return MEMBER (force membership)
			set => _accountStatus = value;  // store set value normally
		}

		public string AuthToken { get; set; }
		public int PenguinColor { get; set; }
		public MySongsStatus MySongsState { get; set; }
		public FriendCollectionStatus FriendCollectionStatus { get; set; }
		public FriendsTrackCollection FriendsTrackCollection => friendsTrackCollection;

		public PlayerAccountVO()
		{
			FriendCollectionStatus = FriendCollectionStatus.UNINITIALIZED;
			friendsTrackCollection = new FriendsTrackCollection();
			genresCollected = new HashSet<int>();

			// This line still here but irrelevant now:
			AccountStatus = MembershipStatus.GUEST;
			MySongsState = MySongsStatus.LOADING;
		}

		public bool HasGenre(int genreID)
		{
			return genresCollected.Contains(genreID);
		}

		public void AddGenreByStoreID(string storeID)
		{
			switch (storeID)
			{
				case "Disney.ClubPenguinSoundStudio.GenrePop":
					AddGenre(0);
					break;
				case "Disney.ClubPenguinSoundStudio.GenreRock":
					AddGenre(1);
					break;
				case "Disney.ClubPenguinSoundStudio.GenreDance":
					AddGenre(2);
					break;
				case "Disney.ClubPenguinSoundStudio.GenreCadence":
					AddGenre(5);
					break;
				case "Disney.ClubPenguinSoundStudio.GenreDubstep":
					AddGenre(3);
					break;
				case "Disney.ClubPenguinSoundStudio.GenreSpooky":
					AddGenre(4);
					break;
			}
		}

		public void AddGenre(int genreID)
		{
			if (!genresCollected.Contains(genreID))
			{
				genresCollected.Add(genreID);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int item in genresCollected)
			{
				stringBuilder.Append(item).Append(",");
			}
			string text = (stringBuilder.Length <= 0) ? string.Empty : stringBuilder.ToString(0, stringBuilder.Length - 1);
			return $"DisplayName: {DisplayName}, Swid: {Swid}, ID: {ID}, AccountStatus: {AccountStatus}, Genres Collected{text}";
		}
	}
}
