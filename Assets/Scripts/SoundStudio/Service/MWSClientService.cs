using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.MWS.Domain;
using Disney.HTTP.Client;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;

namespace SoundStudio.Service
{
	internal class MWSClientService : IMWSClientService
	{
		public const string AppID = "CPMCAPP";

		public const string AppVersion = "1.6";

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher
		{
			get;
			set;
		}

		[Inject]
		public IMWSClient MWSClient
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[PostConstruct]
		public void Initialize()
		{
		}

		public void GetMySoundStudioTracks()
		{
			MWSClient.GetSoundStudioTracks(GetMySoundStudioTracksResponseCallback);
		}

		private void GetMySoundStudioTracksResponseCallback(IGetSoundStudioTracksDataResponse response)
		{
			dispatcher.Dispatch(MWSEvent.GET_MY_TRACKS_COMPLETED, response);
		}

		public void GetSoundStudioTrack(long playerId, long trackId)
		{
			MWSClient.GetSoundStudioTrack(playerId, trackId, GetSoundStudioTrackDataResponseCallback);
		}

		private void GetSoundStudioTrackDataResponseCallback(IGetSoundStudioTrackDataResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_TRACK_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_TRACK_COMPLETED, response.SoundStudioTrackData);
			}
		}

		public void GetMySoundStudioTrack(long playerId, long trackId)
		{
			MWSClient.GetSoundStudioTrack(playerId, trackId, GetMySoundStudioTrackDataResponseCallback);
		}

		private void GetMySoundStudioTrackDataResponseCallback(IGetSoundStudioTrackDataResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_MY_TRACK_FAILED);
				return;
			}
			if (response.SoundStudioTrackData.PlayerId == application.currentPlayer.ID)
			{
				SongVO songVO = Utils.ConvertSoundStudioTrackDataToSongVO(response.SoundStudioTrackData);
				if (application.songData.ContainsSong(songVO))
				{
					SongVO song = application.songData.GetSong(songVO.serverID);
					if (string.IsNullOrEmpty(song.rawData))
					{
						application.songData.AddReplaceSong(songVO);
						dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songVO);
					}
				}
				else
				{
					application.songData.AddReplaceSong(songVO);
					dispatcher.Dispatch(SongSaveEvent.SONG_SAVE, songVO);
				}
			}
			dispatcher.Dispatch(MWSEvent.GET_MY_TRACK_SUCCESS);
		}

		public void GetAccountData()
		{
			MWSClient.GetAccount(GetAccountDataResponseCallback);
		}

		private void GetAccountDataResponseCallback(IGetAccountResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_ACCOUNT_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_ACCOUNT_COMPLETED, response.Account);
			}
		}

		public void GetSoundStudioMyTracksListing()
		{
			MWSClient.GetSoundStudioTracksListing(GetSoundStudioTracksListingResponseCallback);
		}

		private void GetSoundStudioTracksListingResponseCallback(IGetSoundStudioTracksDataResponse response)
		{
			dispatcher.Dispatch(MWSEvent.GET_MY_TRACKS_LISTING_COMPLETED, response);
		}

		public void RenameSoundStudioTrack(long trackId, string name)
		{
			MWSClient.RenameSoundStudioTrack(trackId, name, RenameSoundStudioTrackResponseCallback);
		}

		private void RenameSoundStudioTrackResponseCallback(IHTTPResponse response)
		{
			dispatcher.Dispatch(MWSEvent.RENAME_MY_TRACK_COMPLETED, response);
		}

		public void UpdateShareStateOfSoundStudioTrack(long trackId, TrackShareState shareState)
		{
			MWSClient.UpdateShareStateOfSoundStudioTrack(trackId, shareState, UpdateShareStateOfSoundStudioTrackResponseCallback);
		}

		private void UpdateShareStateOfSoundStudioTrackResponseCallback(IHTTPResponse response)
		{
			if (response.IsError)
			{
				if (response.StatusCode == 204)
				{
					dispatcher.Dispatch(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, response);
				}
				else
				{
					dispatcher.Dispatch(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_FAILED, response);
				}
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.UPDATE_SHARE_STATE_MY_TRACK_SUCCESS, response);
			}
		}

		public void AddSoundStudioTrack(SoundStudioTrackData soundStudioTrackData)
		{
			MWSClient.AddSoundStudioTrack(soundStudioTrackData, AddSoundStudioTrackResponseCallback);
		}

		private void AddSoundStudioTrackResponseCallback(IGetSoundStudioTrackDataResponse response)
		{
			dispatcher.Dispatch(MWSEvent.ADD_NEW_TRACK_COMPLETED, response);
		}

		public void DeleteMySoundStudioTrack(long trackId)
		{
			MWSClient.DeleteSoundStudioTrack(trackId, DeleteMySoundStudioTrackResponseCallback);
		}

		private void DeleteMySoundStudioTrackResponseCallback(IHTTPResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.DELETE_MY_TRACK_FAILED, response);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.DELETE_MY_TRACK_SUCCESS, response);
			}
		}

		public void LoginRetry(string username, string password)
		{
			MWSClient.GetAuthToken("CPMCAPP", "1.6", username, password, LoginRetryResponseCallback);
		}

		private void LoginRetryResponseCallback(IGetAuthTokenResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(LoginEvent.LOGIN_FAIL);
			}
			else
			{
				dispatcher.Dispatch(LoginEvent.LOGIN_SUCCESS);
			}
		}

		public void GetNewSharedTracks(string language, int limit, int beforeTrackId)
		{
			MWSClient.GetNewSharedSoundStudioTracksListing(language, limit, beforeTrackId, GetNewSharedTracksCallback);
		}

		private void GetNewSharedTracksCallback(IGetSoundStudioRadioTracksDataResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST_NEW_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST_NEW_SUCCESS, response.Tracks);
			}
		}

		public void GetRandomSharedTracks(string language, int count)
		{
			MWSClient.GetRandomSharedSoundStudioTracksListing(language, count, GetRandomSharedTracksCallback);
		}

		private void GetRandomSharedTracksCallback(IGetSoundStudioRadioTracksDataResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST_RANDOM_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_RADIO_LIST_RANDOM_SUCCESS, response.Tracks);
			}
		}

		public void GetSharedTracksBySwids(List<string> swids, string language)
		{
			MWSClient.GetSharedSoundStudioTracksListing(swids, language, GetSharedSoundStudioTracksListingCallback);
		}

		private void GetSharedSoundStudioTracksListingCallback(IGetSoundStudioRadioTracksDataResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_SHARED_TRACKS_LISTING_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_SHARED_TRACKS_LISTING_SUCCESS, response.Tracks);
			}
		}

		public void AddSoundStudioGenreToCollection(PlayerAccountVO player, int genreID)
		{
		}

		public void ClaimProductsForMember(IList<string> productIds)
		{
			StoreType store = StoreType.GOOGLE_PLAY;
			MWSClient.ClaimProductsForMember(Utils.GetGuid(), store, productIds, ClaimProductsForMemberCallback);
		}

		private void ClaimProductsForMemberCallback(IClaimProductsForMemberResponse response)
		{
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.CLAIM_PRODUCTS_FOR_MEMBER_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.CLAIM_PRODUCTS_FOR_MEMBER_SUCCESS);
			}
		}

		public void GetIAPPurchasesForPlayer()
		{
			MWSClient.GetIAPPurchasesForPlayer(null, OnGetIAPPurchasesForPlayerCallback);
		}

		private void OnGetIAPPurchasesForPlayerCallback(IGetIAPPurchasesResponse response)
		{
			List<ProductPurchase> list = new List<ProductPurchase>(response.Products);
			using (IEnumerator<GenreVO> enumerator = application.genreData.Collection.GetEnumerator())
			{
				GenreVO genre;
				while (enumerator.MoveNext())
				{
					genre = enumerator.Current;
					if (list.Exists((ProductPurchase x) => x.ProductId == genre.productIdentifier))
					{
						application.currentPlayer.AddGenre(genre.id);
					}
				}
			}
			if (response.IsError)
			{
				dispatcher.Dispatch(MWSEvent.GET_PLAYER_PRODUCTS_FAILED);
			}
			else
			{
				dispatcher.Dispatch(MWSEvent.GET_PLAYER_PRODUCTS_SUCCESS);
			}
		}

		public ICollection<int> GetSoundStudioMyGenres()
		{
			return new List<int>();
		}
	}
}
