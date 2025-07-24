using UnityEngine;

public class MoPubAndroid
{
	private static AndroidJavaObject _plugin;

	static MoPubAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mopub.mobileads.MoPubUnityPlugin"))
			{
				_plugin = androidJavaClass.CallStatic<AndroidJavaObject>("instance", new object[0]);
			}
		}
	}

	public static void createBanner(MNMoPubBannerType size, MNMoPubAdPlacement position, string adUnitId, string keywords, int marginTop, int marginLeft, int customBannerWidth, int customBannerHeight)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showBanner", (int)size, (int)position, adUnitId, keywords, marginTop, marginLeft, customBannerWidth, customBannerHeight);
		}
	}

	public static void hideBanner(string adUnitId, bool shouldHide)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("hideBanner", adUnitId, shouldHide);
		}
	}

	public static void setBannerKeywords(string keywords)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setBannerKeywords", keywords);
		}
	}

	public static void destroyBanner(string adUnitId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("destroyBanner", adUnitId);
		}
	}

	public static void SetAutoRefreshEnabled(string adUnitId, bool enabled)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setAutorefreshEnabled", adUnitId, enabled);
		}
	}

	public static void requestInterstitalAd(string adUnitId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestInterstitalAd", adUnitId);
		}
	}

	public static void requestInterstitalAdWithKeywords(string adUnitId, string keywords)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestInterstitalAdWithKeywords", adUnitId, keywords);
		}
	}

	public static void setInterstitialKeywords(string keywords)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("setInterstitialKeywords", keywords);
		}
	}

	public static void showInterstitalAd()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("showInterstitalAd");
		}
	}

	public static void requestNativeAd(string adUnitId, string keywords)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("requestNativeAd", adUnitId, keywords);
		}
	}

	public static string getNativeAdTitle(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		return _plugin.Call<string>("getNativeAdTitle", new object[1]
		{
			adUnitId
		});
	}

	public static string getNativeAdImageURL(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		return _plugin.Call<string>("getNativeAdImageURL", new object[1]
		{
			adUnitId
		});
	}

	public static string getNativeAdClickURL(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		return _plugin.Call<string>("getNativeAdClickURL", new object[1]
		{
			adUnitId
		});
	}

	public static void callNativeAdTrackImpression(string adUnitId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("callNativeAdTrackImpression", adUnitId);
		}
	}

	public static void callNativeAdTrackClick(string adUnitId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("callNativeAdTrackClick", adUnitId);
		}
	}

	public static void reportApplicationOpen()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			_plugin.Call("reportApplicationOpen");
		}
	}

	public static bool isInterstitalAdReady(string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return false;
		}
		return _plugin.Call<bool>("isInterstitalAdReady", new object[1]
		{
			adUnitId
		});
	}
}
