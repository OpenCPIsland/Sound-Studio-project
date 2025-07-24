using Disney.Xmpp;
using Matrix.Xmpp;
using Matrix.Xmpp.Client;
using System.Collections.Generic;

namespace Disney.XmppInternal
{
	public class XmppRoom : IXmppRoom
	{
		private IXmppManagerInternal manager;

		private Dictionary<string, MucRoom> rooms;

		private bool inviteEnabled;

		public XmppRoom(IXmppManagerInternal aManager)
		{
			manager = aManager;
			rooms = new Dictionary<string, MucRoom>();
			inviteEnabled = false;
		}

		public bool CreateMucRoom(string aRoom, List<string> aMembers)
		{
			if (CheckRoom(aRoom))
			{
				return false;
			}
			AddRoom(aRoom, aMembers);
			manager.EnterRoom(aRoom);
			return true;
		}

		public void JoinMucRoom(string aRoom)
		{
			manager.EnterRoom(aRoom);
		}

		public void LeaveMucRoom(string aRoom)
		{
			CustomGroupChatMetaMessage customGroupChatMetaMessage = new CustomGroupChatMetaMessage();
			customGroupChatMetaMessage.Type = MessageType.groupchat;
			customGroupChatMetaMessage.To = aRoom + XmppConsts.MUC_DOMAIN;
			customGroupChatMetaMessage.From = manager.GetMySwid() + XmppConsts.DOMAIN + XmppConsts.RESOURCE;
			customGroupChatMetaMessage.Product = new Product();
			customGroupChatMetaMessage.Location = new Location
			{
				Room = aRoom + XmppConsts.MUC_DOMAIN
			};
			customGroupChatMetaMessage.MetaMessage = new MetaMessage
			{
				Type = "disney:spark:muc:leave"
			};
			CustomGroupChatMetaMessage customGroupChatMetaMessage2 = customGroupChatMetaMessage;
			customGroupChatMetaMessage2.Body = null;
			customGroupChatMetaMessage2.XmlLanguage = "en";
			manager.Send(customGroupChatMetaMessage2);
			Presence presence = manager.RoomExit(aRoom);
			presence.SetAttribute("from", manager.GetMySwid() + XmppConsts.DOMAIN + XmppConsts.RESOURCE);
			presence.AddTag("status", "exit");
			manager.Send(presence);
		}

		public void EnableInviting()
		{
			inviteEnabled = true;
		}

		public void InviteRoomMembers(string aRoom)
		{
			if (inviteEnabled && CheckRoom(aRoom))
			{
				string mySwid = manager.GetMySwid();
				foreach (string item in RoomMembers(aRoom))
				{
					if (item != mySwid)
					{
						manager.RoomInvite(item, aRoom);
					}
				}
			}
			inviteEnabled = false;
		}

		public void AddRoom(string aRoom, List<string> aMembers)
		{
			rooms.Add(aRoom, new MucRoom(aRoom, aMembers));
		}

		public bool CheckRoom(string aRoom)
		{
			return rooms.ContainsKey(aRoom);
		}

		public List<string> RoomMembers(string aRoom)
		{
			return rooms[aRoom].members;
		}
	}
}
