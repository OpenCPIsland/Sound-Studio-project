using Matrix.Xml;

namespace Disney.Xmpp
{
	public class PhotoSticker : XmppXElement
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

		public string TypeName
		{
			get
			{
				return GetAttribute("name");
			}
			set
			{
				SetAttribute("name", value);
			}
		}

		public PhotoSticker()
			: base("disney:friends:sticker", "sticker")
		{
		}
	}
}
