using Disney.ClubPenguin.Service.DirectoryService;
using Disney.ClubPenguin.Service.FriendsInternal;
using Disney.Database;
using Disney.Utility;
using Disney.Xmpp;
using strange.extensions.injector.api;
using System.Collections;
using System.Collections.Generic;

namespace Disney.ClubPenguin.Service.Friends
{
	[Implements(typeof(IFriendsManager), InjectionBindingScope.CROSS_CONTEXT)]
	public class FriendsManager : IInternalFriendsManager, IFriendsManager
	{
		private static IFriendsManager instance = new FriendsManager();

		private IGameStateManager _gameStateManager;

		private IDirectoryServiceClient _directoryServiceClient;

		private XmppManager xmpp;

		private FriendsDatabaseApi databaseApi;

		private FriendsManagerInvitations friendsManagerInvitations;

		private FriendsManagerFriends friendsManagerFriends;

		public static IFriendsManager Instance
		{
			get
			{
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		[Inject]
		public IGameStateManager gameStateManager
		{
			get
			{
				if (_gameStateManager == null)
				{
					return GameStateManager.Instance;
				}
				return _gameStateManager;
			}
			set
			{
				_gameStateManager = value;
			}
		}

		[Inject]
		public IDirectoryServiceClient directoryServiceClient
		{
			get
			{
				if (_directoryServiceClient == null)
				{
					return DirectoryServiceClient.Instance;
				}
				return _directoryServiceClient;
			}
			set
			{
				_directoryServiceClient = value;
			}
		}

		public IEnumerable Invitations => friendsManagerInvitations.GetInvitations();

		public int InvitationCount => friendsManagerInvitations.Count();

		public IEnumerable Friends => friendsManagerFriends.GetFriends();

		public int FriendCount => friendsManagerFriends.Count();

		public event OnRefreshFriendsEvent OnRefreshFriends = delegate
		{
		};

		public event OnFoundEvent OnFound = delegate
		{
		};

		public FriendsManager()
		{
			Util.Log("FriendsManager constructor");
			xmpp = XmppManager.Instance;
			friendsManagerFriends = new FriendsManagerFriends(this);
			friendsManagerInvitations = new FriendsManagerInvitations(this, friendsManagerFriends);
			databaseApi = new FriendsDatabaseApi(new SqliteDatabase(FriendsDatabaseApi.ConnectionString));
			xmpp.OnFound += onFound;
			xmpp.OnContactAdded += onAddFriend;
			xmpp.OnContactRemoved += onRemoveFriend;
			xmpp.OnContactListStart += onContactListStart;
			xmpp.OnContactListComplete += onContactListComplete;
			xmpp.OnInvitation += onInvitation;
			xmpp.OnTrustReceived += SetTrustedFlags;
			xmpp.OnOthersPresence += HandleOnOthersPresence;
		}

		[PostConstruct]
		public void PostConstruct()
		{
			xmpp.directoryServiceClient = directoryServiceClient;
		}

		public void Init()
		{
			Util.Log("FriendsManager Init");
		}

		public void AcceptInvitation(string aName)
		{
			friendsManagerInvitations.Accept(aName);
		}

		public void DeclineInvitation(string aName)
		{
			friendsManagerInvitations.Decline(aName);
		}

		public void FindUser(string aName)
		{
			friendsManagerInvitations.Find(aName);
		}

		public void Invite()
		{
			friendsManagerInvitations.Invite();
		}

		public void Unfriend(string aName)
		{
			friendsManagerFriends.Unfriend(aName);
		}

		public void SetTrustedFlags(string[] aSwids)
		{
			friendsManagerFriends.SetTrustedFlags(aSwids);
			SendOnRefreshFriends();
		}

		public int TrustedFriendCount()
		{
			return friendsManagerFriends.TrustedFriendCount();
		}

		public Friend FindFriendByName(string aName)
		{
			return friendsManagerFriends.FindFriendByName(aName);
		}

		public Friend FindFriendBySwid(string aSwid)
		{
			return friendsManagerFriends.FindFriendBySwid(aSwid);
		}

		public void ClearFriendsCache()
		{
			friendsManagerFriends.Clear();
			friendsManagerInvitations.Clear();
			xmpp.Clear();
		}

		public void LoadFriendsCache()
		{
			loadFriendsFromDisk();
		}

		private void loadFriendsFromDisk()
		{
			Util.Log("FriendsManager, loadFriendsFromDisk called... ");
			string swid = gameStateManager.GameState.swid;
			if (swid != null)
			{
				List<Friend> list = databaseApi.loadFriends(swid);
				foreach (Friend item in list)
				{
					friendsManagerFriends.OnAddFriend(item.Swid, item.Name);
				}
			}
		}

		private void saveFriendsToDisk()
		{
			string swid = gameStateManager.GameState.swid;
			if (swid != null)
			{
				databaseApi.saveFriends(swid);
			}
		}

		public void SendFindUser(string aName)
		{
			xmpp.Find(aName);
		}

		public void SendInvite(string aSwid)
		{
			xmpp.Invite(aSwid);
		}

		public void SendAcceptInvitation(string aName)
		{
			xmpp.AcceptInvitation(aName);
		}

		public void SendDeclineInvitation(string aName)
		{
			xmpp.DeclineInvitation(aName);
		}

		public bool GetInvitationSent(string aSwid)
		{
			return xmpp.InvitationSent(aSwid);
		}

		public void SendRemoveContact(string aSwid)
		{
			xmpp.RemoveContact(aSwid);
		}

		public void SendOnRefreshFriends()
		{
			this.OnRefreshFriends();
		}

		public void SendOnFound(FindResults aResults)
		{
			this.OnFound(aResults);
		}

		public Friend GenerateFriend(string aName, string aSwid)
		{
			return new Friend(aSwid, aName);
		}

		private void onFound(string aSwid)
		{
			friendsManagerInvitations.OnFound(aSwid);
		}

		private void onAddFriend(string aSwid, string aName)
		{
			friendsManagerFriends.OnAddFriend(aSwid, aName);
		}

		private void onRemoveFriend(string aSwid)
		{
			friendsManagerFriends.OnRemoveFriend(aSwid);
		}

		private void onContactListStart()
		{
			friendsManagerFriends.Clear();
			xmpp.Clear();
		}

		private void onContactListComplete()
		{
			saveFriendsToDisk();
		}

		private void onInvitation(string aName)
		{
			friendsManagerInvitations.OnInvitation(aName);
		}

		private void HandleOnOthersPresence(string swid, string status, Land land)
		{
			Friend friend = friendsManagerFriends.FindFriendBySwid(swid);
			if (friend != null)
			{
				friend.Status = status;
				friend.Land = land;
				SendOnRefreshFriends();
			}
		}
	}
}
