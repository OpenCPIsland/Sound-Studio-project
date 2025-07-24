using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections;
using UnityEngine;

public class RadioCategoryMediator : EventMediator
{
	protected RadioCategoryView view;

	protected RadioCategory _radioCategory;

	protected RadioSongCollection songCollection;

	protected RadioSongView currentRadioSong;

	protected RadioSongView nextRadioSong;

	protected bool isLoadingTracks;

	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		songCollection = new RadioSongCollection();
		_radioCategory = RadioCategory.FRIENDS;
		isLoadingTracks = false;
		view.isInTransition = false;
		view.dispatcher.AddListener("NEXT_CLICK_EVENT", OnNextClick);
		view.dispatcher.AddListener("PREVIOUS_CLICK_EVENT", OnPreviousClick);
		view.dispatcher.AddListener("INIT_EVENT", OnInit);
		ConfigureNagivation();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("NEXT_CLICK_EVENT", OnNextClick);
		view.dispatcher.RemoveListener("PREVIOUS_CLICK_EVENT", OnPreviousClick);
		view.dispatcher.RemoveListener("INIT_EVENT", OnInit);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFail);
		if (currentRadioSong != null)
		{
			currentRadioSong.radioSong.songVO.SongPlayEvent -= OnCurrentSongPlay;
		}
	}

	protected virtual void OnInit()
	{
		CheckRemainingTracks();
		if (currentRadioSong != null)
		{
			InitialTransition();
		}
	}

	protected virtual void LoadRadioBlock(int numSongs = 5)
	{
		if (songCollection.TotalSongs == 0)
		{
			ShowLoadingPanel();
		}
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFail);
		base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK, new LoadRadioBlockCommandPayload(RadioCategory.NEW, 5, songCollection.LastSongID));
		isLoadingTracks = true;
	}

	protected virtual void OnLoadRadioBlockComplete(IEvent evt)
	{
		LoadRadioBlockCompletePayload loadRadioBlockCompletePayload = evt.data as LoadRadioBlockCompletePayload;
		view.RemoveLoadingPanel();
		view.HidePopup();
		if (loadRadioBlockCompletePayload == null || loadRadioBlockCompletePayload.RadioCategory != _radioCategory)
		{
			return;
		}
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFail);
		isLoadingTracks = false;
		if (loadRadioBlockCompletePayload.RadioBlock.Count > 0)
		{
			songCollection.AddSongs(loadRadioBlockCompletePayload.RadioBlock);
			CheckRemainingTracks();
			if (currentRadioSong == null)
			{
				LoadRadioSong(songCollection.Current);
				TransitionSongs(RadioSongTransition.NEXT);
			}
		}
		ConfigureNagivation();
	}

	protected void OnLoadRadioBlockFail(IEvent evt)
	{
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, OnLoadRadioBlockFail);
		RadioCategory radioCategory = (RadioCategory)(int)evt.data;
		if (radioCategory == _radioCategory)
		{
			isLoadingTracks = false;
			view.ShowPopup("Prefabs/NoConnectionPanel", RadioError.Stop);
			songCollection.Clear();
		}
	}

	protected void OnNextClick()
	{
		if (!view.isInTransition)
		{
			LoadRadioSong(songCollection.Next());
			TransitionSongs(RadioSongTransition.NEXT);
			CheckRemainingTracks();
		}
	}

	protected void OnPreviousClick()
	{
		if (!view.isInTransition)
		{
			LoadRadioSong(songCollection.Previous());
			TransitionSongs(RadioSongTransition.PREVIOUS);
		}
	}

	protected void CheckRemainingTracks()
	{
		if (songCollection.TotalSongs == 0 && !isLoadingTracks)
		{
			LoadRadioBlock(1);
		}
		else if (songCollection.RemainingSongs < 5 && !isLoadingTracks)
		{
			LoadRadioBlock();
		}
		else if (base.gameObject.activeSelf)
		{
			StartCoroutine(RemoveLoadingPanel());
		}
	}

	private IEnumerator RemoveLoadingPanel()
	{
		yield return new WaitForSeconds(1f);
		view.RemoveLoadingPanel();
	}

	protected void ShowLoadingPanel()
	{
		view.ShowLoadingPanel();
	}

	protected void LoadRadioSong(RadioSongVO radioSong)
	{
		if (view != null && radioSong != null)
		{
			nextRadioSong = CreateRadioSong();
			nextRadioSong.loadData(radioSong);
		}
	}

	protected void InitialTransition()
	{
		currentRadioSong.transform.position = Vector3.zero;
		currentRadioSong.AnimateEnterRight();
		view.isInTransition = true;
		Invoke("OnInitialSongTransitionComplete", 0.4f);
	}

	protected void TransitionSongs(RadioSongTransition transition)
	{
		switch (transition)
		{
		case RadioSongTransition.NEXT:
			if (currentRadioSong != null)
			{
				currentRadioSong.AnimateExitLeft();
			}
			nextRadioSong.AnimateEnterRight();
			break;
		case RadioSongTransition.PREVIOUS:
			if (currentRadioSong != null)
			{
				currentRadioSong.AnimateExitRight();
			}
			nextRadioSong.AnimateEnterLeft();
			break;
		}
		view.isInTransition = true;
		Invoke("OnSongTransitionComplete", 0.4f);
		view.SetNagivationArrowState(isLoading: true);
	}

	protected void OnInitialSongTransitionComplete()
	{
		view.isInTransition = false;
		if (base.gameObject.activeSelf)
		{
			currentRadioSong.playButton_Click_Handler();
		}
	}

	protected void OnSongTransitionComplete()
	{
		view.isInTransition = false;
		RemoveCurrentSong();
		currentRadioSong = nextRadioSong;
		base.dispatcher.AddListener("RADIO_SONG_COMPLETE_EVENT", OnRadioSongComplete);
		currentRadioSong.radioSong.songVO.SongPlayEvent += OnCurrentSongPlay;
		ConfigureNagivation();
		if (base.gameObject.activeSelf)
		{
			currentRadioSong.playButton_Click_Handler();
		}
	}

	private void OnCurrentSongPlay()
	{
		currentRadioSong.radioSong.songVO.SongPlayEvent -= OnCurrentSongPlay;
		view.SetNagivationArrowState(isLoading: false);
	}

	protected void RemoveCurrentSong()
	{
		if (currentRadioSong != null)
		{
			base.dispatcher.RemoveListener("RADIO_SONG_COMPLETE_EVENT", OnRadioSongComplete);
			base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
			UnityEngine.Object.Destroy(currentRadioSong.gameObject);
		}
	}

	protected RadioSongView CreateRadioSong()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/RadioSongPrefab")) as GameObject;
		gameObject.transform.SetParent(view.songContainerObject.transform, worldPositionStays: false);
		return gameObject.GetComponent("RadioSongView") as RadioSongView;
	}

	protected void OnRadioSongComplete(IEvent evt)
	{
		if (currentRadioSong != null)
		{
			RadioSongView x = (RadioSongView)evt.data;
			if (x == currentRadioSong && songCollection.RemainingSongs > 0 && base.gameObject.activeSelf)
			{
				OnNextClick();
			}
		}
	}

	protected void ConfigureNagivation()
	{
		view.nextButton.SetActive(value: true);
		view.previousButton.SetActive(value: true);
		if (songCollection.RemainingSongs <= 0)
		{
			view.nextButton.SetActive(value: false);
		}
		if (songCollection.RemainingSongs == songCollection.TotalSongs - 1)
		{
			view.previousButton.SetActive(value: false);
		}
	}
}
