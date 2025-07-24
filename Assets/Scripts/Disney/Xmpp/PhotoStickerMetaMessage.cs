using Matrix.Xml;

namespace Disney.Xmpp
{
	public class PhotoStickerMetaMessage : XmppXElement
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

		public PhotoSticker Sticker
		{
			get
			{
				return Element<PhotoSticker>();
			}
			set
			{
				Replace(value);
			}
		}

		public PhotoStickerMetaMessage()
			: base("disney:social:metamessage", "meta")
		{
		}
	}
}
