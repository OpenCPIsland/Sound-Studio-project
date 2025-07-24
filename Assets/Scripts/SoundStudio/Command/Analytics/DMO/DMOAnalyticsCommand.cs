using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections.Generic;

namespace SoundStudio.Command.Analytics.DMO
{
	internal class DMOAnalyticsCommand : EventCommand
	{
		private static readonly string CONTEXT = "context";

		private static readonly string ACTION = "action";

		private static readonly string TYPE = "type";

		private static readonly string MESSAGE = "message";

		private DateTime radioStartTime = DateTime.MinValue;

		[Inject]
		public ApplicationState applicationState
		{
			get;
			set;
		}

		[Inject]
		public IAnalyticsService DMOAnalyticsService
		{
			get;
			set;
		}

		[PostConstruct]
		public void Initialize()
		{
			base.dispatcher.AddListener(LoadLevelEvent.LOAD_LEVEL, OnLoadLevel);
			base.dispatcher.AddListener(SoundStudioEvent.APPLICATION_PAUSED_STATE_CHANGE, OnApplicationPause);
			base.dispatcher.AddListener(SoundStudioEvent.APPLICATION_QUIT, OnApplicationQuit);
			base.dispatcher.AddListener(SoundStudioEvent.RESUME_DMO_ANALYTICS, OnResumeAnalytics);
			base.dispatcher.AddListener(SoundStudioEvent.PAUSE_DMO_ANALYTICS, OnPauseAnalytics);
			base.dispatcher.AddListener("PLAY_CLICK_EVENT", OnPlayClicked);
			base.dispatcher.AddListener("SONGS_CLICK_EVENT", OnSongsClicked);
			base.dispatcher.AddListener("LOGIN_CLICK_EVENT", OnLoginClicked);
			base.dispatcher.AddListener("SETTINGS_CLICK_EVENT", OnSettingsClicked);
			base.dispatcher.AddListener("RADIO_CLICK_EVENT", OnRadioClicked);
			base.dispatcher.AddListener("REFERRAL_CLICK_EVENT", OnReferralClicked);
			base.dispatcher.AddListener("STORE_CLICK_EVENT", OnStoreClicked);
			base.dispatcher.AddListener(SelectGenreEvent.SELECT_GENRE, OnLoadGenere);
			base.dispatcher.AddListener(SongDeleteEvent.SONG_DELETE, OnMySongListItemDelete);
			base.dispatcher.AddListener(SongShareEvent.SONG_SHARE, OnMySongShare);
			base.dispatcher.AddListener(SongUnshareEvent.SONG_UNSHARE, OnMySongUnshare);
			base.dispatcher.AddListener(SoundStudioEvent.SONG_SYNC_COMPLETED, OnSongSaveSuccess);
			base.dispatcher.AddListener(SongDeleteEvent.SONG_DELETE, OnDeleteSong);
			base.dispatcher.AddListener("SOUND_CLICK_EVENT", OnMixingBoardViewSoundButtonClicked);
			base.dispatcher.AddListener("FRIENDS_CLICK_EVENT", OnFriendsClicked);
			base.dispatcher.AddListener("NEW_CLICK_EVENT", OnNewClicked);
			base.dispatcher.AddListener("RANDOM_CLICK_EVENT", OnRandomClicked);
			base.dispatcher.AddListener(SoundStudioEvent.RADIO_SONG_PLAY, OnRadioButtonPlay);
			base.dispatcher.AddListener(SoundStudioEvent.ON_AGE_GATE_SUCCESS, OnAgeGateSuccess);
			base.dispatcher.AddListener(SoundStudioEvent.ON_AGE_GATE_FAILED, OnAgeGateFailed);
		}

		private void OnLoadLevel(IEvent payload)
		{
			if (!applicationState.CurrentLevel.Equals(SceneNames.Radio.ToString()))
			{
				LogRadioElapsedTime();
			}
		}

		private void OnPauseAnalytics(IEvent payload)
		{
			DMOAnalyticsService.PauseAnalytics();
		}

		public override void Execute()
		{
			Retain();
			DMOAnalyticsService.StartAnaylitics();
		}

		private void OnResumeAnalytics(IEvent payload)
		{
			DMOAnalyticsService.ResumeAnalytics();
			DMOAnalyticsService.Flush();
		}

		private void LogRadioFilterEvent(string actionValue)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[CONTEXT] = "cp_radio";
			dictionary[ACTION] = actionValue;
			DMOAnalyticsService.LogGameAction(dictionary);
		}

		private void OnRandomClicked(IEvent payload)
		{
			LogRadioFilterEvent("everyone");
		}

		private void OnNewClicked(IEvent payload)
		{
			LogRadioFilterEvent("new");
		}

		private void OnFriendsClicked(IEvent payload)
		{
			LogRadioFilterEvent("friends");
		}

