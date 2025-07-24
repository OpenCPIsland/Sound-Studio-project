

// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MembershipBenefits
using System.Collections;
using System.Collections.Generic;
using DevonLocalization.Core;
using Disney.ClubPenguin.CPModuleUtils;
using Disney.ClubPenguin.WebPageViewer;
using Disney.DMOAnalytics;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DisplayNativeWebPage), typeof(AudioSource))]
public class MembershipBenefits : MonoBehaviour
{
	public delegate void MembershipBenefitsBackDelegate();

	private static string MEMBERBENEFITS_LOC_PATH = "Assets/Framework/MembershipBenefits/Resources/Translations";

	private static string MEMBERBENEFITS_MODULE = "MEMBERBENEFITS";

	private static string URLTokenAndroid = "memberbenefits.url.android";

	public static string CPAndroidAppURL = "com.disney.cpcompanion_goo";

	public static string CPAndroidStoreURL = "https://play.google.com/store/apps/details?id=com.disney.cpcompanion_goo";

	private static string CONTEXT = "context";

	private static string CONTEXT_VALUE = "get_membership_attempt";

	private static string ACTION = "action";

	private static string ACTION_VALUE = string.Empty;

	private static string PAGE_VIEW = "page_view";

	private static string PLAYER_ID = "player_id";

	private static string PLAYER_NULL = "NULL";

	private static string LOCATION = "location";

	private static string MEMBER_BENEFITS = "membership_page";

	public Button BuyNowButton;

	public Image BuyNowMemberBadge;

	public GameObject GaryImage;

	public Button BackButton;

	public AudioClip BackButtonSound;

	public AudioClip BuyNowButtonSound;

	public long PlayerID;

	public MembershipBenefitsBackDelegate BackButtonPressed;

	private DisplayNativeWebPage displayNativeWebPage;

	private string TranslatedPageURL;

	private void Awake()
	{
		GaryImage.SetActive(false);
		SetupLocalization();
		GetLocalizedWebPage(URLTokenAndroid);
	}

	private void Start()
	{
		HardwareBackButtonDispatcher.SetTargetClickHandler(BackButton, false);
		SendBIPageView();
	}

	private void GetLocalizedWebPage(string urlToken)
	{
		TranslatedPageURL = Localizer.Instance.GetTokenTranslation(urlToken);
	}

	private void SetupLocalization()
	{
		ILocalizedTokenFilePath path = new ModuleTokensFilePath(MEMBERBENEFITS_LOC_PATH, MEMBERBENEFITS_MODULE, Platform.global);
		Localizer.Instance.LoadTokensFromLocalJSON(path, OnTokensLoaded);
	}

	private void OnTokensLoaded(bool tokensUpdated)
	{
		Debug.Log((object)"Memberbenefits Tokens Loaded.");
	}

	public void ShowWebPage()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)displayNativeWebPage == (Object)null)
		{
			displayNativeWebPage = ((Component)this).GetComponent<DisplayNativeWebPage>();
		}
		if ((int)Application.internetReachability != 0)
		{
			displayNativeWebPage.DisplayPage(TranslatedPageURL);
			return;
		}
		GaryImage.SetActive(true);
		((Selectable)BuyNowButton).interactable = false;
		Image buyNowMemberBadge = BuyNowMemberBadge;
		ColorBlock colors = ((Selectable)BuyNowButton).colors;
		((Graphic)buyNowMemberBadge).color = colors.disabledColor;
	}

	public void OnMembershipPlanButtonPress()
	{
		((Component)this).GetComponent<AudioSource>().PlayOneShot(BuyNowButtonSound);
		SendBIGameActionExitApp();
		new OpenAppOnDeviceCMD(CPAndroidAppURL, CPAndroidStoreURL).Execute();
	}

	public void OnBackButton()
	{
		((Component)this).GetComponent<AudioSource>().PlayOneShot(BackButtonSound);
		((MonoBehaviour)this).StartCoroutine(WaitForSound());
	}

	private IEnumerator WaitForSound()
	{
		yield return null;
		if ((Object)(object)displayNativeWebPage != (Object)null)
		{
			displayNativeWebPage.OnCloseButton();
		}
		if (BackButtonPressed != null)
		{
			BackButtonPressed();
		}
	}

	private void SendBIPageView()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (PlayerID == 0L)
		{
			dictionary.Add(PLAYER_ID, PLAYER_NULL);
		}
		else
		{
			dictionary.Add(PLAYER_ID, PlayerID);
		}
		dictionary.Add(LOCATION, MEMBER_BENEFITS);
		DMOAnalytics.SharedAnalytics.LogAnalyticsEventWithContext(PAGE_VIEW, dictionary);
	}

	private void SendBIGameActionExitApp()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add(CONTEXT, CONTEXT_VALUE);
		dictionary.Add(ACTION, ACTION_VALUE);
		if (PlayerID == 0L)
		{
			dictionary.Add(PLAYER_ID, PLAYER_NULL);
		}
		else
		{
			dictionary.Add(PLAYER_ID, PlayerID);
		}
		DMOAnalytics.SharedAnalytics.LogGameAction(dictionary);
	}
}
