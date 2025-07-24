using DevonLocalization.Core;
using Disney.ClubPenguin.CPModuleUtils;
using Disney.ClubPenguin.WebPageViewer;
using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System;
using UnityEngine;
using UnityEngine.UI;

internal class SettingsView : View
{
	internal const string LOGO_CLICK_EVENT = "LOGOCLICK";

	internal const string BACK_CLICK_EVENT = "BACK";

	internal const string RESTORE_PURCHASES_CLICK_EVENT = "RESTOREPURCHASES";

	internal const string MANAGE_ACCOUNT_CLICK_EVENT = "MANAGEACCOUNT";

	internal const string LICENSE_CREDTIS_CLICK_EVENT = "LICENSECREDITS";

	internal const string SHOW_PROTECTED_WEB_PAGE_CLICK_EVENT = "SHOWPROTECTEDWEBPAGE";

	internal const string MUTE_VALUE_CHANGED = "MUTE";

	public Toggle muteToggle;

	public GameObject RestorePurchasesController;

	public GameObject messageDialogOverlay;

	public RectTransform canvas;

	public Button manageAccountButton;

	public Button backButton;

	public Text VersionText;

	public DisplayNativeWebPage NativeWebPagePrefab;

	private GameObject popupLicense;

	private DisplayNativeWebPage nativeWebPageinstance;

	private int logoClickCount;

	[Inject]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher contextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		base.Start();
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	protected new void OnDestroy()
	{
	}

	private void DispatchShowProtectedWeb(string urlToken)
	{
		dispatcher.Dispatch("SHOWPROTECTEDWEBPAGE", Localizer.Instance.GetTokenTranslation(urlToken));
	}

	public void OnTermsClick()
	{
		DispatchShowProtectedWeb("soundstudio.settings.terms");
	}

	public void OnPrivacyClick()
	{
		DispatchShowProtectedWeb("soundstudio.settings.privacypolicyurl");
	}

	public void OnDisneyClick()
	{
		DispatchShowProtectedWeb("soundstudio.settings.disneyurl");
	}

	public void OnSupportClick()
	{
		DispatchShowProtectedWeb("soundstudio.settings.supporturl");
	}

	public void OnRulesClick()
	{
		contextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		nativeWebPageinstance = (UnityEngine.Object.Instantiate(NativeWebPagePrefab) as DisplayNativeWebPage);
		nativeWebPageinstance.GetComponent<RectTransform>().SetParent(canvas, worldPositionStays: false);
		DisplayNativeWebPage displayNativeWebPage = nativeWebPageinstance;
		displayNativeWebPage.Closed = (DisplayNativeWebPage.ClosedDelegate)Delegate.Combine(displayNativeWebPage.Closed, new DisplayNativeWebPage.ClosedDelegate(OnWebPageClosed));
		nativeWebPageinstance.DisplayPage(Localizer.Instance.GetTokenTranslation("soundstudio.settings.rulesurl"));
	}

	private void OnWebPageClosed()
	{
		contextDispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		DisplayNativeWebPage displayNativeWebPage = nativeWebPageinstance;
		displayNativeWebPage.Closed = (DisplayNativeWebPage.ClosedDelegate)Delegate.Remove(displayNativeWebPage.Closed, new DisplayNativeWebPage.ClosedDelegate(OnWebPageClosed));
		UnityEngine.Object.Destroy(nativeWebPageinstance.gameObject);
	}

	public void OnChildrensPrivacyClick()
	{
		DispatchShowProtectedWeb("soundstudio.settings.childprivacyurl");
	}

	public void OnClickBackButton()
	{
		dispatcher.Dispatch("BACK");
	}

	public void OnClickLogo()
	{
		dispatcher.Dispatch("LOGOCLICK");
	}

	public void OnMuteValueChanged()
	{
		dispatcher.Dispatch("MUTE", muteToggle.isOn);
	}

	public void SetMuteToggleControlValue(bool value)
	{
		muteToggle.isOn = value;
	}

	public void OnClickLicense()
	{
		popupLicense = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/SettingLicensePrefab")) as GameObject);
		contextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		popupLicense.transform.SetParent(canvas.transform, worldPositionStays: false);
	}

	public void OnClickRestorePurchases()
	{
		dispatcher.Dispatch("RESTOREPURCHASES");
	}

	public void OnClickManageAccount()
	{
		dispatcher.Dispatch("MANAGEACCOUNT");
	}
}
