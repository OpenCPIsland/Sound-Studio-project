using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class CustomGroupChatMetaMessage : Message
	{
		public Product Product
		{
			get
			{
				return Element<Product>();
			}
			set
			{
				Replace(value);
			}
		}

		public Location Location
		{
			get
			{
				return Element<Location>();
			}
			set
			{
				Replace(value);
			}
		}

		public MetaMessage MetaMessage
		{
			get
			{
				return Element<MetaMessage>();
			}
			set
			{
				Replace(value);
			}
		}

		public CustomGroupChatMetaMessage()
		{
			GenerateId();
		}
	}
}
