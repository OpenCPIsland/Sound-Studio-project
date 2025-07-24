using System.Collections.Generic;

namespace Disney.Xmpp
{
	public class MucRoom
	{
		public string name;

		public List<string> members;

		public MucRoom(string aName, List<string> aMembers)
		{
			name = aName;
			members = aMembers;
		}
	}
}
