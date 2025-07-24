using Disney.Utility.ThreadPoolInternal;
using UnityEngine;

namespace Disney.Utility
{
	public class ThreadPoolManager : MonoBehaviour
	{
		public interface IUnityThreadPoolInterface
		{
			void ThreadComplete(object obj);

			void ThreadedMethod(object obj);
		}

		public interface IUnityThreadInterface
		{
			void Run(object obj);

			bool Update();

			void CompleteThread();
		}

		protected ThreadList mThreadList = new ThreadList();

		private static ThreadPoolManager inst;

		public ThreadList ThreadList => mThreadList;

		public static ThreadPoolManager Instance
		{
			get
			{
				if (inst == null)
				{
					GameObject gameObject = new GameObject();
					inst = gameObject.AddComponent<ThreadPoolManager>();
				}
				return inst;
			}
		}

		private void Start()
		{
			Util.Log("Thread pool manager instanced");
		}

		private void Update()
		{
			mThreadList.RunUpdates();
		}

		public void addToPool(IUnityThreadPoolInterface caller, object obj)
		{
			UnityThreadDelegate unityThreadDelegate = new UnityThreadDelegate(caller, obj);
			if (caller != null && unityThreadDelegate != null)
			{
				mThreadList.addToList(unityThreadDelegate);
			}
			else
			{
				caller?.ThreadComplete(null);
			}
		}

		public void AddToPool()
		{
		}
	}
}
