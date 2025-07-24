using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class ChatMessage : Message
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

		public Version Version
		{
			get
			{
				return Element<Version>();
			}
			set
			{
				Replace(value);
			}
		}

		public ChatMessage()
		{
			GenerateId();
		}
	}
}
