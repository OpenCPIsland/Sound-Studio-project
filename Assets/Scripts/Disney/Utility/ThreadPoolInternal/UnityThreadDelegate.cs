namespace Disney.Utility.ThreadPoolInternal
{
	public class UnityThreadDelegate : ThreadPoolManager.IUnityThreadInterface
	{
		protected ThreadPoolManager.IUnityThreadPoolInterface mCaller;

		protected object mObject;

		private bool m_IsDone;

		private object m_Handle = new object();

		public bool IsDone
		{
			get
			{
				lock (m_Handle)
				{
					return m_IsDone;
				}
			}
			set
			{
				lock (m_Handle)
				{
					m_IsDone = value;
				}
			}
		}

		public UnityThreadDelegate(ThreadPoolManager.IUnityThreadPoolInterface aCaller, object aObject)
		{
			mCaller = aCaller;
			mObject = aObject;
		}

		public void Run(object obj)
		{
			mCaller.ThreadedMethod(mObject);
			IsDone = true;
		}

		public virtual bool Update()
		{
			if (IsDone)
			{
				return true;
			}
			return false;
		}

		public void CompleteThread()
		{
			mCaller.ThreadComplete(mObject);
		}
	}
}
