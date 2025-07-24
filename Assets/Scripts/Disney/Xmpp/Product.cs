using Matrix.Xml;

namespace Disney.Xmpp
{
	public class Product : XmppXElement
	{
		public Product()
			: base("disney:social:product", "product")
		{
			SetValue("spk");
		}
	}
}
