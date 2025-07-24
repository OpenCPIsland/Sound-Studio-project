using Disney.ClubPenguin.Service.DirectoryService;
using Disney.HTTP.Client;
using Disney.Utility;
using Disney.XmppInternal;
using Matrix;
using Matrix.License;
using Matrix.Xml;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Roster;
using Matrix.Xmpp.Sasl;
using strange.extensions.injector.api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Disney.Xmpp
{
	[Implements(InjectionBindingScope.CROSS_CONTEXT)]
	public class XmppManager : IXmppManagerInternal
	{
		public delegate void OnContactAddedEvent(string swid, string name);

		public delegate void OnContactRemovedEvent(string swid);

		public delegate void OnContactListStartEvent();

		public delegate void OnContactListCompleteEvent();

		public delegate void OnMyPresenceEvent();

		public delegate void OnOthersPresenceEvent(string swid, string status, Land land);

		public delegate void OnFoundEvent(string swid);

		public delegate void OnInvitationEvent(string name);

		public delegate void OnTrustReceivedEvent(string[] swids);

		public delegate void OnChatMessageReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnGroupChatMessageReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnStickerMessageReceivedEvent(string from, DateTime sent, string name);

		public delegate void OnGroupStickerMessageReceivedEvent(string from, DateTime sent, string name);

		public delegate void OnGogChallengeReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnGogResultsReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnGagReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnUgcReceivedEvent(string from, DateTime sent, string message);

		public delegate void OnRoomCreatedEvent(string room);

		public delegate void OnRoomReceivedEvent(string room);

		public delegate void OnRoomMemberEvent(string room, string swid);

		public delegate void OnRoomMembersEvent(string room, string swids);

		public delegate void OnRoomMemberLeaveEvent(string room, string swid);

		private static XmppManager instance = new XmppManager();

		private IDirectoryServiceClient _directoryServiceClient;

		public XmppClient client;

		public bool Connecting;

		public bool Connected;

		public bool ConnectFailed;

		private string mySwid;

		private PresenceManager presenceManager;

		private RosterManager rosterManager;

		private MucManager mucManager;

		private bool closing;

		private XmppMessage xmppMessage;

		private XmppRoom xmppRoom;

		private XmppRoster xmppRoster;

		private XmppPresence xmppPresence;

		public static XmppManager Instance
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
				UnityEngine.Debug.Log("XmppManager.directoryServiceClient - set to " + _directoryServiceClient.GetType().Name + "(" + _directoryServiceClient.GetHashCode() + ")");
			}
		}

		public event OnContactAddedEvent OnContactAdded = delegate
		{
		};

		public event OnContactRemovedEvent OnContactRemoved = delegate
		{
		};

		public event OnContactListStartEvent OnContactListStart = delegate
		{
		};

		public event OnContactListCompleteEvent OnContactListComplete = delegate
		{
		};

		public event OnMyPresenceEvent OnMyPresence = delegate
		{
		};

		public event OnOthersPresenceEvent OnOthersPresence = delegate
		{
		};

		public event OnFoundEvent OnFound = delegate
		{
		};

		public event OnInvitationEvent OnInvitation = delegate
		{
		};

		public event OnTrustReceivedEvent OnTrustReceived = delegate
		{
		};

		public event OnChatMessageReceivedEvent OnChatMessageReceived = delegate
		{
		};

		public event OnGroupChatMessageReceivedEvent OnGroupChatMessageReceived = delegate
		{
		};

		public event OnStickerMessageReceivedEvent OnStickerMessageReceived = delegate
		{
		};

		public event OnGroupStickerMessageReceivedEvent OnGroupStickerMessageReceived = delegate
		{
		};

		public event OnGogChallengeReceivedEvent OnGogChallengeReceived = delegate
		{
		};

		public event OnGogResultsReceivedEvent OnGogResultsReceived = delegate
		{
		};

		public event OnGagReceivedEvent OnGagReceived = delegate
		{
		};

		public event OnUgcReceivedEvent OnUgcReceived = delegate
		{
		};

		public event OnRoomCreatedEvent OnRoomCreated = delegate
		{
		};

		public event OnRoomReceivedEvent OnRoomReceived = delegate
		{
		};

		public event OnRoomMemberEvent OnRoomMember = delegate
		{
		};

		public event OnRoomMembersEvent OnRoomMembers = delegate
		{
		};

		public event OnRoomMemberLeaveEvent OnRoomMemberLeave = delegate
		{
		};

		public XmppManager()
		{
			xmppRoom = new XmppRoom(this);
			xmppRoster = new XmppRoster(this);
			xmppPresence = new XmppPresence(this, xmppRoster, xmppRoom);
			xmppMessage = new XmppMessage(this, xmppRoom);
			closing = false;
			LicenseManager.SetLicense(XmppConsts.MATRIX_LICENCE);
			client = new XmppClient();
			presenceManager = new PresenceManager(client);
			rosterManager = new RosterManager(client);
			mucManager = new MucManager(client);
			client.StartTls = false;
			client.Username = string.Empty;
			client.Password = string.Empty;
			client.XmppDomain = XmppConsts.LOGIN_DOMAIN;
			client.OnBeforeSasl += onBeforeSasl;
			client.OnMessage += onMessage;
			client.OnSendXml += onSend;
			client.OnReceiveXml += onReceive;
			client.OnLogin += onLogin;
			client.OnClose += onClose;
			client.OnRosterStart += onRosterStart;
			client.OnRosterItem += onRosterItem;
			client.OnRosterEnd += onRosterEnd;
			presenceManager.OnSubscribe += onSubscribe;
			presenceManager.OnAvailablePresence += onAvailablePresence;
			presenceManager.OnUnavailablePresence += onUnavailablePresence;
			client.OnError += onError;
			client.OnBindError += onBindError;
			client.OnAuthError += onAuthError;
			Factory.RegisterElement<TrustQuery>("disney:relationships", "query");
			Factory.RegisterElement<Criteria>("criteria");
			Factory.RegisterElement<Item>("item");
			Factory.RegisterElement<FriendsQuery>("disney:friends:search", "query");
			Factory.RegisterElement<AnimationSticker>("disney:friends:sticker", "meta");
			Factory.RegisterElement<AnimationStickerMetaMessage>("disney:social:metamessage", "meta");
			Factory.RegisterElement<Land>("disney:land:clubpenguin", "land");
		}

		public void OnApplicationQuitEvents()
		{
			Disconnect();
		}

		public bool InvitationSent(string aSwid)
		{
			return xmppRoster.CheckSentInvitation(aSwid);
		}

		public void Connect(string aUser, string aPass)
		{
			ConnectFailed = false;
			Util.Log("XmppManager Connect " + aUser + ", " + aPass);
			mySwid = aUser;
			Connecting = true;
			closing = false;
			client.Username = aUser;
			client.Password = aPass;
			directoryServiceClient.GetServiceURL("friends-tigase-server", delegate(string serviceUrl)
			{
				string[] array = serviceUrl.Split(':');
				client.Hostname = array[0];
				client.Port = Convert.ToInt32(array[1]);
				client.OldStyleSsl = (client.Port == 5223);
				client.Open();
			}, delegate(IHTTPResponse errorResponse)
			{
				Util.LogError("Failed to get xmpp server url from Directory Services. Error: " + errorResponse.StatusMessage);
			});
		}

		public void Disconnect()
		{
			Util.Log("XmppManager Disconnect ");
			if (client != null && !closing)
			{
				Util.Log("Close");
				closing = true;
				client.SendUnavailablePresence();
				client.Close();
			}
		}

		public void CreateMucRoom(string aRoom, List<string> aMembers)
		{
			xmppRoom.CreateMucRoom(aRoom, aMembers);
		}

		public void JoinMucRoom(string aRoom)
		{
			xmppRoom.JoinMucRoom(aRoom);
		}

		public void LeaveMucRoom(string aRoom)
		{
			xmppRoom.LeaveMucRoom(aRoom);
		}

		public void SendMessage(string aUser, string aMessage)
		{
			xmppMessage.SendMessage(aUser, aMessage);
		}

		public void SendGroupMessage(string aRoom, string aMessage)
		{
			xmppMessage.SendGroupMessage(aRoom, aMessage);
		}

		public void SendSticker(string aUser, string aName)
		{
			xmppMessage.SendSticker(aUser, aName);
		}

		public void SendGroupSticker(string aRoom, string aName)
		{
			xmppMessage.SendGroupSticker(aRoom, aName);
		}

		public void SendGogChallenge(string aUser, string aMessage)
		{
			xmppMessage.SendGogChallenge(aUser, aMessage);
		}

		public void SendGogResults(string aUser, string aMessage)
		{
			xmppMessage.SendGogResults(aUser, aMessage);
		}

		public void SendGag(string aUser, string aMessage)
		{
			xmppMessage.SendGag(aUser, aMessage);
		}

		public void SendUgc(string aUser, string aMessage)
		{
			xmppMessage.SendUgc(aUser, aMessage);
		}

		public void Find(string aName)
		{
			xmppRoster.Find(aName);
		}

		public void Clear()
		{
			xmppRoster.Clear();
		}

		public void Invite(string aSwid)
		{
			Jid to = aSwid + XmppConsts.DOMAIN;
			presenceManager.Subscribe(to);
		}

		public void AcceptInvitation(string aName)
		{
			xmppRoster.AcceptInvitation(aName);
		}

		public void DeclineInvitation(string aName)
		{
			xmppRoster.DeclineInvitation(aName);
		}

		public void RemoveContact(string aSwid)
		{
			xmppRoster.RemoveContact(aSwid);
		}

		public string GetMySwid()
		{
			return mySwid;
		}

		public void EnterRoom(string aRoom)
		{
			mucManager.EnterRoom(aRoom + XmppConsts.MUC_DOMAIN, mySwid);
		}

		public void RequestInstantRoom(string aRoom)
		{
			mucManager.RequestInstantRoom(aRoom + XmppConsts.MUC_DOMAIN);
		}

		public void RoomInvite(string aMember, string aRoom)
		{
			mucManager.Invite(aMember + XmppConsts.DOMAIN, aRoom + XmppConsts.MUC_DOMAIN);
		}

		public Presence RoomExit(string aRoom)
		{
			return mucManager.CreateExitRoomStanza(aRoom + XmppConsts.MUC_DOMAIN, mySwid);
		}

		public void RemoveRosterItem(string aSwid)
		{
			Jid jid = aSwid + XmppConsts.DOMAIN;
			rosterManager.Remove(jid);
		}

		public void Subscribe(Jid aJid)
		{
			presenceManager.Subscribe(aJid);
		}

		public void ApproveSubscriptionRequest(Jid aJid)
		{
			presenceManager.ApproveSubscriptionRequest(aJid);
		}

		public void DenySubscriptionRequest(Jid aJid)
		{
			presenceManager.DenySubscriptionRequest(aJid);
		}

		public void Send(XmppXElement aElement)
		{
			if (client != null)
			{
				client.Send(aElement);
			}
		}

		public void SendIq(Iq aIq, EventHandler<IqEventArgs> aCallBack)
		{
			client.IqFilter.SendIq(aIq, aCallBack);
		}

		public void SendOnContactAdded(string aSwid, string aName)
		{
			this.OnContactAdded(aSwid, aName);
		}

		public void SendOnContactRemoved(string aSwid)
		{
			this.OnContactRemoved(aSwid);
		}

		public void SendOnContactListStart()
		{
			this.OnContactListStart();
		}

		public void SendOnContactListComplete()
		{
			this.OnContactListComplete();
		}

		public void SendOnMyPresence()
		{
			this.OnMyPresence();
		}

		public void SendOnOthersPresence(string aSwid, string status, Land land)
		{
			Util.Log("XmppManager.SendOnOthersPresence: swid=" + aSwid + ", status=" + status + ", land=" + land);
			this.OnOthersPresence(aSwid, status, land);
		}

		public void SendOnFound(string aSwid)
		{
			this.OnFound(aSwid);
		}

		public void SendOnInvitation(string aName)
		{
			this.OnInvitation(aName);
		}

		public void SendOnTrustReceived(string[] aSwids)
		{
			this.OnTrustReceived(aSwids);
		}

		public void SendOnChatMessageReceived(string aSwid, DateTime aSent, string aMessage)
		{
			this.OnChatMessageReceived(aSwid, aSent, aMessage);
		}

		public void SendOnGroupChatMessageReceived(string aFrom, DateTime aSent, string aMessage)
		{
			this.OnGroupChatMessageReceived(aFrom, aSent, aMessage);
		}

		public void SendOnStickerMessageReceived(string aFrom, DateTime aSent, string aName)
		{
			this.OnStickerMessageReceived(aFrom, aSent, aName);
		}

		public void SendOnGroupStickerMessageReceived(string aRoom, DateTime aSent, string aName)
		{
			this.OnGroupStickerMessageReceived(aRoom, aSent, aName);
		}

		public void SendOnGogChallengeReceived(string aFrom, DateTime aSent, string aBody)
		{
			this.OnGogChallengeReceived(aFrom, aSent, aBody);
		}

		public void SendOnGogResultsReceived(string aFrom, DateTime aSent, string aBody)
		{
			this.OnGogResultsReceived(aFrom, aSent, aBody);
		}

		public void SendOnGagReceived(string aFrom, DateTime aSent, string aBody)
		{
			this.OnGagReceived(aFrom, aSent, aBody);
		}

		public void SendOnUgcReceived(string aFrom, DateTime aSent, string aBody)
		{
			this.OnUgcReceived(aFrom, aSent, aBody);
		}

		public void SendOnRoomCreated(string aRoom)
		{
			this.OnRoomCreated(aRoom);
		}

		public void SendOnRoomReceived(string aRoom)
		{
			this.OnRoomReceived(aRoom);
		}

		public void SendOnRoomMember(string aRoom, string aSwid)
		{
			this.OnRoomMember(aRoom, aSwid);
		}

		public void SendOnRoomMembers(string room, string aSwids)
		{
			this.OnRoomMembers(room, aSwids);
		}

		public void SendOnRoomMemberLeave(string room, string aSwid)
		{
			this.OnRoomMemberLeave(room, aSwid);
		}

		private void onLogin(object sender, Matrix.EventArgs e)
		{
			UnityEngine.Debug.Log("XmppManager onLogin");
			Connecting = false;
			Connected = true;
		}

		private void onClose(object sender, Matrix.EventArgs e)
		{
			Connected = false;
			if (Connecting)
			{
				ConnectFailed = true;
			}
		}

		private void onSend(object sender, TextEventArgs e)
		{
			UnityEngine.Debug.Log("XmppManager onSend " + e.Text);
			Util.Log("XmppManager onSend " + e.Text);
		}

		private void onReceive(object sender, TextEventArgs e)
		{
			Util.Log("XmppManager onReceive " + e.Text);
		}

		private void onInvite(object sender, MessageEventArgs e)
		{
			Util.Log("XmppManager On Invite: " + e.Message);
		}

		private void onMessage(object sender, MessageEventArgs e)
		{
			xmppMessage.ParseMessage(e.Message);
		}

		private void onBeforeSasl(object sender, SaslEventArgs e)
		{
			e.Auto = false;
			e.SaslMechanism = SaslMechanism.PLAIN;
		}

		private void onRosterItem(object sender, RosterEventArgs e)
		{
			xmppRoster.ParseRosterItem(e.RosterItem);
		}

		private void RosterAdd(string aSwid, RosterItem aItem)
		{
			xmppRoster.RosterAdd(aSwid, aItem);
		}

		private void onRosterStart(object sender, Matrix.EventArgs e)
		{
			xmppRoster.RosterStart();
		}

		private void onRosterEnd(object sender, Matrix.EventArgs e)
		{
			xmppRoster.RosterEnd();
		}

		private void onAvailablePresence(object sender, PresenceEventArgs e)
		{
			if (e.Presence.Status == null || e.Presence.Status.Length == 0)
			{
				e.Presence.Status = "available";
			}
			xmppPresence.ParsePresence(e.Presence);
		}

		private void onUnavailablePresence(object sender, PresenceEventArgs e)
		{
			if (e.Presence.Status == null || e.Presence.Status.Length == 0)
			{
				e.Presence.Status = "unavailable";
			}
			xmppPresence.ParsePresence(e.Presence);
		}

		private void onSubscribe(object sender, PresenceEventArgs e)
		{
			xmppPresence.Subscribe(e.Presence);
		}

		private void onError(object sender, ExceptionEventArgs e)
		{
			UnityEngine.Debug.Log("XmppManager.onError: " + e.Exception.Message + "\n" + e.Exception.StackTrace);
		}

		private void onBindError(object sender, IqEventArgs args)
		{
			UnityEngine.Debug.Log("XmppManager.onBindError: " + args);
		}

		private void onAuthError(object sender, SaslEventArgs args)
		{
			UnityEngine.Debug.Log("XmppManager.onAuthError: " + args);
		}
	}
}
