using Disney.Utility;

namespace Disney.Xmpp
{
	public class XmppConnectedWorker : ThreadPoolManager.IUnityThreadPoolInterface
	{
		protected IXmppConnectedWorker caller;

		private bool lastXmppManagerConnectedState;

		private bool failedConnection;

		public XmppConnectedWorker(IXmppConnectedWorker aCaller)
		{
			caller = aCaller;
			ThreadPoolManager.Instance.addToPool(this, null);
		}

		public void ThreadedMethod(object aObject)
		{
			while (!lastXmppManagerConnectedState)
			{
				if (!lastXmppManagerConnectedState && XmppManager.Instance.Connected)
				{
					lastXmppManagerConnectedState = XmppManager.Instance.Connected;
				}
				if (XmppManager.Instance.ConnectFailed)
				{
					failedConnection = true;
				}
			}
		}

		public void ThreadComplete(object aObject)
		{
			caller.OnXmppConnected(failedConnection);
			caller = null;
		}
	}
}
