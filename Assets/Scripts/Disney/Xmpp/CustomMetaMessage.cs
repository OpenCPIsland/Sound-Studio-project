using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class CustomMetaMessage : Message
	{
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

		public CustomMetaMessage()
		{
			GenerateId();
		}
	}
}
