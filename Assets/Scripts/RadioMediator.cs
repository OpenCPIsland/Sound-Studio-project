using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;

public class RadioMediator : EventMediator
{
	private RadioCategory currentCategory;

	private bool isNoConnectionVisible;

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	[Inject]
	public RadioView view
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		isNoConnectionVisible = false;
		view.dispatcher.AddListener("BACK_CLICK_EVENT", OnBackClicked);
		base.dispatcher.AddListener("FRIENDS_CLICK_EVENT", OnFriendsClicked);
		base.dispatcher.AddListener("NEW_CLICK_EVENT", OnNewClicked);
		base.dispatcher.AddListener("RANDOM_CLICK_EVENT", OnRandomClicked);
		base.dispatcher.AddListener("NO_CONNECTION_EVENT", OnNoConnection);
		base.dispatcher.AddListener("GET_CONNECTION_EVENT", OnGetConnection);
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFailed);
		view.UpdateFriendsButton(selected: false);
		view.UpdateRandomButton(selected: true);
		view.UpdateNewButton(selected: false);
		HandleFriendsSwitch();
		base.dispatcher.AddListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
		ShowRadioCategory(RadioCategory.RANDOM);
	}

	private void OnLoadRadioBlockFailed(IEvent payload)
	{
		view.HideAllButtonsButBack();
	}

	private void HandleFriendsSwitch()
	{
	}

	private void OnLoginSuccess(IEvent payload)
	{
		OnFriendsClicked();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("BACK_CLICK_EVENT", OnBackClicked);
		base.dispatcher.RemoveListener("FRIENDS_CLICK_EVENT", OnFriendsClicked);
		base.dispatcher.RemoveListener("NEW_CLICK_EVENT", OnNewClicked);
		base.dispatcher.RemoveListener("RANDOM_CLICK_EVENT", OnRandomClicked);
		base.dispatcher.RemoveListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
		base.dispatcher.RemoveListener("NO_CONNECTION_EVENT", OnNoConnection);
		base.dispatcher.RemoveListener("GET_CONNECTION_EVENT", OnGetConnection);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFailed);
	}

	public void OnBackClicked()
	{
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
		base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.StartScreen);
	}

	public void OnFriendsClicked()
	{
		if (!IsViewInTransition())
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			view.UpdateFriendsButton(selected: true);
			view.UpdateRandomButton(selected: false);
			view.UpdateNewButton(selected: false);
			ShowRadioCategory(RadioCategory.FRIENDS);
		}
	}

	public void OnNewClicked()
	{
		if (!IsViewInTransition())
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			view.UpdateFriendsButton(selected: false);
			view.UpdateRandomButton(selected: false);
			view.UpdateNewButton(selected: true);
			ShowRadioCategory(RadioCategory.NEW);
		}
	}

	public void OnRandomClicked()
	{
		if (!IsViewInTransition())
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			view.UpdateFriendsButton(selected: false);
			view.UpdateRandomButton(selected: true);
			view.UpdateNewButton(selected: false);
			ShowRadioCategory(RadioCategory.RANDOM);
		}
	}

	private void ShowRadioCategory(RadioCategory category)
	{
		currentCategory = category;
		view.radioRandomView.gameObject.SetActive(value: false);
		view.radioNewView.gameObject.SetActive(value: false);
		view.radioFriendsView.gameObject.SetActive(value: false);
		base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		switch (currentCategory)
		{
		case RadioCategory.RANDOM:
			view.radioRandomView.gameObject.SetActive(value: true);
			view.radioRandomView.OnInit();
			break;
		case RadioCategory.FRIENDS:
			view.radioFriendsView.gameObject.SetActive(value: true);
			view.radioFriendsView.OnInit();
			break;
		case RadioCategory.NEW:
			view.radioNewView.gameObject.SetActive(value: true);
			view.radioNewView.OnInit();
			break;
		}
	}

	private bool IsViewInTransition()
	{
		RadioCategoryView radioCategoryView = view.radioRandomView;
		switch (currentCategory)
		{
		case RadioCategory.RANDOM:
			radioCategoryView = view.radioRandomView;
			break;
		case RadioCategory.FRIENDS:
			radioCategoryView = view.radioFriendsView;
			break;
		case RadioCategory.NEW:
			radioCategoryView = view.radioNewView;
			break;
		}
		return radioCategoryView.isInTransition;
	}

	private void StopCategory(RadioCategory category)
	{
		switch (category)
		{
		case RadioCategory.RANDOM:
			view.radioRandomView.gameObject.SetActive(value: true);
			view.radioRandomView.OnInit();
			break;
		case RadioCategory.FRIENDS:
			view.radioFriendsView.gameObject.SetActive(value: true);
			view.radioFriendsView.OnInit();
			break;
		case RadioCategory.NEW:
			view.radioNewView.gameObject.SetActive(value: true);
			view.radioNewView.OnInit();
			break;
		}
	}

	private void OnNoConnection()
	{
		if (!isNoConnectionVisible)
		{
			view.ShowNoConnectionPopup();
			isNoConnectionVisible = true;
		}
	}

	private void OnGetConnection()
	{
		if (isNoConnectionVisible)
		{
			view.HideNoConnectionPopup();
			isNoConnectionVisible = false;
		}
	}
}
