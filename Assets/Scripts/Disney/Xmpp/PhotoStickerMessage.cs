using Matrix.Xmpp;
using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class PhotoStickerMessage : Message
	{
		public PhotoStickerMetaMessage MetaMessage
		{
			get
			{
				return Element<PhotoStickerMetaMessage>();
			}
			set
			{
				Replace(value);
			}
		}

		public PhotoStickerMessage()
		{
			GenerateId();
			base.Type = MessageType.normal;
		}
	}
}
