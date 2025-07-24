using UnityEngine;

public class ReferralStoreHelperAndroid : IReferralStoreHelper
{
	public void Show()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.mobilenetwork.referralstore.DMNReferralStoreActivity");
		AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.Bundle");
		AndroidJavaObject androidJavaObject2 = @static.Call<AndroidJavaObject>("getPackageName", new object[0]);
		androidJavaObject.Call("putString", androidJavaClass2.GetStatic<string>("APP_ID"), androidJavaObject2);
		AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("android.content.Intent", @static, androidJavaClass2);
		androidJavaObject3.Call<AndroidJavaObject>("putExtras", new object[1]
		{
			androidJavaObject
		});
		@static.Call("startActivity", androidJavaObject3);
	}
}
