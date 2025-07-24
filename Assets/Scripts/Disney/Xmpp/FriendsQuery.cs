using Matrix.Xml;

namespace Disney.Xmpp
{
	public class FriendsQuery : XmppXElement
	{
		public Criteria Criteria
		{
			get
			{
				return Element<Criteria>();
			}
			set
			{
				Replace(value);
			}
		}

		public Item Item
		{
			get
			{
				return Element<Item>();
			}
			set
			{
				Replace(value);
			}
		}

		public FriendsQuery()
			: base("disney:friends:search", "query")
		{
		}
	}
}
