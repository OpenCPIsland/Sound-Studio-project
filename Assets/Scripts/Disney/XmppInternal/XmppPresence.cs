using Disney.Xmpp;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Muc.User;

namespace Disney.XmppInternal
{
	public class XmppPresence
	{
		private IXmppManagerInternal manager;

		private IXmppRoster xmppRoster;

		private IXmppRoom xmppRoom;

		public XmppPresence(IXmppManagerInternal aManager, IXmppRoster aRoster, IXmppRoom aRoom)
		{
			manager = aManager;
			xmppRoster = aRoster;
			xmppRoom = aRoom;
		}

		public void ParsePresence(Presence aPresence)
		{
			string mySwid = manager.GetMySwid();
			X mucUser = aPresence.MucUser;
			if (mucUser != null)
			{
				string user = aPresence.From.User;
				string resource = aPresence.From.Resource;
				if (resource == mySwid)
				{
					if (xmppRoom.CheckRoom(user))
					{
						if (mucUser.HasStatus(201))
						{
							manager.RequestInstantRoom(user);
							manager.SendOnRoomCreated(user);
						}
						else if (mucUser.HasStatus(110) && mucUser.HasStatus(100))
						{
							xmppRoom.InviteRoomMembers(user);
						}
					}
				}
				else
				{
					Matrix.Xmpp.Muc.User.Item item = mucUser.Item;
					if (item != null)
					{
						manager.SendOnRoomMember(user, resource);
					}
				}
			}
			else if (aPresence.From.User == mySwid)
			{
				manager.SendOnMyPresence();
			}
			else
			{
				manager.SendOnOthersPresence(aPresence.From.User, aPresence.Status, aPresence.Element<Land>());
			}
		}

		public void Subscribe(Presence aPresence)
		{
			xmppRoster.AddInvitation(aPresence.Nick, aPresence.From);
			manager.SendOnInvitation(aPresence.Nick);
		}
	}
}
