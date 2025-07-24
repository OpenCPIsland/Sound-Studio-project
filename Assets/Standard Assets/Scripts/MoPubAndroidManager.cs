using System;
using UnityEngine;

public class MoPubAndroidManager : MonoBehaviour
{
	public static event Action onAdLoadedEvent;

	public static event Action onAdFailedEvent;

	public static event Action onAdClickedEvent;

	public static event Action onAdExpandedEvent;

	public static event Action onAdCollapsedEvent;

	public static event Action onInterstitialLoadedEvent;

	public static event Action onInterstitialFailedEvent;

	public static event Action onInterstitialShownEvent;

	public static event Action onInterstitialClickedEvent;

	public static event Action onInterstitialDismissedEvent;

	public static event Action onNativeAdLoadedEvent;

	public static event Action onNativeAdFailedEvent;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void onAdLoaded(string empty)
	{
		if (MoPubAndroidManager.onAdLoadedEvent != null)
		{
			MoPubAndroidManager.onAdLoadedEvent();
		}
	}

	public void onAdFailed(string empty)
	{
		if (MoPubAndroidManager.onAdFailedEvent != null)
		{
			MoPubAndroidManager.onAdFailedEvent();
		}
	}

	public void onAdClicked(string empty)
	{
		if (MoPubAndroidManager.onAdClickedEvent != null)
		{
			MoPubAndroidManager.onAdClickedEvent();
		}
	}

	public void onAdExpanded(string empty)
	{
		if (MoPubAndroidManager.onAdExpandedEvent != null)
		{
			MoPubAndroidManager.onAdExpandedEvent();
		}
	}

	public void onAdCollapsed(string empty)
	{
		if (MoPubAndroidManager.onAdCollapsedEvent != null)
		{
			MoPubAndroidManager.onAdCollapsedEvent();
		}
	}

	public void onInterstitialLoaded(string empty)
	{
		if (MoPubAndroidManager.onInterstitialLoadedEvent != null)
		{
			MoPubAndroidManager.onInterstitialLoadedEvent();
		}
	}

	public void onInterstitialFailed(string empty)
	{
		if (MoPubAndroidManager.onInterstitialFailedEvent != null)
		{
			MoPubAndroidManager.onInterstitialFailedEvent();
		}
	}

	public void onInterstitialShown(string empty)
	{
		if (MoPubAndroidManager.onInterstitialShownEvent != null)
		{
			MoPubAndroidManager.onInterstitialShownEvent();
		}
	}

	public void onInterstitialClicked(string empty)
	{
		if (MoPubAndroidManager.onInterstitialClickedEvent != null)
		{
			MoPubAndroidManager.onInterstitialClickedEvent();
		}
	}

	public void onInterstitialDismissed(string empty)
	{
		if (MoPubAndroidManager.onInterstitialDismissedEvent != null)
		{
			MoPubAndroidManager.onInterstitialDismissedEvent();
		}
	}

	public void onNativeAdLoaded(string empty)
	{
		if (MoPubAndroidManager.onAdLoadedEvent != null)
		{
			MoPubAndroidManager.onAdLoadedEvent();
		}
	}

	public void onNativeAdFailed(string empty)
	{
		if (MoPubAndroidManager.onAdFailedEvent != null)
		{
			MoPubAndroidManager.onAdFailedEvent();
		}
	}
}
