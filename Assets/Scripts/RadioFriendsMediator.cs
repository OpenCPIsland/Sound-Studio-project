// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// RadioFriendsMediator
using System.Collections.Generic;
using Disney.ClubPenguin.Service.Friends;
using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class RadioFriendsMediator : RadioCategoryMediator
{
	private List<Friend> friendsList;

	private int friendsIndex;

	private bool isInit;

	[Inject]
	public RadioFriendsView radioFriendsView { get; set; }

	public override void OnRegister()
	{
		songCollection = new RadioSongCollection();
		_radioCategory = RadioCategory.FRIENDS;
		isLoadingTracks = false;
		view = radioFriendsView;
		isInit = false;
		view.dispatcher.AddListener("NEXT_CLICK_EVENT", base.OnNextClick);
		view.dispatcher.AddListener("PREVIOUS_CLICK_EVENT", base.OnPreviousClick);
		view.dispatcher.AddListener("INIT_EVENT", OnInit);
		ConfigureNagivation();
	}

	public override void OnRemove()
	{
		view.dispatcher.RemoveListener("NEXT_CLICK_EVENT", base.OnNextClick);
		view.dispatcher.RemoveListener("PREVIOUS_CLICK_EVENT", base.OnPreviousClick);
		view.dispatcher.RemoveListener("INIT_EVENT", OnInit);
		base.dispatcher.RemoveListener(FriendsEvent.FRIENDS_REFRESH_SUCCESS, LoadFriendsData);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, base.OnLoadRadioBlockFail);
	}

	protected override void OnInit()
	{
		if (base.application.currentPlayer.AccountStatus == MembershipStatus.GUEST)
		{
			ShowGuestFriendsPanel();
			ConfigureNagivation();
		}
		else if (base.application.currentPlayer.FriendCollectionStatus == FriendCollectionStatus.UNINITIALIZED)
		{
			base.dispatcher.AddListener(FriendsEvent.FRIENDS_REFRESH_SUCCESS, LoadFriendsData);
			ShowGetFriendsPanel();
			ConfigureNagivation();
		}
		else if (!isInit)
		{
			LoadFriendsData();
		}
		isInit = true;
		if (currentRadioSong != null)
		{
			InitialTransition();
			CheckRemainingTracks();
		}
		if (base.application.currentPlayer.AccountStatus == MembershipStatus.GUEST || (friendsList != null && friendsList.Count == 0))
		{
			base.dispatcher.Dispatch(SongStopEvent.SONG_STOP);
		}
	}

	private void LoadFriendsData()
	{
		base.dispatcher.RemoveListener(FriendsEvent.FRIENDS_REFRESH_SUCCESS, LoadFriendsData);
		friendsList = base.application.currentPlayer.FriendsTrackCollection.GetFriends();
		if (friendsList.Count == 0)
		{
			ShowGetFriendsPanel();
			return;
		}
		friendsIndex = 0;
		if (base.application.currentPlayer.AccountStatus == MembershipStatus.GUEST)
		{
			ShowGuestFriendsPanel();
		}
		else
		{
			LoadRadioBlock(1);
		}
	}

	protected override void LoadRadioBlock(int numSongs = 5)
	{
		view.HidePopup();
		if (songCollection.TotalSongs == 0)
		{
			ShowLoadingPanel();
		}
		if (friendsIndex >= friendsList.Count)
		{
			isLoadingTracks = false;
			view.RemoveLoadingPanel();
			if (songCollection.TotalSongs == 0 && friendsList.Count > 0)
			{
				view.ShowPopup("Prefabs/FriendsNotSharingPanel");
			}
		}
		else
		{
			int num = Mathf.Min(numSongs, friendsList.Count - friendsIndex);
			List<Friend> range = friendsList.GetRange(friendsIndex, num);
			List<string> friendsSWIDS = getFriendsSWIDS(range);
			friendsIndex += num;
			base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
			base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, base.OnLoadRadioBlockFail);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK, new LoadRadioBlockCommandPayload(_radioCategory, friendsSWIDS));
			isLoadingTracks = true;
		}
	}

	protected override void OnLoadRadioBlockComplete(IEvent evt)
	{
		LoadRadioBlockCompletePayload loadRadioBlockCompletePayload = (LoadRadioBlockCompletePayload)evt.data;
		if (loadRadioBlockCompletePayload.RadioCategory != _radioCategory)
		{
			return;
		}
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, base.OnLoadRadioBlockFail);
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
			view.HidePopup();
			view.RemoveLoadingPanel();
		}
		else if (friendsIndex < friendsList.Count)
		{
			CheckRemainingTracks();
		}
		else if (songCollection.TotalSongs == 0)
		{
			view.ShowPopup("Prefabs/FriendsNotSharingPanel");
		}
		ConfigureNagivation();
	}

	private void ShowGetFriendsPanel()
	{
		view.ShowPopup("Prefabs/GetFriendsPanel");
	}

	private void ShowGuestFriendsPanel()
	{
		view.ShowPopup("Prefabs/GuestFriendsPrefab");
	}

	public List<string> getFriendsSWIDS(List<Friend> friends)
	{
		List<string> list = new List<string>();
		foreach (Friend friend in friends)
		{
			list.Add(friend.Swid);
		}
		return list;
	}
}
