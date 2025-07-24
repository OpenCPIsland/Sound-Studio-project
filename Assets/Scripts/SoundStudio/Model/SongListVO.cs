using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SoundStudio.Model
{
	public class SongListVO
	{
		private Dictionary<long, SongVO> songIDToSongVOMap = new Dictionary<long, SongVO>();

		private List<SongVO> unSyncedSongs = new List<SongVO>();

		public int Count => songIDToSongVOMap.Count + unSyncedSongs.Count;

		public ICollection<SongVO> SongList
		{
			get
			{
				List<SongVO> list = new List<SongVO>();
				list.AddRange(songIDToSongVOMap.Values);
				list.AddRange(unSyncedSongs);
				return new ReadOnlyCollection<SongVO>(list);
			}
		}

		public int SynchronizedCount => songIDToSongVOMap.Count;

		public void AddReplaceSong(SongVO song)
		{
			if (song != null)
			{
				if (song.HasServerID)
				{
					songIDToSongVOMap[song.serverID] = song;
					RemoveUnsynchronizedSongVO(song);
				}
				else
				{
					AddUnsyncedSong(song);
				}
				return;
			}
			throw new InvalidOperationException("SongVO cannot be null");
		}

		public void AddUnsyncedSong(SongVO song)
		{
			if (!song.HasServerID)
			{
				unSyncedSongs.Add(song);
			}
		}

		public void AddAll(ICollection<SongVO> newSongs)
		{
			foreach (SongVO newSong in newSongs)
			{
				AddReplaceSong(newSong);
			}
		}

		public void Clear()
		{
			songIDToSongVOMap.Clear();
			unSyncedSongs.Clear();
		}

		public bool ContainsSong(SongVO song)
		{
			return songIDToSongVOMap.ContainsKey(song.serverID);
		}

		public bool ContainsSong(long id)
		{
			return songIDToSongVOMap.ContainsKey(id);
		}

		public SongVO GetSong(long id)
		{
			return songIDToSongVOMap[id];
		}

		public void RemoveUnsynchronizedSongVO(SongVO song)
		{
			unSyncedSongs.Remove(song);
		}

		public void Remove(SongVO song)
		{
			if (songIDToSongVOMap.ContainsKey(song.serverID))
			{
				songIDToSongVOMap.Remove(song.serverID);
			}
			else
			{
				RemoveUnsynchronizedSongVO(song);
			}
		}
	}
}
