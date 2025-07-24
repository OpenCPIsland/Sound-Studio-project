using System.Collections;
using UnityEngine;

public class AnimateLogo : MonoBehaviour
{
	public GameObject logo_EN;

	public GameObject logo_RU;

	public GameObject disclaimer_EN;

	public GameObject disclaimer_FR;

	public GameObject disclaimer_PT;

	public GameObject disclaimer_ES;

	public GameObject disclaimer_DE;

	public GameObject disclaimer_RU;

	private IEnumerator Start()
	{
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: true);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: false);
			break;
		case SystemLanguage.French:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: false);
			disclaimer_FR.SetActive(value: true);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: false);
			break;
		case SystemLanguage.Spanish:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: false);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: true);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: false);
			break;
		case SystemLanguage.Portuguese:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: false);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: true);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: false);
			break;
		case SystemLanguage.German:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: false);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: true);
			disclaimer_RU.SetActive(value: false);
			break;
		case SystemLanguage.Russian:
			logo_EN.SetActive(value: false);
			logo_RU.SetActive(value: true);
			disclaimer_EN.SetActive(value: false);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: true);
			break;
		default:
			logo_EN.SetActive(value: true);
			logo_RU.SetActive(value: false);
			disclaimer_EN.SetActive(value: true);
			disclaimer_FR.SetActive(value: false);
			disclaimer_PT.SetActive(value: false);
			disclaimer_ES.SetActive(value: false);
			disclaimer_DE.SetActive(value: false);
			disclaimer_RU.SetActive(value: false);
			break;
		}
		yield return new WaitForSeconds(2f);
		LoadMainMenu();
	}

	private void LoadMainMenu()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("SizzleVideo");
	}
}
