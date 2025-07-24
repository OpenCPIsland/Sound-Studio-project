using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class FriendsSearch : Iq
	{
		public FriendsQuery FriendsQuery
		{
			get
			{
				return Element<FriendsQuery>();
			}
			set
			{
				Replace(value);
			}
		}

		public FriendsSearch()
		{
			GenerateId();
		}
	}
}
