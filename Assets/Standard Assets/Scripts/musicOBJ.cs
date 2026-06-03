using System.Runtime.InteropServices;
using UnityEngine;

public class musicOBJ
{
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern bool _IsMusicPlaying();
#endif

    public static bool isMusicPlaying()
    {
#if UNITY_IOS && !UNITY_EDITOR
		return _IsMusicPlaying();
#elif UNITY_ANDROID && !UNITY_EDITOR
		return false;
#else
        return false;
#endif
    }
}