using Matrix.Xml;

namespace Disney.Xmpp
{
	public class Version : XmppXElement
	{
		public Version()
			: base("disney:social:version", "version")
		{
			SetValue(XmppConsts.SAFE_CHAT_VERSION);
		}
	}
}
