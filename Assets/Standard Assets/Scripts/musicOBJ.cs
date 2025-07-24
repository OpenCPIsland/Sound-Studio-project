using System.Runtime.InteropServices;
using UnityEngine;

public class musicOBJ
{
	[DllImport("__Internal")]
	private static extern bool _IsMusicPlaying();

	public static bool isMusicPlaying()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _IsMusicPlaying();
		}
		return false;
	}
}
