using SoundStudio.Event;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SoundStudio.Service
{
	public class AssetBundleDownloadService : IDisposable
	{
		private const float TIMER_INTERVAL_SECONDS = 1f;

		private MonoBehaviour root;
		private UnityWebRequest request;
		private bool lastBundleSuccess;
		private Timer timer;

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher { get; set; }

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView { get; set; }

		[PostConstruct]
		public void Initialize()
		{
			dispatcher.AddListener(SoundStudioEvent.APPLICATION_QUIT, OnApplicationQuit);
			if (root == null)
			{
				root = contextView.GetComponent<SoundStudioRoot>();
			}
			if (root == null)
			{
				throw new InvalidOperationException("The root was not set!");
			}
		}

		private void OnWatchDogTimerTick()
		{
			try
			{
				if (request != null)
				{
					dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_PROGRESS, request.downloadProgress);
				}
			}
			catch (Exception) { }
		}

		private void OnApplicationQuit(IEvent payload)
		{
			dispatcher.RemoveListener(SoundStudioEvent.APPLICATION_QUIT, OnApplicationQuit);
#if !UNITY_WEBGL
			if (!lastBundleSuccess)
			{
				Caching.ClearCache();
			}
#endif
			Dispose();
		}

		public void DownloadBundle(string bundleURL, int version)
		{
			try
			{
				timer = new Timer(TIMER_INTERVAL_SECONDS, true, () => OnWatchDogTimerTick());
				root.StartCoroutine(timer.Start());
				root.StartCoroutine(DownloadAndCache(bundleURL, version));
			}
			catch (Exception) { }
		}

		private IEnumerator DownloadAndCache(string bundleURL, int version)
		{
			lastBundleSuccess = false;

#if !UNITY_WEBGL
			while (!Caching.ready)
			{
				yield return null;
			}

			if (Caching.IsVersionCached(bundleURL, version))
			{
				dispatcher.Dispatch(SoundStudioEvent.ASSET_BUNDLE_CACHED);
			}
			else
			{
				dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_STARTED);
			}

			request = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL, (uint)version, 0);
#else
			dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_STARTED);
			request = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
#endif

			DateTime startTime = DateTime.Now;
			yield return request.SendWebRequest();

			if (request.result != UnityWebRequest.Result.Success)
			{
				dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_FAILED);
			}
			else
			{
				AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
				if (bundle != null)
				{
					lastBundleSuccess = true;
					dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_SUCCESS, bundle);
				}
				else
				{
					dispatcher.Dispatch(SoundStudioEvent.GET_ASSET_BUNDLE_FAILED);
				}
			}

			DateTime endTime = DateTime.Now;
			timer.Stop();
			request = null;
		}

		public void Dispose()
		{
			if (request != null)
			{
				request.Dispose();
			}
		}
	}
}