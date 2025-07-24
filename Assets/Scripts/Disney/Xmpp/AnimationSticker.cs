using Matrix.Xml;

namespace Disney.Xmpp
{
	public class AnimationSticker : XmppXElement
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

		public AnimationSticker()
			: base("disney:friends:sticker", "sticker")
		{
		}
	}
}
