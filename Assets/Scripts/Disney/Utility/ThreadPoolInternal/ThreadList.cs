using System.Collections;
using System.Threading;

namespace Disney.Utility.ThreadPoolInternal
{
	public class ThreadList
	{
		protected ArrayList mThreads = new ArrayList();

		public void addToList(ThreadPoolManager.IUnityThreadInterface thread)
		{
			mThreads.Add(thread);
			ThreadPool.QueueUserWorkItem(thread.Run);
		}

		public void RunUpdates()
		{
			for (int i = 0; i < mThreads.Count; i++)
			{
				ThreadPoolManager.IUnityThreadInterface unityThreadInterface = (ThreadPoolManager.IUnityThreadInterface)mThreads[i];
				if (unityThreadInterface.Update())
				{
					unityThreadInterface.CompleteThread();
					unityThreadInterface = null;
					mThreads[i] = null;
					mThreads.RemoveAt(i);
				}
			}
		}

		public int GetActiveThreadCount()
		{
			return mThreads.Count;
		}
	}
}