		private void OnRadioButtonPlay(IEvent payload)
		{
			SongVO songVO = payload.data as SongVO;
			if (songVO == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (songVO != null)
			{
				GenreVO genreByID = applicationState.genreData.getGenreByID(songVO.GenreID);
				if (genreByID != null)
				{
					dictionary[CONTEXT] = "cp_radio";
					dictionary[ACTION] = "song_played";
					dictionary[MESSAGE] = genreByID.name.ToLower();
					DMOAnalyticsService.LogGameAction(dictionary);
				}
			}
		}

		private void LogRadioElapsedTime()
		{
			if (radioStartTime != DateTime.MinValue)
			{
				if (applicationState == null || applicationState.currentPlayer == null)
				{
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["player_id"] = applicationState.currentPlayer.ID;
				dictionary["location"] = "cp_radio";
				int num = (int)(DateTime.Now - radioStartTime).TotalSeconds;
				if (num >= 1)
				{
					dictionary["elapsed_time"] = num;
					DMOAnalyticsService.LogAnalyticsEventWithContext("timing", dictionary);
				}
			}
			radioStartTime = DateTime.MinValue;
		}

		private void OnMySongUnshare(IEvent payload)
		{
			SongVO songVO = payload.data as SongVO;
			if (songVO != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				GenreVO genreByID = applicationState.genreData.getGenreByID(songVO.GenreID);
				if (genreByID != null)
				{
					dictionary[CONTEXT] = "track_unshared";
					dictionary[ACTION] = genreByID.name.ToLower();
				}
				DMOAnalyticsService.LogGameAction(dictionary);
			}
		}

		private void OnAgeGateFailed(IEvent payload)
		{
			if (payload.data != null)
			{
				OnAgeGateComplete((int)payload.data);
			}
		}

		private void OnAgeGateSuccess(IEvent payload)
		{
			if (payload.data != null)
			{
				OnAgeGateComplete((int)payload.data);
			}
		}

		private void OnAgeGateComplete(int age)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[CONTEXT] = "age_gate";
			dictionary[ACTION] = "age_gate";
			dictionary[TYPE] = age.ToString();
			DMOAnalyticsService.LogGameAction(dictionary);
		}

		private void OnMySongShare(IEvent payload)
		{
			SongVO songVO = payload.data as SongVO;
			if (songVO != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				GenreVO genreByID = applicationState.genreData.getGenreByID(songVO.GenreID);
				if (genreByID != null)
				{
					dictionary[CONTEXT] = "track_shared";
					dictionary[ACTION] = genreByID.name.ToLower();
				}
				DMOAnalyticsService.LogGameAction(dictionary);
			}
		}

		private void OnMySongListItemDelete(IEvent payload)
		{
		}

		private void LogMainScreenEvent(string actionValue)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[CONTEXT] = "main_screen";
			dictionary[ACTION] = actionValue;
			DMOAnalyticsService.LogGameAction(dictionary);
		}

		private void OnStoreClicked(IEvent payload)
		{
			LogMainScreenEvent("store");
		}

		private void OnReferralClicked(IEvent payload)
		{
		}

		private void OnRadioClicked(IEvent payload)
		{
			LogMainScreenEvent("radio");
			radioStartTime = DateTime.Now;
		}

		private void OnSettingsClicked(IEvent payload)
		{
		}

		private void OnLoginClicked(IEvent payload)
		{
		}

		private void OnSongsClicked(IEvent payload)
		{
			LogMainScreenEvent("my_music");
		}

		private void OnPlayClicked(IEvent payload)
		{
			LogMainScreenEvent("play");
		}

		public void OnApplicationPause(IEvent eventParams)
		{
			if (eventParams == null || eventParams.data == null)
			{
				return;
			}
			PauseState pauseState = (PauseState)(int)eventParams.data;
			switch (pauseState)
			{
			case PauseState.Paused:
				LogRadioElapsedTime();
				break;
			case PauseState.UnPaused:
				if (applicationState.CurrentLevel.Equals(SceneNames.Radio.ToString()))
				{
					radioStartTime = DateTime.Now;
				}
				break;
			}
			DMOAnalyticsService.ApplicationPause(pauseState);
		}

		private void OnApplicationQuit(IEvent payload)
		{
			LogRadioElapsedTime();
			DMOAnalyticsService.LogApplicationQuit();
			DMOAnalyticsService.Flush();
		}

		public void OnLoadGenere(IEvent eventParams)
		{
			if (eventParams != null && eventParams.data != null)
			{
				GenreVO genreVO = (GenreVO)eventParams.data;
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary[CONTEXT] = "genre_selected";
				dictionary[ACTION] = genreVO.name.ToLower();
				DMOAnalyticsService.LogGameAction(dictionary);
			}
		}

		private void OnMixingBoardViewSoundButtonClicked(IEvent payload)
		{
			SoundVO soundVO = payload.data as SoundVO;
			if (soundVO != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (soundVO != null)
				{
					dictionary[CONTEXT] = "mixing_board";
					dictionary[ACTION] = applicationState.CurrentGenre.name.ToLower();
					dictionary[MESSAGE] = soundVO.id.ToString();
					DMOAnalyticsService.LogGameAction(dictionary);
				}
			}
		}

		private void OnDeleteSong(IEvent payload)
		{
			try
			{
				SongVO songVO = payload.data as SongVO;
				if (songVO != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					GenreVO genreByID = applicationState.genreData.getGenreByID(songVO.GenreID);
					if (genreByID != null)
					{
						dictionary[CONTEXT] = "song_deleted";
						dictionary[ACTION] = genreByID.name.ToLower();
						dictionary[MESSAGE] = applicationState.songData.Count;
						DMOAnalyticsService.LogGameAction(dictionary);
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}

		private void OnSongSaveSuccess(IEvent payload)
		{
			try
			{
				SongSyncVO songSyncVO = payload.data as SongSyncVO;
				if (songSyncVO != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					GenreVO genreByID = applicationState.genreData.getGenreByID(songSyncVO.SongVo.GenreID);
					if (genreByID != null)
					{
						dictionary[CONTEXT] = "song_saved";
						dictionary[ACTION] = genreByID.name.ToLower();
						dictionary[MESSAGE] = applicationState.songData.Count;
						dictionary["length_in_seconds"] = (int)songSyncVO.SongVo.Length.TotalSeconds;
						DMOAnalyticsService.LogGameAction(dictionary);
					}
				}
			}
			catch (NullReferenceException)
			{
			}
		}
	}
}
