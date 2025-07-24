using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Model;
using System.Collections.Generic;

namespace SoundStudio
{
	public class TracksConsolidator
	{
		public List<SongVO> CalculateLocalSongsToDelete(List<SoundStudioTrackData> serverSongs, ICollection<SongVO> localSongs)
		{
			HashSet<long> hashSet = CreateServerTrackIDsHashSet(serverSongs);
			List<SongVO> list = new List<SongVO>();
			foreach (SongVO localSong in localSongs)
			{
				if (localSong.HasServerID && !hashSet.Contains(localSong.serverID))
				{
					list.Add(localSong);
				}
			}
			return list;
		}

		private static HashSet<long> CreateServerTrackIDsHashSet(List<SoundStudioTrackData> serverSongs)
		{
			HashSet<long> hashSet = new HashSet<long>();
			foreach (SoundStudioTrackData serverSong in serverSongs)
			{
				hashSet.Add(serverSong.TrackId);
			}
			return hashSet;
		}

		public List<long> CalculateServerTracksToAddLocally(List<SoundStudioTrackData> serverSongs, ICollection<SongVO> localSongs)
		{
			HashSet<long> hashSet = new HashSet<long>();
			foreach (SongVO localSong in localSongs)
			{
				if (localSong.HasServerID)
				{
					hashSet.Add(localSong.serverID);
				}
			}
			List<long> list = new List<long>();
			foreach (SoundStudioTrackData serverSong in serverSongs)
			{
				if (!hashSet.Contains(serverSong.TrackId))
				{
					list.Add(serverSong.TrackId);
				}
			}
			return list;
		}
	}
}
