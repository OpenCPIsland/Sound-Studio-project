using Matrix.Xml;

namespace Disney.Xmpp
{
	public class TrustQuery : XmppXElement
	{
		public TrustQuery()
			: base("disney:relationships", "query")
		{
		}
	}
}
