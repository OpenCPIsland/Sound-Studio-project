using Disney.ClubPenguin.CPModuleUtils;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RadioView : EventView
{
	internal const string BACK_CLICK_EVENT = "BACK_CLICK_EVENT";

	internal const string RANDOM_CLICK_EVENT = "RANDOM_CLICK_EVENT";

	internal const string FRIENDS_CLICK_EVENT = "FRIENDS_CLICK_EVENT";

	internal const string NEW_CLICK_EVENT = "NEW_CLICK_EVENT";

	public const string CONNECTION_ANIMATOR_TRIGGER_HIDE = "Hide";

	public const string CONNECTION_ANIMATOR_TRIGGER_SHOW = "Show";

	public GameObject RandomSelectedImage;

	public GameObject NewSelectedImage;

	public GameObject FriendsSelectedImage;

	public GameObject RandomUnselectedImage;

	public GameObject NewUnselectedImage;

	public GameObject FriendsUnselectedImage;

	public GameObject FriendsButton;

	public RadioRandomView radioRandomView;

	public RadioFriendsView radioFriendsView;

	public RadioNewView radioNewView;

	public GameObject CanvasObject;

	public Button backButton;

	public Animator NoConnectionPopupAnimator;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher contextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		base.Awake();
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	public void OnRandomClicked()
	{
		contextDispatcher.Dispatch("RANDOM_CLICK_EVENT");
	}

	public void OnFriendsClicked()
	{
		contextDispatcher.Dispatch("FRIENDS_CLICK_EVENT");
	}

	public void OnNewClicked()
	{
		contextDispatcher.Dispatch("NEW_CLICK_EVENT");
	}

	public void onBackClick()
	{
		base.dispatcher.Dispatch("BACK_CLICK_EVENT");
	}

	public void UpdateFriendsButton(bool selected)
	{
		FriendsSelectedImage.SetActive(selected);
		FriendsUnselectedImage.SetActive(!selected);
	}

	public void UpdateRandomButton(bool selected)
	{
		RandomSelectedImage.SetActive(selected);
		RandomUnselectedImage.SetActive(!selected);
	}

	public void UpdateNewButton(bool selected)
	{
		NewSelectedImage.SetActive(selected);
		NewUnselectedImage.SetActive(!selected);
	}

	public void ShowNoConnectionPopup()
	{
		NoConnectionPopupAnimator.SetTrigger("Show");
	}

	public void HideNoConnectionPopup()
	{
		NoConnectionPopupAnimator.SetTrigger("Hide");
	}

	public void HideAllButtonsButBack()
	{
		GameObject gameObject = GameObject.Find("TopPanel");
		if (gameObject != null)
		{
			gameObject.gameObject.SetActive(value: false);
		}
	}
}
