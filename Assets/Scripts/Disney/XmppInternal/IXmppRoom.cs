using System.Collections.Generic;

namespace Disney.XmppInternal
{
	public interface IXmppRoom
	{
		void AddRoom(string aRoom, List<string> aMembers);

		void EnableInviting();

		void InviteRoomMembers(string room);

		bool CheckRoom(string aRoom);

		List<string> RoomMembers(string aRoom);
	}
}
