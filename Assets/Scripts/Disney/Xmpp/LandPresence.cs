using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class LandPresence : Presence
	{
		public Land Land
		{
			get
			{
				return Element<Land>();
			}
			set
			{
				Replace(value);
			}
		}
	}
}
