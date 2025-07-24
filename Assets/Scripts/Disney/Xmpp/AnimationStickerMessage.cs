using Matrix.Xmpp;
using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class AnimationStickerMessage : Message
	{
		public AnimationStickerMetaMessage StickerMetaMessage
		{
			get
			{
				return Element<AnimationStickerMetaMessage>();
			}
			set
			{
				Replace(value);
			}
		}

		public AnimationStickerMessage()
		{
			GenerateId();
			base.Type = MessageType.normal;
		}
	}
}
