using Matrix.Xml;

namespace Disney.Xmpp
{
	public class Criteria : XmppXElement
	{
		public string Field
		{
			get
			{
				return GetAttribute("field");
			}
			set
			{
				SetAttribute("field", value);
			}
		}

		public string Val
		{
			get
			{
				return GetAttribute("value");
			}
			set
			{
				SetAttribute("value", value);
			}
		}

		public Criteria()
			: base("criteria")
		{
			RemoveAttribute("xmlns");
		}
	}
}
