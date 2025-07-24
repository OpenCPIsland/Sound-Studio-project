using System.Collections;
using UnityEngine;

public class PlayVideoTexture : MonoBehaviour
{

	private void Awake()
	{

		StartCoroutine(DelayedLoadMain());

#if Unity_Android
		Handheld.PlayFullScreenMovie("AppIntro-960x540_android.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
#endif

	}

	private IEnumerator DelayedLoadMain()
	{
		yield return null;
		UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.Main.ToString());
	}
}
