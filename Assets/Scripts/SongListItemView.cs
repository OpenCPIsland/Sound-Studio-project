using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class SongListItemView : EventView
{
	internal const string PLAY_EVENT = "PLAY_EVENT";

	internal const string STOP_EVENT = "STOP_EVENT";

	internal const string DELETE_EVENT = "DELETE_EVENT";

	internal const string RENAME_EVENT = "RENAME_EVENT";

	internal const string SYNC_EVENT = "SYNC_EVENT";

	internal const string LOADED_EVENT = "LOADED_EVENT";

	internal const string UNSYNC_EVENT = "UNSYNC_EVENT";

	internal const string SHARE_EVENT = "SHARE_EVENT";

	internal const string UNSHARE_EVENT = "UNSHARE_EVENT";

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

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher ContextDispatcher
	{
		get;
		set;
	}

	public void loadSong(SongVO song)
	{
		this.song = song;
		nameText.text = song.songName;
		songProgressView.LoadSong(song);
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

	public void ShowSongSynced()
	{
		SyncedImage.gameObject.SetActive(value: true);
		UnsyncedImage.SetActive(value: false);
	}

	public void ShowSongUnsynced()
	{
		SyncedImage.gameObject.SetActive(value: false);
		UnsyncedImage.SetActive(value: true);
	}

	public void showSongShared()
	{
		SharingButton_NotShared.SetActive(value: false);
		SharingButton_Shared.SetActive(value: true);
	}

	public void showSongUnshared()
	{
		SharingButton_NotShared.SetActive(value: true);
		SharingButton_Shared.SetActive(value: false);
	}

	public void OnShareClick()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch("SHARE_EVENT");
	}

	public void OnUnshareClick()
	{
		ContextDispatcher.Dispatch(SFXEvent.SFX_CLICK_SELECT);
		base.dispatcher.Dispatch("UNSHARE_EVENT");
	}
}
