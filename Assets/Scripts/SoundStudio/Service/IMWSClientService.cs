using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;

namespace SoundStudio.Service
{
	public interface IMWSClientService
	{
		IEventDispatcher dispatcher
		{
			get;
			set;
		}

		IMWSClient MWSClient
		{
			get;
			set;
		}

		void AddSoundStudioTrack(SoundStudioTrackData soundStudioTrackData);

		void DeleteMySoundStudioTrack(long trackId);

		void GetMySoundStudioTracks();

		void GetSoundStudioTrack(long playerId, long trackId);

		void GetMySoundStudioTrack(long playerId, long trackId);

		void GetAccountData();

		void GetSoundStudioMyTracksListing();

		void GetSharedTracksBySwids(List<string> swids, string language);

		void RenameSoundStudioTrack(long trackId, string name);

		void UpdateShareStateOfSoundStudioTrack(long trackId, TrackShareState shareState);

		void LoginRetry(string username, string Password);

		void GetRandomSharedTracks(string language, int count);

		void GetNewSharedTracks(string language, int limit, int beforeTrackId);

		void AddSoundStudioGenreToCollection(PlayerAccountVO player, int genreID);

		void ClaimProductsForMember(IList<string> productIds);

		void GetIAPPurchasesForPlayer();

		ICollection<int> GetSoundStudioMyGenres();
	}
}
