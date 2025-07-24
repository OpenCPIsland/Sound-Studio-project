using DevonLocalization.Core;
using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;

internal class SettingsMediator : EventMediator
{
	private int logoClickCount;

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	[Inject]
	public SettingsView view
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		view.dispatcher.AddListener("LOGOCLICK", OnLogoClicked);
		view.dispatcher.AddListener("BACK", OnBackButtonClick);
		view.dispatcher.AddListener("MUTE", OnMuteValueChanged);
		view.dispatcher.AddListener("RESTOREPURCHASES", OnRestorePurchasesClicked);
		view.dispatcher.AddListener("MANAGEACCOUNT", OnManageAccountClicked);
		view.dispatcher.AddListener("SHOWPROTECTEDWEBPAGE", OnShowProtectedWebPageClicked);
		view.SetMuteToggleControlValue(AudioListener.pause);
		if (view.manageAccountButton != null)
		{
			view.manageAccountButton.gameObject.SetActive(application.currentPlayer.AccountStatus != MembershipStatus.GUEST);
		}
		ShowVersionLocalized();
	}

	private void ShowVersionLocalized()
	{
		view.VersionText.text = Localizer.Instance.GetTokenTranslation("soundstudio.settings.version") + " 1.2";
	}

	private void OnLogoClicked()
	{
	}

	private void OnMuteValueChanged(IEvent payload)
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_TOGGLE);
		bool flag = (bool)payload.data;
		bool flag2 = flag;
		application.IsMute = flag2;
		AudioListener.pause = flag2;
	}

	private void OnClickTerms(IEvent payload)
	{
	}

	private void OnBackButtonClick(IEvent payload)
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.StartScreen);
	}

	private void OnRestorePurchasesClicked(IEvent payload)
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		List<GameObject> list = new List<GameObject>();
		list.Add(view.RestorePurchasesController);
		list.Add(view.messageDialogOverlay);
		base.dispatcher.Dispatch(RestorePlayerPurchasesEvent.RESTORE_PLAYER_PURCHASES, list);
	}

	private void OnManageAccountClicked(IEvent payload)
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch(ShowManageAccountEvent.SHOW_MANAGE_ACCOUNT, view.canvas.transform);
	}

	private void OnShowProtectedWebPageClicked(IEvent payload)
	{
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch(ShowProtectedWebPageEvent.SHOW_PROTECTED_WEB_PAGE, new ShowProtectedWebPagePayload(view.canvas.transform as RectTransform, payload.data as string));
	}

	public void Start()
	{
	}

	public override void OnRemove()
	{
		if (view != null)
		{
			view.dispatcher.RemoveListener("BACK", OnBackButtonClick);
			view.dispatcher.RemoveListener("MUTE", OnMuteValueChanged);
			view.dispatcher.RemoveListener("LOGOCLICK", OnLogoClicked);
			view.dispatcher.RemoveListener("RESTOREPURCHASES", OnRestorePurchasesClicked);
			view.dispatcher.RemoveListener("MANAGEACCOUNT", OnManageAccountClicked);
			view.dispatcher.RemoveListener("SHOWPROTECTEDWEBPAGE", OnShowProtectedWebPageClicked);
		}
	}

	public void OnDestroy()
	{
		OnRemove();
	}
}
