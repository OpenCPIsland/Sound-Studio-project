using Disney.Utility;

namespace Disney.Xmpp
{
	public class XmppDisconnectedWorker : ThreadPoolManager.IUnityThreadPoolInterface
	{
		protected IXmppDisconnectedWorker caller;

		private bool lastXmppManagerConnectedState = true;

		public XmppDisconnectedWorker(IXmppDisconnectedWorker aCaller)
		{
			caller = aCaller;
			ThreadPoolManager.Instance.addToPool(this, null);
		}

		public void ThreadedMethod(object aObject)
		{
			while (lastXmppManagerConnectedState)
			{
				if (lastXmppManagerConnectedState && !XmppManager.Instance.Connected)
				{
					lastXmppManagerConnectedState = XmppManager.Instance.Connected;
				}
			}
		}

		public void ThreadComplete(object aObject)
		{
			caller.OnXmppDisconnected();
			caller = null;
		}
	}
}
