using SoundStudio.Event;
using SoundStudio.Model;
using DevonLocalization;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class SongListItemView : EventView
{
	private const float DOUBLE_CLICK_INTERVAL_SECONDS = 0.35f;

	internal const string PLAY_EVENT = "PLAY_EVENT";

	internal const string STOP_EVENT = "STOP_EVENT";

	internal const string DELETE_EVENT = "DELETE_EVENT";

	internal const string EXPORT_EVENT = "EXPORT_EVENT";

	internal const string RENAME_EVENT = "RENAME_EVENT";

	internal const string SYNC_EVENT = "SYNC_EVENT";

	internal const string LOADED_EVENT = "LOADED_EVENT";

	internal const string UNSYNC_EVENT = "UNSYNC_EVENT";

	public Text nameText;

	public GameObject buttonPlay;

	public GameObject buttonStop;

	public GameObject DeleteConfirm;

	public GameObject DeleteConfirmYesButton;

	public GameObject SharingPanel;

	public GameObject LoadingPanel;

	public GameObject SharingButton_Shared;

	public GameObject SharingButton_NotShared;

	public AudioClip SongDeleteSound;

	public Image SyncedImage;

	public GameObject UnsyncedImage;

	public SongProgressView songProgressView;

	public GameObject CanvasObject;

	public Image bgImage;

	public SongVO song;

	private Button nameButton;

	private float lastNameClickTime = float.MinValue;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		base.Awake();
		ConfigureNameButton();
		ConfigureSaveButtons();
	}

	public void loadSong(SongVO song)
	{
		this.song = song;
		nameText.text = song.songName;
		songProgressView.LoadSong(song);
		ConfigureSaveButtons();
	}

	private void ConfigureSaveButtons()
	{
		CenterSaveButton(SharingButton_Shared);
		CenterSaveButton(SharingButton_NotShared);
		ConfigureSaveButtonText(SharingButton_Shared);
		ConfigureSaveButtonText(SharingButton_NotShared);
		if (SharingPanel != null)
		{
			SharingPanel.SetActive(value: true);
			TrimSavePanelToButtonRow();
		}
		HideLegacyShareStatus();
		if (SharingButton_NotShared != null)
		{
			SharingButton_NotShared.SetActive(value: true);
		}
		if (SharingButton_Shared != null)
		{
			SharingButton_Shared.SetActive(value: false);
		}
	}

	private void ConfigureNameButton()
	{
		if (nameText == null)
		{
			return;
		}
		nameText.raycastTarget = true;
		if (nameButton == null)
		{
			nameButton = nameText.GetComponent<Button>();
			if (nameButton == null)
			{
				nameButton = nameText.gameObject.AddComponent<Button>();
			}
			nameButton.onClick.RemoveListener(songName_Click_Handler);
			nameButton.onClick.AddListener(songName_Click_Handler);
		}
		nameButton.transition = Selectable.Transition.None;
		nameButton.targetGraphic = nameText;
		Navigation navigation = nameButton.navigation;
		navigation.mode = Navigation.Mode.None;
		nameButton.navigation = navigation;
	}

	private void ConfigureSaveButtonText(GameObject buttonObject)
	{
		if (buttonObject == null)
		{
			return;
		}
		foreach (LocalizedText item in buttonObject.GetComponentsInChildren<LocalizedText>(includeInactive: true))
		{
			item.doNotLocalize = true;
			item.token = string.Empty;
		}
		foreach (Text item2 in buttonObject.GetComponentsInChildren<Text>(includeInactive: true))
		{
			item2.text = "SAVE";
		}
	}

	private void CenterSaveButton(GameObject buttonObject)
	{
		RectTransform rectTransform = buttonObject?.transform as RectTransform;
		if (rectTransform == null)
		{
			return;
		}
		rectTransform.anchorMin = new Vector2(0.14f, 0.0622022f);
		rectTransform.anchorMax = new Vector2(0.86f, 0.971105f);
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
	}

	private void TrimSavePanelToButtonRow()
	{
		GameObject gameObject = SharingButton_Shared?.transform.parent?.gameObject;
		if (gameObject == null)
		{
			gameObject = SharingButton_NotShared?.transform.parent?.gameObject;
		}
		if (gameObject == null || SharingPanel == null)
		{
			return;
		}
		foreach (Transform item in SharingPanel.transform)
		{
			item.gameObject.SetActive(item.gameObject == gameObject);
		}
		RectTransform rectTransform = gameObject.transform as RectTransform;
		if (rectTransform != null)
		{
			rectTransform.anchorMin = new Vector2(0f, 0f);
			rectTransform.anchorMax = new Vector2(1f, 1f);
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
		}
	}

	private void HideLegacyShareStatus()
	{
		if (SyncedImage != null)
		{
			SyncedImage.gameObject.SetActive(value: false);
		}
		if (UnsyncedImage != null)
		{
			UnsyncedImage.SetActive(value: false);
		}
	}

	public void deleteButton_Click_Handler()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		DeleteConfirm.gameObject.SetActive(value: true);
	}

	public void deleteConfirm_Click_Handler()
	{
		base.dispatcher.Dispatch("DELETE_EVENT", song);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
	}

	public void deleteCancel_Click_Handler()
	{
		DeleteConfirm.gameObject.SetActive(value: false);
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
	}

	public void playButton_Click_Handler()
	{
		base.dispatcher.Dispatch("PLAY_EVENT", song);
	}

	public void stopButton_Click_Handler()
	{
		base.dispatcher.Dispatch("STOP_EVENT", song);
	}

	public void Song_Stop_Handler()
	{
		buttonPlay.SetActive(value: true);
		buttonStop.SetActive(value: false);
	}

	public void Song_Play_Handler()
	{
		buttonPlay.SetActive(value: false);
		buttonStop.SetActive(value: true);
	}

	public void renameButton_Click_Handler()
	{
		base.dispatcher.Dispatch("RENAME_EVENT", song);
	}

	public void songName_Click_Handler()
	{
		if (song == null)
		{
			return;
		}
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime - lastNameClickTime <= DOUBLE_CLICK_INTERVAL_SECONDS)
		{
			lastNameClickTime = float.MinValue;
			ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
			base.dispatcher.Dispatch("RENAME_EVENT", song);
			return;
		}
		lastNameClickTime = unscaledTime;
	}

	public void ShowSongSynced()
	{
		HideLegacyShareStatus();
	}

	public void ShowSongUnsynced()
	{
		HideLegacyShareStatus();
	}

	public void showSongShared()
	{
		ConfigureSaveButtons();
	}

	public void showSongUnshared()
	{
		ConfigureSaveButtons();
	}

	public void OnShareClick()
	{
		DispatchExport();
	}

	public void OnUnshareClick()
	{
		DispatchExport();
	}

	public void exportButton_Click_Handler()
	{
		DispatchExport();
	}

	private void DispatchExport()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch("EXPORT_EVENT", song);
	}
}
