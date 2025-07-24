using System.Collections;
using UnityEngine;

internal class ObbDownloader : MonoBehaviour
{
	private string expPath = string.Empty;

	private void Start()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNames.SplashScreen.ToString());
	}
}
