using Disney.ClubPenguin.Login.Authentication;
using Disney.ClubPenguin.ParentPermission;
using InAppPurchases;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StartScreenView : View
{
	internal const string ABOUT_MEMBERSHIP_EVENT = "ABOUT_MEMBERSHIP";

	internal const string PLAY_CLICK_EVENT = "PLAY_CLICK_EVENT";

	internal const string SONGS_CLICK_EVENT = "SONGS_CLICK_EVENT";

	internal const string LOGIN_CLICK_EVENT = "LOGIN_CLICK_EVENT";

	internal const string RADIO_CLICK_EVENT = "RADIO_CLICK_EVENT";

	internal const string STORE_CLICK_EVENT = "STORE_CLICK_EVENT";

	internal const string SETTINGS_CLICK_EVENT = "SETTINGS_CLICK_EVENT";

	internal const string REFERRAL_CLICK_EVENT = "REFERRAL_CLICK_EVENT";

	internal const string LOGOUT_CLICK_EVENT = "LOGOUT_CLICK_EVENT";

	internal const string CP_BUTTON_CLICK_EVENT = "CP_BUTTON_CLICK_EVENT";

	internal const string BACK_CLICK_EVENT = "BACK_CLICK_EVENT";

	public GameObject penguinImage_Guest;

	public GameObject penguinImage_Color;

	public Image penguinImage_Tint;

	public GameObject canvasObject;

	public GameObject MenuContainer;

	public MemberBenefitsClickedHandler IapContentPanel;

	public GameObject StartScreenViewGameObject;

	public LoginContext LoginContextPrefab;

	public RectTransform LoginCanvas;

	public RectTransform AboutMembershipCanvas;

	public IAPContext IAPContext;

	public ParentPermissionController ParentPermissionController;

	public Text penguinNameText;

	public MembershipBenefits AboutMembershipPrefab;

	public GameObject SwitchUserButton;

	public GameObject LogoutButton;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	private void Update()
	{
		if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
		{
			dispatcher.Dispatch("BACK_CLICK_EVENT");
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public void OnClickAboutMembership()
	{
		dispatcher.Dispatch("ABOUT_MEMBERSHIP", this);
	}

	public void onPlayClick()
	{
		dispatcher.Dispatch("PLAY_CLICK_EVENT");
	}

	public void onSongsClick()
	{
		dispatcher.Dispatch("SONGS_CLICK_EVENT");
	}

	public void onLoginClick()
	{
		dispatcher.Dispatch("LOGIN_CLICK_EVENT", this);
	}

	public void OnLogoutClick()
	{
		dispatcher.Dispatch("LOGOUT_CLICK_EVENT");
	}

	public void OnSettingsClick()
	{
		dispatcher.Dispatch("SETTINGS_CLICK_EVENT");
	}

	public void OnStoreClick()
	{
		dispatcher.Dispatch("STORE_CLICK_EVENT");
	}

	public void OnRadioClick()
	{
		dispatcher.Dispatch("RADIO_CLICK_EVENT");
	}

	public void OnReferralClick()
	{
		dispatcher.Dispatch("REFERRAL_CLICK_EVENT");
	}

	public void OnClickCPButton()
	{
		dispatcher.Dispatch("CP_BUTTON_CLICK_EVENT");
	}
}
