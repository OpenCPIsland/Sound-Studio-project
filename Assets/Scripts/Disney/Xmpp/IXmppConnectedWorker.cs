namespace Disney.Xmpp
{
	public interface IXmppConnectedWorker
	{
		void OnXmppConnected(bool failedConnection);
	}
}
