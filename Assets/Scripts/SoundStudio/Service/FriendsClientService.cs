using Disney.ClubPenguin.Service.DirectoryService;
using Disney.ClubPenguin.Service.Friends;
using Disney.Xmpp;
using Matrix;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using UnityEngine;

namespace SoundStudio.Service
{
	public class FriendsClientService
	{
		public const float TIMER_INTERVAL_SECONDS = 2f;

		private Timer timer;

		private MonoBehaviour root;

		private bool refreshComplete;

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState ApplicationState
		{
			get;
			set;
		}

		[Inject]
		public IFriendsManager FriendsManager
		{
			get;
			set;
		}

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher
		{
			get;
			set;
		}

		[Inject]
		public XmppManager TheXmppManager
		{
			get;
			set;
		}

		[Inject]
		public IDirectoryServiceClient DirectoryServiceClient
		{
			get;
			set;
		}

		public bool OnXmppManagerConnectFailed
		{
			get;
			private set;
		}

		[PostConstruct]
		public void PostConstruct()
		{
			FriendsManager.OnRefreshFriends += FriendsManager_OnRefreshFriends;
			if (root == null)
			{
				root = contextView.GetComponent<SoundStudioRoot>();
			}
			if (root == null)
			{
				throw new InvalidOperationException("The root was not set!");
			}
			dispatcher.AddListener(SoundStudioEvent.APPLICATION_QUIT, OnApplicationQuit);
		}

		private void OnApplicationQuit(IEvent payload)
		{
			Close();
		}

		public void Close()
		{
			TheXmppManager.client.Close();
			TheXmppManager.client.OnError -= Client_OnError;
			TheXmppManager.Clear();
			TheXmppManager.Disconnect();
			XmppManager.Instance = null;
		}

		private void OnTimerTick()
		{
			if (refreshComplete)
			{
				timer.Stop();
				UpdateModelWithFriends();
				dispatcher.Dispatch(FriendsEvent.FRIENDS_REFRESH_SUCCESS);
			}
		}

		public void Initialize(PlayerAccountVO playerAccountVO)
		{
			XmppManager.Instance = new XmppManager();
			refreshComplete = false;
			if (TheXmppManager.Connected)
			{
				Close();
			}
			playerAccountVO.FriendCollectionStatus = FriendCollectionStatus.UNINITIALIZED;
			playerAccountVO.FriendsTrackCollection.Clear();
			TheXmppManager.client.OnError += Client_OnError;
			TheXmppManager.Connect(playerAccountVO.Swid, playerAccountVO.AuthToken);
			FriendsManager.LoadFriendsCache();
			try
			{
				timer = new Timer(2f, true, OnTimerTick);
				root.StartCoroutine(timer.Start());
			}
			catch (Exception)
			{
			}
		}

		private void Client_OnError(object sender, ExceptionEventArgs e)
		{
			refreshComplete = true;
			UnityEngine.Debug.LogWarning("Error loading friends " + e.ToString());
		}

		private void FriendsManager_OnRefreshFriends()
		{
			refreshComplete = true;
		}

		public void UpdateModelWithFriends()
		{
			if (refreshComplete)
			{
				ApplicationState.currentPlayer.FriendCollectionStatus = FriendCollectionStatus.INITIALIZED;
				foreach (Friend friend in FriendsManager.Friends)
				{
					ApplicationState.currentPlayer.FriendsTrackCollection.AddOrUpdateFriend(friend);
				}
			}
		}
	}
}
