using Matrix.Xml;

namespace Disney.Xmpp
{
	public class MetaMessage : XmppXElement
	{
		public string Type
		{
			get
			{
				return GetAttribute("type");
			}
			set
			{
				SetAttribute("type", value);
			}
		}

		public MetaMessage()
			: base("disney:social:metamessage", "meta")
		{
		}
	}
}
