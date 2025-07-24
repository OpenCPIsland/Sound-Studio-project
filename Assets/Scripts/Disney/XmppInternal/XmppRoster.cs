using Disney.Utility;
using Disney.Xmpp;
using Matrix;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Roster;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Disney.XmppInternal
{
	public class XmppRoster : IXmppRoster
	{
		private IXmppManagerInternal manager;

		private Dictionary<string, RosterItem> roster;

		private Dictionary<string, Jid> invitations;

		private HashSet<string> sentInvitations;

		public XmppRoster(IXmppManagerInternal aManager)
		{
			manager = aManager;
			roster = new Dictionary<string, RosterItem>();
			invitations = new Dictionary<string, Jid>();
			sentInvitations = new HashSet<string>();
		}

		public void Clear()
		{
			invitations.Clear();
			roster.Clear();
		}

		public void AddInvitation(string aSwid, Jid aJid)
		{
			invitations.Add(aSwid, aJid);
		}

		public void Find(string aName)
		{
			FriendsSearch friendsSearch = new FriendsSearch();
			friendsSearch.Type = IqType.get;
			friendsSearch.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			friendsSearch.FriendsQuery = new FriendsQuery
			{
				Criteria = new Criteria
				{
					Field = "name",
					Val = aName
				}
			};
			FriendsSearch aIq = friendsSearch;
			manager.SendIq(aIq, friendsSearchResponse);
		}

		public void GetTrust()
		{
			Util.Log("XmppRoster GetTrust ");
			RequestTrust requestTrust = new RequestTrust();
			requestTrust.Type = IqType.get;
			requestTrust.From = manager.GetMySwid() + XmppConsts.DOMAIN;
			requestTrust.TrustQuery = new TrustQuery();
			RequestTrust aIq = requestTrust;
			manager.SendIq(aIq, GetTrustResponse);
		}

		public void Invite(string aSwid)
		{
			Jid aJid = aSwid + XmppConsts.DOMAIN;
			manager.Subscribe(aJid);
		}

		public bool CheckInvitation(string aName)
		{
			return invitations.ContainsKey(aName);
		}

		public void AcceptInvitation(string aName)
		{
			if (CheckInvitation(aName))
			{
				manager.ApproveSubscriptionRequest(invitations[aName]);
				invitations.Remove(aName);
			}
		}

		public void DeclineInvitation(string aName)
		{
			if (CheckInvitation(aName))
			{
				manager.DenySubscriptionRequest(invitations[aName]);
				invitations.Remove(aName);
			}
		}

		public void RemoveContact(string aSwid)
		{
			if (roster.ContainsKey(aSwid))
			{
				roster.Remove(aSwid);
				manager.RemoveRosterItem(aSwid);
			}
		}

		public void ParseRosterItem(RosterItem aItem)
		{
			string user = aItem.Jid.User;
			if (aItem.Subscription == Subscription.none && aItem.Ask == Ask.subscribe)
			{
				AddSentInvitation(user);
			}
			else if (roster.ContainsKey(user))
			{
				if (roster[user].Subscription == Subscription.both)
				{
					roster[user].Subscription = aItem.Subscription;
					if (aItem.Subscription != Subscription.both)
					{
						manager.SendOnContactRemoved(user);
					}
				}
				else
				{
					RosterAdd(user, aItem);
				}
			}
			else if (aItem.Subscription == Subscription.remove)
			{
				manager.SendOnContactRemoved(user);
			}
			else
			{
				RosterAdd(user, aItem);
			}
		}

		public void RosterAdd(string aSwid, RosterItem aItem)
		{
			if (roster.ContainsKey(aSwid))
			{
				roster.Remove(aSwid);
			}
			roster.Add(aSwid, aItem);
			if (aItem.Subscription == Subscription.both)
			{
				manager.SendOnContactAdded(aSwid, aItem.Name);
				if (CheckSentInvitation(aSwid))
				{
					RemoveSentInvitation(aSwid);
					GetTrust();
				}
			}
		}

		public void RosterStart()
		{
			manager.SendOnContactListStart();
		}

		public void RosterEnd()
		{
			manager.SendOnContactListComplete();
			GetTrust();
		}

		private void GetTrustResponse(object sender, IqEventArgs e)
		{
			if (e.Iq.Type == IqType.result)
			{
				Iq iq = e.Iq;
				List<string> list = new List<string>();
				IEnumerable<XElement> enumerable = iq.Descendants();
				foreach (XElement item in enumerable)
				{
					if (item.Name.ToString() == "{disney:relationships}relationship")
					{
						XAttribute xAttribute = item.Attribute("id");
						XAttribute xAttribute2 = item.Attribute("type");
						XAttribute xAttribute3 = item.Attribute("state");
						if (xAttribute != null && xAttribute2 != null && xAttribute3 != null && xAttribute2.Value == "TRUSTED" && xAttribute3.Value == "OK")
						{
							list.Add(xAttribute.Value);
						}
					}
				}
				manager.SendOnTrustReceived(list.ToArray());
			}
		}

		private void friendsSearchResponse(object sender, IqEventArgs e)
		{
			if (e.Iq.Type == IqType.result)
			{
				Iq iq = e.Iq;
				IEnumerable<XElement> enumerable = iq.Descendants();
				foreach (XElement item in enumerable)
				{
					XAttribute xAttribute = item.Attribute("jid");
					if (xAttribute != null)
					{
						manager.SendOnFound(((Jid)xAttribute.Value).User);
						return;
					}
				}
				manager.SendOnFound(string.Empty);
			}
		}

		public void AddSentInvitation(string aSwid)
		{
			sentInvitations.Add(aSwid);
		}

		public bool CheckSentInvitation(string aSwid)
		{
			return sentInvitations.Contains(aSwid);
		}

		public void RemoveSentInvitation(string aSwid)
		{
			sentInvitations.Remove(aSwid);
		}
	}
}
