using UnityEngine;

public class MoPubHelper : MonoBehaviour
{
	public static void CreateBanner(string adUnitId, MNMoPubAdPlacement position = MNMoPubAdPlacement.Custom, MNMoPubBannerType size = MNMoPubBannerType.Size320x50, string keywords = "", int marginTop = 0, int marginLeft = 0, int customBannerWidth = 0, int customBannerHeight = 0)
	{
		MoPubAndroid.createBanner(size, position, adUnitId, keywords, marginTop, marginLeft, customBannerWidth, customBannerHeight);
	}

	public static void DestroyBanner(string adUnitId)
	{
		MoPubAndroid.destroyBanner(adUnitId);
	}

	public static void ShowBanner(string adUnitId)
	{
		MoPubAndroid.hideBanner(adUnitId, shouldHide: false);
	}

	public static void HideBanner(string adUnitId)
	{
		MoPubAndroid.hideBanner(adUnitId, shouldHide: true);
	}

	public static void SetAutoRefreshBannerEnabled(string adUnitId, bool enabled)
	{
		MoPubAndroid.SetAutoRefreshEnabled(adUnitId, enabled);
	}

	public static void ReadjustAdPosition(string adUnitId)
	{
	}

	public static void RequestInterstitial(string adUnitId)
	{
		MoPubAndroid.requestInterstitalAd(adUnitId);
	}

	public static void RequestInterstitialWithKeywords(string adUnitId, string keywords = null)
	{
		MoPubAndroid.requestInterstitalAdWithKeywords(adUnitId, keywords);
	}

	public static void ShowInterstitial(string adUnitId)
	{
		MoPubAndroid.showInterstitalAd();
	}

	public static void RequestNativeAd(string adUnitId, string keywords = null)
	{
		MoPubAndroid.requestNativeAd(adUnitId, keywords);
	}

	public static string GetNativeAdTitle(string adUnitId)
	{
		return MoPubAndroid.getNativeAdTitle(adUnitId);
	}

	public static string GetNativeAdImageURL(string adUnitId)
	{
		return MoPubAndroid.getNativeAdImageURL(adUnitId);
	}

	public static string GetNativeAdClickURL(string adUnitId)
	{
		return MoPubAndroid.getNativeAdClickURL(adUnitId);
	}

	public static void CallNativeAdTrackImpression(string adUnitId)
	{
		MoPubAndroid.callNativeAdTrackImpression(adUnitId);
	}

	public static void CallNativeAdTrackClick(string adUnitId)
	{
		MoPubAndroid.callNativeAdTrackClick(adUnitId);
	}

	public static void ReportApplicationOpen(string itunesAppId)
	{
		MoPubAndroid.reportApplicationOpen();
	}

	public static void RefreshBannerWithKeywords(string keywords = "")
	{
		MoPubAndroid.setBannerKeywords(keywords);
	}

	public static bool IsInterstitalAdReady(string adUnitId)
	{
		return MoPubAndroid.isInterstitalAdReady(adUnitId);
	}
}
