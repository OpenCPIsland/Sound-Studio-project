using Disney.Xmpp;
using Matrix;
using Matrix.Xml;
using Matrix.Xmpp.Client;
using System;

namespace Disney.XmppInternal
{
	public interface IXmppManagerInternal
	{
		string GetMySwid();

		void RequestInstantRoom(string aRoom);

		void EnterRoom(string aRoom);

		void RoomInvite(string aMember, string aRoom);

		Presence RoomExit(string aRoom);

		void RemoveRosterItem(string aSwid);

		void Subscribe(Jid aJid);

		void ApproveSubscriptionRequest(Jid aJid);

		void DenySubscriptionRequest(Jid aJid);

		void Send(XmppXElement aElement);

		void SendIq(Iq aIq, EventHandler<IqEventArgs> aCallBack);

		void SendOnContactAdded(string aSwid, string aName);

		void SendOnContactRemoved(string aSwid);

		void SendOnContactListStart();

		void SendOnContactListComplete();

		void SendOnMyPresence();

		void SendOnOthersPresence(string aSwid, string status, Land land);

		void SendOnFound(string aSwid);

		void SendOnInvitation(string aName);

		void SendOnTrustReceived(string[] aSwids);

		void SendOnChatMessageReceived(string aSwid, DateTime aSent, string aMessage);

		void SendOnGroupChatMessageReceived(string aFrom, DateTime aSent, string aMessage);

		void SendOnStickerMessageReceived(string aFrom, DateTime aSent, string aName);

		void SendOnGroupStickerMessageReceived(string aRoom, DateTime aSent, string aName);

		void SendOnGogChallengeReceived(string aFrom, DateTime aSent, string aBody);

		void SendOnGogResultsReceived(string aFrom, DateTime aSent, string aBody);

		void SendOnGagReceived(string aFrom, DateTime aSent, string aBody);

		void SendOnUgcReceived(string aFrom, DateTime aSent, string aBody);

		void SendOnRoomCreated(string aRoom);

		void SendOnRoomReceived(string aRoom);

		void SendOnRoomMember(string aRoom, string aSwid);

		void SendOnRoomMembers(string aRoom, string aSwids);

		void SendOnRoomMemberLeave(string aRoom, string aSwid);
	}
}
