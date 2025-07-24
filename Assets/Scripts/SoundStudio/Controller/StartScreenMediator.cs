using Disney.ClubPenguin.ForcedUpdate;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using StrangeHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class StartScreenMediator : EventMediator
	{
		private AutoListener contextDispatcherAutoListener;

		private AutoListener viewDispatcherAutoListener;

		[Inject]
		public StartScreenView view
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
		public AssetBundleDownloadService AssetBundleDownloadService
		{
			get;
			set;
		}

		[Inject]
		public IForcedUpdateManager ForcedUpdateManager
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

		private void StartMainAssetBundleDownload()
		{
			if (application.AssetBundle == null)
			{
				GetAssetBundlePayloadVO getAssetBundlePayloadVO = new GetAssetBundlePayloadVO();
				getAssetBundlePayloadVO.ParentGameObject = view.StartScreenViewGameObject;
				getAssetBundlePayloadVO.BundleURL = "http://cpapps.edgesuite.net/cpapps/AssetBundleAudio.android.unity3d";
				getAssetBundlePayloadVO.BundleURL = "http://cpapps.edgesuite.net/cpapps/AssetBundleAudio.android.unity3d";
				getAssetBundlePayloadVO.Version = 18;
				base.dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE, getAssetBundlePayloadVO);
			}
		}

		public override void OnRegister()
		{
			Input.multiTouchEnabled = false;
			base.dispatcher.Dispatch(SoundStudioEvent.RESUME_DMO_ANALYTICS);
			viewDispatcherAutoListener.AddListener("ABOUT_MEMBERSHIP", OnAboutMembershipClicked);
			viewDispatcherAutoListener.AddListener("PLAY_CLICK_EVENT", OnPlayClicked);
			viewDispatcherAutoListener.AddListener("SONGS_CLICK_EVENT", OnSongsClicked);
			viewDispatcherAutoListener.AddListener("LOGIN_CLICK_EVENT", OnLoginClicked);
			viewDispatcherAutoListener.AddListener("SETTINGS_CLICK_EVENT", OnSettingsClicked);
			viewDispatcherAutoListener.AddListener("RADIO_CLICK_EVENT", OnRadioClicked);
			viewDispatcherAutoListener.AddListener("REFERRAL_CLICK_EVENT", OnReferralClicked);
			viewDispatcherAutoListener.AddListener("STORE_CLICK_EVENT", OnStoreClicked);
			viewDispatcherAutoListener.AddListener("LOGOUT_CLICK_EVENT", OnLogoutClicked);
			viewDispatcherAutoListener.AddListener("CP_BUTTON_CLICK_EVENT", OnCPButtonClicked);
			viewDispatcherAutoListener.AddListener("BACK_CLICK_EVENT", OnBackClicked);
			contextDispatcherAutoListener.AddListener(LoginEvent.LOGIN_FAIL, OnLoginFail);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.LOAD_ACCOUNT_COMPLETE, OnGetAccountDataComplete);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_STARTED, OnGetAssetBundleStarted);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_SUCCESS, OnGetAssetBundleSuccess);
			contextDispatcherAutoListener.AddListener(SoundStudioEvent.SHOW_APP_USES_DATA_CLOSED, OnShowAppUsesDataClosed);
			ShowPenguin();
			if (application.currentPlayer == null)
			{
				base.dispatcher.Dispatch(SoundStudioEvent.AUTO_LOGIN, view);
			}
			StartCoroutine(DelayShowPromptAppUsesData());
			AudioClipPlayer.ClearMemoryAsNeeded();
		}

		private void OnRetryGetAssetBundle(IEvent payload)
		{
			StartMainAssetBundleDownload();
		}

		private void OnGetAssetBundleSuccess(IEvent payload)
		{
			if (view.MenuContainer != null)
			{
				view.MenuContainer.gameObject.SetActive(value: true);
			}
		}

		private void OnGetAssetBundleStarted(IEvent payload)
		{
			if (view.MenuContainer != null)
			{
				view.MenuContainer.gameObject.SetActive(value: false);
			}
		}

		private void OnBackClicked(IEvent evt)
		{
			StartScreenView startScreenView = view;
			if (startScreenView == null)
			{
				startScreenView = (evt.data as StartScreenView);
			}
			if (startScreenView != null)
			{
				base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
				base.dispatcher.Dispatch(ShowConfirmQuitEvent.SHOW_CONFIRM_QUIT, startScreenView.canvasObject.GetComponent<RectTransform>());
			}
		}

		private void OnAboutMembershipClicked(IEvent evt)
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(ShowAboutMembershipEvent.SHOW_ABOUT_MEMBERSHIP, view.canvasObject.GetComponent<RectTransform>());
		}

		private void OnCPButtonClicked(IEvent payload)
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			StartScreenView startScreenView = view;
			if (startScreenView == null)
			{
				startScreenView = (payload.data as StartScreenView);
			}
			if (startScreenView != null)
			{
				Object component = startScreenView.canvasObject.GetComponent<RectTransform>();
				base.dispatcher.Dispatch(ShowCPAppEvent.SHOW_CP_APP, component);
			}
		}

		public override void OnRemove()
		{
			viewDispatcherAutoListener.RemovedListeners();
			contextDispatcherAutoListener.RemovedListeners();
		}

		public void OnDestroy()
		{
			OnRemove();
		}

		private void OnPlayClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Genres);
		}

		private void OnSongsClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Songs);
		}

		private void OnLoginClicked(IEvent evt)
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			StartScreenView startScreenView = view;
			if (startScreenView == null)
			{
				startScreenView = (evt.data as StartScreenView);
			}
			if (startScreenView != null)
			{
				List<object> list = new List<object>(2);
				list.Add(startScreenView.LoginContextPrefab);
				list.Add(startScreenView.LoginCanvas);
				base.dispatcher.Dispatch(ShowLoginEvent.SHOW_LOGIN, list);
			}
		}

		private void OnLogoutClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(SoundStudioEvent.LOG_OUT);
			ShowPenguin();
		}

		private void OnSettingsClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Settings);
		}

		private void OnStoreClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(ShowStoreEvent.SHOW_STORE, view.canvasObject.GetComponent<RectTransform>());
		}

		private void OnRadioClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.Radio);
		}

		private void OnReferralClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			Object component = view.canvasObject.GetComponent<RectTransform>();
			base.dispatcher.Dispatch(ShowReferralStoreEvent.SHOW_REFERRAL_STORE, component);
		}

		private void OnLoginFail()
		{
			ShowPenguin();
		}

		private void OnGetAccountDataComplete()
		{
			ShowPenguin();
		}

		private void ShowPenguin()
		{
			if (application.currentPlayer != null)
			{
				if (application.currentPlayer.AccountStatus == MembershipStatus.GUEST)
				{
					ShowPenguinGuest();
				}
				else
				{
					ShowPenguinColor();
				}
			}
			else
			{
				ShowPenguinGuest();
			}
		}

		private void ShowPenguinColor()
		{
			view.penguinImage_Color.SetActive(value: true);
			view.penguinImage_Guest.SetActive(value: false);
			view.LogoutButton.SetActive(value: true);
			view.SwitchUserButton.SetActive(value: true);
			view.penguinNameText.text = application.currentPlayer.DisplayName;
			int penguinColor = application.currentPlayer.PenguinColor;
			view.penguinImage_Tint.color = Utils.PenguinColorIDToColor(penguinColor);
		}

		private void ShowPenguinGuest()
		{
			view.penguinImage_Color.SetActive(value: false);
			view.penguinImage_Guest.SetActive(value: true);
			view.LogoutButton.SetActive(value: false);
			view.SwitchUserButton.SetActive(value: false);
			view.penguinNameText.text = string.Empty;
		}

		private IEnumerator DelayShowPromptAppUsesData()
		{
			yield return new WaitForSeconds(1f);
			ShowPromptAppUsesData();
		}

		private void ShowPromptAppUsesData()
		{
			base.dispatcher.Dispatch(SoundStudioEvent.SHOW_APP_USES_DATA, view.StartScreenViewGameObject.transform);
		}

		private void OnShowAppUsesDataClosed(IEvent payload)
		{
		}
	}
}
