using DevonLocalization.Core;
using Disney.ClubPenguin.Login.Authentication;
using Disney.DMOAnalytics;
using ErrorPopup.Core;
using UnityEngine;

public class StrikeModules : MonoBehaviour
{
	public const string LocalizedFilesDirSoundStudio = "Assets/soundstudio/Resources/SSTranslations";

	public const string LocalizedFilesDirLogin = "Assets/Framework/Login/Resources/Translations";

	public const string ERRORS_JSON_SOUNDSTUDIO = "Assets/soundstudio/Resources/ErrorPopup/SoundStudioErrors.json.txt";

	public const string ERRORS_JSON_LOGIN = "Assets/Framework/Login/Resources/ErrorPopup/LoginErrors.json.txt";

	private const string LOGIN_MODULE_ID = "LOGIN";

	public LoginContext loginContextPrefab;

	public Language locLanguage;

	public void Init()
	{
		SetupLocalization();
		SetupErrorPopups();
	}

	private void SetupLocalization()
	{
		locLanguage = Language.en_US;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.English:
			locLanguage = Language.en_US;
			break;
		case SystemLanguage.French:
			locLanguage = Language.fr_FR;
			break;
		case SystemLanguage.Spanish:
			locLanguage = Language.es_LA;
			break;
		case SystemLanguage.Portuguese:
			locLanguage = Language.pt_BR;
			break;
		case SystemLanguage.German:
			locLanguage = Language.de_DE;
			break;
		case SystemLanguage.Russian:
			locLanguage = Language.ru_RU;
			break;
		}
		Localizer.Instance.Language = locLanguage;
		AppTokensFilePath path = new AppTokensFilePath("Assets/soundstudio/Resources/SSTranslations", Platform.global);
		ModuleTokensFilePath path2 = new ModuleTokensFilePath("Assets/Framework/Login/Resources/Translations", "LOGIN", Platform.global);
		AppTokensFilePath appTokensFilePath = null;
		appTokensFilePath = new AppTokensFilePath("Assets/soundstudio/Resources/SSTranslations", Platform.android);
		Localizer.Instance.LoadTokensFromLocalJSON(path, onTokensLoaded);
		Localizer.Instance.LoadTokensFromLocalJSON(path2, onTokensLoaded);
		if (appTokensFilePath != null)
		{
			Localizer.Instance.LoadTokensFromLocalJSON(appTokensFilePath, onTokensLoaded);
		}
	}

	private void onTokensLoaded(bool tokensUpdated)
	{
	}

	private void SetupErrorPopups()
	{
		ErrorsMap.Instance.LoadErrorsLocally("Assets/soundstudio/Resources/ErrorPopup/SoundStudioErrors.json.txt");
		ErrorsMap.Instance.LoadErrorsLocally("Assets/Framework/Login/Resources/ErrorPopup/LoginErrors.json.txt");
	}

	private void OnApplicationQuit()
	{
		DMOAnalytics.SharedAnalytics.LogAppEnd();
	}
}
