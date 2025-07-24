using Matrix.Xml;

namespace Disney.Xmpp
{
	public class AnimationStickerMetaMessage : XmppXElement
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

		public AnimationSticker Sticker
		{
			get
			{
				return Element<AnimationSticker>();
			}
			set
			{
				Replace(value);
			}
		}

		public AnimationStickerMetaMessage()
			: base("disney:social:metamessage", "meta")
		{
		}
	}
}
