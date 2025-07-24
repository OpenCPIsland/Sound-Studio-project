using Matrix.Xmpp.Client;

namespace Disney.Xmpp
{
	public class RequestTrust : Iq
	{
		public TrustQuery TrustQuery
		{
			get
			{
				return Element<TrustQuery>();
			}
			set
			{
				Replace(value);
			}
		}

		public RequestTrust()
		{
			GenerateId();
		}
	}
}
