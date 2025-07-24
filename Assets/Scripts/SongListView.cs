using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class SongListView : View
{
	internal const string NEW_CLICK_EVENT = "new_click";

	internal const string BACK_CLICK_EVENT = "back_click";

	public GameObject SongUIPrefab;

	public GameObject LoadingPanel;

	public GameObject SongListContent;

	public GameObject CanvasObject;

	public Text SyncCountText;

	public GameObject LoadingSongPanel;

	public Button backButton;

	[Inject]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	protected override void Awake()
	{
		base.Awake();
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	public void newSong_Click_Handler()
	{
		dispatcher.Dispatch("new_click");
	}

	public void addSong(SongVO song)
	{
		GameObject gameObject = Object.Instantiate(SongUIPrefab, new Vector3(0f, 0f, 0f), default(Quaternion)) as GameObject;
		gameObject.transform.SetParent(SongListContent.transform);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		SongListItemView songListItemView = gameObject.GetComponent("SongListItemView") as SongListItemView;
		songListItemView.loadSong(song);
		songListItemView.CanvasObject = CanvasObject;
	}

	public void removeSong(SongVO song)
	{
		SongListItemView[] componentsInChildren = SongListContent.GetComponentsInChildren<SongListItemView>();
		foreach (SongListItemView songListItemView in componentsInChildren)
		{
			if (songListItemView.song.songName == song.songName)
			{
				UnityEngine.Object.Destroy(songListItemView.gameObject);
			}
		}
	}

	public void onBackClick()
	{
		dispatcher.Dispatch("back_click");
	}

	public void ShowLoadingPanel()
	{
		LoadingPanel.SetActive(value: true);
	}

	public void HideLoadingPanel()
	{
		LoadingPanel.SetActive(value: false);
	}

	public void ShowSongLoadingPanel()
	{
		if (LoadingSongPanel == null)
		{
			LoadingSongPanel = (Object.Instantiate(Resources.Load("Prefabs/LoadingPanel")) as GameObject);
			LoadingSongPanel.transform.SetParent(CanvasObject.transform, worldPositionStays: false);
		}
	}

	public void HideSongLoadingPanel()
	{
		if (LoadingSongPanel != null)
		{
			UnityEngine.Object.Destroy(LoadingSongPanel);
			LoadingSongPanel = null;
		}
	}
}
