using System;
using UnityEngine;

namespace SoundStudio.Model
{
	public class ApplicationState
	{
		private SongVO currentSong;

		private GenreVO currentGenre;

		private string currentLevel;

		private int hasSeenDataAndPurchaseWarningPrompt = -1;

		public bool IsMute
		{
			get;
			set;
		}

		public AssetBundle AssetBundle
		{
			get;
			set;
		}

		public PlayerAccountVO currentPlayer
		{
			get;
			set;
		}

		public GenreListVO genreData
		{
			get;
			set;
		}

		public SongListVO songData
		{
			get;
			set;
		}

		public CachedActionListVO cachedActionData
		{
			get;
			set;
		}

		public DateTime LastAppUpdateCheck
		{
			get;
			set;
		}

		public GenreVO CurrentGenre
		{
			get
			{
				return currentGenre;
			}
			set
			{
				currentGenre = value;
			}
		}

		public string CurrentLevel
		{
			get
			{
				return currentLevel;
			}
			set
			{
				currentLevel = value;
			}
		}

		public bool HasSeenDataAndPurchaseWarningPrompt
		{
			get
			{
				if (hasSeenDataAndPurchaseWarningPrompt == -1)
				{
					hasSeenDataAndPurchaseWarningPrompt = PlayerPrefs.GetInt("HasSeenDataAndPurchaseWarningPrompt", -1);
					if (hasSeenDataAndPurchaseWarningPrompt == -1)
					{
						hasSeenDataAndPurchaseWarningPrompt = 1;
						PlayerPrefs.SetInt("HasSeenDataAndPurchaseWarningPrompt", 1);
						return false;
					}
				}
				return true;
			}
		}

		public string CurrentPlayerDataDirectory => $"{Application.persistentDataPath}/{currentPlayer.Username}/";

		public ApplicationState()
		{
			LastAppUpdateCheck = DateTime.MinValue;
			IsMute = false;
			currentLevel = string.Empty;
		}

		public override string ToString()
		{
			return $"[ApplicationState: CurrentGenre={CurrentGenre}]";
		}
	}
}
