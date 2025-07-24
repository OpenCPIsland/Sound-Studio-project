using Matrix.Xml;

namespace Disney.Xmpp
{
	public class Location : XmppXElement
	{
		public string Room
		{
			get
			{
				return GetAttribute("room");
			}
			set
			{
				SetAttribute("room", value);
			}
		}

		public Location()
			: base("disney:social:location", "location")
		{
			SetValue("spk");
		}
	}
}
