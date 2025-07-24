using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using StrangeHelper;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class MixingBoardMediator : EventMediator
	{
		public static int NUM_BUTTON_ROWS = 5;

		private AutoListener contextDispatcherAutoListener;

		private AutoListener viewDispatcherAutoListener;

		[Inject]
		public SongPlayer songPlayer
		{
			get;
			set;
		}

		[Inject]
		public MixingBoardView view
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

		[Inject]
		public GenreAudioManager genreAudioManager
		{
			get;
			set;
		}

		[Inject]
		public SongRecorder songRecorder
		{
			get;
			set;
		}

		[Inject]
		public IMWSClientService MWSClient
		{
			get;
			set;
		}

		[PostConstruct]
		public void Initialize()
		{
			contextDispatcherAutoListener = new AutoListener(base.dispatcher);
			viewDispatcherAutoListener = new AutoListener(view.dispatcher);
		}

		public override void OnRegister()
		{
			Input.multiTouchEnabled = true;
			LoadGenreAudioAsync();
			if (application.currentPlayer.AccountStatus == MembershipStatus.GUEST)
			{
				base.dispatcher.Dispatch(LoadPlayerPurchasesEvent.LOAD_PLAYER_PURCHASES);
			}
			base.dispatcher.Dispatch(SoundStudioEvent.PAUSE_DMO_ANALYTICS);
			viewDispatcherAutoListener.AddListener("SOUND_CLICK_EVENT", View_SoundClick_Handler);
			viewDispatcherAutoListener.AddListener("BACK_CLICK_EVENT", onBackClicked);
			viewDispatcherAutoListener.AddListener("RECORD_CLICK_EVENT", View_RecordClick_Handler);
			viewDispatcherAutoListener.AddListener("STOP_CLICK_EVENT", View_StopClick_Handler);
			SongRecorder.OnRecordStop += SongRecorder_Stop_Handler;
			SetRecordButtonState();
			CheckPlayerOwnership();
			contextDispatcherAutoListener.AddListener(LoginEvent.LOGIN_FAIL, SetRecordButtonState);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.LOAD_ACCOUNT_COMPLETE, SetRecordButtonState);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.PURCHASES_REFRESHED, OnPurchasesRefreshed);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.RECORD_COUNTDOWN_COMPLETE, OnCountDownComplete);
			base.dispatcher.Dispatch(SoundStudioEvent.SHOW_MOPUB_AD);
		}

		private void OnPurchasesRefreshed(IEvent payload)
		{
			SetRecordButtonState();
		}

		private void CheckPlayerOwnership()
		{
			if (application.currentPlayer.AccountStatus == MembershipStatus.MEMBER && !application.currentPlayer.HasGenre(application.CurrentGenre.id))
			{
				List<string> list = new List<string>();
				list.Add(application.CurrentGenre.productIdentifier);
				MWSClient.ClaimProductsForMember(list);
				application.currentPlayer.AddGenre(application.CurrentGenre.id);
				SetRecordButtonState();
				base.dispatcher.Dispatch(SoundStudioEvent.SHOW_MEMBERSHIP_PROMPT, view.CanvasObject);
			}
		}

		private void LoadGenreAudioAsync()
		{
			base.dispatcher.AddListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnLoadGenreAudioComplete);
			view.LoadingPanel.SetActive(value: true);
			StartCoroutine(genreAudioManager.LoadGenreAudioAsync(application.CurrentGenre));
		}

		private void OnLoadGenreAudioComplete(IEvent payload)
		{
			base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnLoadGenreAudioComplete);
			view.SetGenre(application.CurrentGenre);
			RestartSounds();
			StartGenreLoops(application.CurrentGenre);
			view.LoadingPanel.SetActive(value: false);
		}

		private void SetRecordButtonState()
		{
			if (!application.currentPlayer.HasGenre(application.CurrentGenre.id))
			{
				view.LockedRecordButton.gameObject.SetActive(value: true);
				view.RecordButton.gameObject.SetActive(value: false);
			}
			else
			{
				view.LockedRecordButton.gameObject.SetActive(value: false);
				view.RecordButton.gameObject.SetActive(value: true);
			}
		}

		public override void OnRemove()
		{
			Input.multiTouchEnabled = false;
			viewDispatcherAutoListener.RemovedListeners();
			contextDispatcherAutoListener.RemovedListeners();
			base.dispatcher.RemoveListener(SoundStudioEvent.SAVE_SONG_DIALOG_CANCEL, OnSongSaveCancelled);
			SongRecorder.OnRecordStop -= SongRecorder_Stop_Handler;
		}

		public void OnDestroy()
		{
			OnRemove();
		}

		private void onBackClicked()
		{
			AudioClipPlayer.StopAllAudio();
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Genres);
		}

		private void StartGenreLoops(GenreVO genre)
		{
			List<MixingBoardButtonView> list = new List<MixingBoardButtonView>();
			for (int i = 0; i < NUM_BUTTON_ROWS; i++)
			{
				list.AddRange(view.ButtonViewColumns[i]);
			}
			foreach (MixingBoardButtonView item in list)
			{
				item.Sound.Stop();
				item.Sound.Mute();
			}
			songPlayer.TrackOneShotsForMemoryUnload(genre.id);
			songPlayer.StartGenreLoops(genre.id);
		}

		private void View_SoundClick_Handler(IEvent evt)
		{
			application.CurrentGenre.ResetOneShots();
			SoundVO soundVO = (SoundVO)evt.data;
			if (soundVO.IsLoop())
			{
				HandleSoundClick_Loop(soundVO);
				ShowVisuals();
			}
			else
			{
				soundVO.Play();
			}
			if (songRecorder.IsRecording)
			{
				songRecorder.AddData(application.CurrentGenre.GetGridStatus());
			}
		}

		private void HandleSoundClick_Loop(SoundVO sound)
		{
			if (sound.IsPlaying())
			{
				sound.Mute();
				return;
			}
			sound.Play();
			MuteSoundsForColumn(sound);
		}

		private void MuteSoundsForColumn(SoundVO sound)
		{
			foreach (MixingBoardButtonView item in view.ButtonViewColumns[sound.id / NUM_BUTTON_ROWS])
			{
				if (item.Sound.id != sound.id)
				{
					item.Sound.Mute();
				}
			}
		}

		private void View_RecordClick_Handler()
		{
			if (!application.currentPlayer.HasGenre(application.CurrentGenre.id))
			{
				base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
				base.dispatcher.Dispatch(SoundStudioEvent.SHOW_RECORD_OOPS_POPUP, view.CanvasObject);
			}
			else if (application.songData.SongList.Count >= 24)
			{
				base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
				base.dispatcher.Dispatch(SoundStudioEvent.SHOW_MAX_SONGS_POPUP, view.CanvasObject);
			}
			else
			{
				AudioClipPlayer.StopAllAudio();
				GameObject gameObject = Object.Instantiate(view.CountDownPrefab, new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
				gameObject.transform.SetParent(view.CanvasObject.transform, worldPositionStays: false);
			}
		}

		private void OnCountDownComplete()
		{
			view.RecordButton.gameObject.SetActive(value: false);
			view.StopButton.gameObject.SetActive(value: true);
			RestartSounds();
			songRecorder.Record();
			songRecorder.AddData(application.CurrentGenre.GetGridStatus());
		}

		private void RestartSounds()
		{
			application.CurrentGenre.ResetOneShots();
			foreach (SoundVO sound in application.CurrentGenre.SoundList)
			{
				if (sound.IsLoop())
				{
					if (sound.IsPlaying())
					{
						sound.Stop();
						sound.Play();
					}
					else
					{
						sound.Stop();
						sound.Mute();
					}
				}
			}
		}

		private void View_StopClick_Handler()
		{
			songRecorder.Stop();
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		}

		private void SongRecorder_Stop_Handler()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
			StopRecordingAndReset();
			SongVO songVO = songRecorder.createSong("temp", application.CurrentGenre.id);
			if (songVO.Length.TotalSeconds > 5.0)
			{
				ShowSaveSongDialogue();
			}
			StartGenreLoops(application.CurrentGenre);
		}

		private void ShowSaveSongDialogue()
		{
			SongVO data = songRecorder.createSong("temp", application.CurrentGenre.id);
			base.dispatcher.Dispatch(SongEditEvent.SONG_EDIT, data);
			base.dispatcher.AddListener(SoundStudioEvent.SAVE_SONG_DIALOG_CANCEL, OnSongSaveCancelled);
		}

		private void OnSongSaveCancelled(IEvent payload)
		{
			base.dispatcher.RemoveListener(SoundStudioEvent.SAVE_SONG_DIALOG_CANCEL, OnSongSaveCancelled);
			RestartSounds();
		}

		private void StopRecordingAndReset()
		{
			view.StopButton.gameObject.SetActive(value: false);
			view.RecordButton.gameObject.SetActive(value: true);
			AudioClipPlayer.StopAllAudio();
			foreach (SoundVO sound in application.CurrentGenre.SoundList)
			{
				sound.Stop();
			}
			ShowVisuals();
		}

		private void ShowVisuals()
		{
			switch (GetNumLoopsActive())
			{
			case 0:
				view.MovingLights.SetActive(value: false);
				view.MovingLights2.SetActive(value: false);
				view.Sparkles.SetActive(value: false);
				view.Twinkles.SetActive(value: false);
				view.SmallLights.SetActive(value: false);
				break;
			case 1:
				view.MovingLights.SetActive(value: true);
				view.MovingLights2.SetActive(value: false);
				view.Sparkles.SetActive(value: false);
				view.Twinkles.SetActive(value: false);
				view.SmallLights.SetActive(value: false);
				break;
			case 2:
				view.MovingLights.SetActive(value: true);
				view.MovingLights2.SetActive(value: true);
				view.Sparkles.SetActive(value: false);
				view.Twinkles.SetActive(value: false);
				view.SmallLights.SetActive(value: false);
				break;
			case 3:
				view.MovingLights.SetActive(value: true);
				view.MovingLights2.SetActive(value: true);
				view.Sparkles.SetActive(value: true);
				view.Twinkles.SetActive(value: false);
				view.SmallLights.SetActive(value: false);
				break;
			case 4:
				view.MovingLights.SetActive(value: true);
				view.MovingLights2.SetActive(value: true);
				view.Sparkles.SetActive(value: true);
				view.Twinkles.SetActive(value: true);
				view.SmallLights.SetActive(value: false);
				break;
			case 5:
				view.MovingLights.SetActive(value: true);
				view.MovingLights2.SetActive(value: true);
				view.Sparkles.SetActive(value: true);
				view.Twinkles.SetActive(value: true);
				view.SmallLights.SetActive(value: true);
				break;
			}
		}

		private int GetNumLoopsActive()
		{
			string gridStatus = application.CurrentGenre.GetGridStatus();
			return gridStatus.Split('1').Length - 1;
		}
	}
}
