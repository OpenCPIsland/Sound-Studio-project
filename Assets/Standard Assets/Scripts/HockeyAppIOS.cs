using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HockeyAppIOS : MonoBehaviour
{
	protected const string HOCKEYAPP_BASEURL = "https://api.disney.com/dmn/crash/v2";

	protected const string HOCKEYAPP_CRASHESPATH = "apps/[APPID]/crashes/upload";

	protected const string HEADER_KEY = " FD 5F20D8F8-9411-45D7-ADAC-F186C5B3574C:72C6967910F6B3FD03DF0AAF9C692860409908D8AD8CCC9E";

	protected const int MAX_CHARS = 199800;

	protected const string LOG_FILE_DIR = "/logs/";

	public string appID = "1e3d4401c074850da8d983d3044d482d";

	private string secret = "a019b857f8da2c2b840bbcca56dea0b9";

	private string authenticationType = " ";

	private string serverURL = "https://api.disney.com/dmn/crash/v2";

	public bool autoUpload;

	public bool exceptionLogging;

	public bool updateManager;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void OnDestroy()
	{
		Application.RegisterLogCallback(null);
	}

	private void GameViewLoaded(string message)
	{
	}

	protected virtual List<string> GetLogHeaders()
	{
		return new List<string>();
	}

	protected virtual WWWForm CreateForm(string log)
	{
		WWWForm result = new WWWForm();
		byte[] array = null;
		return result;
	}

	protected virtual List<string> GetLogFiles()
	{
		return new List<string>();
	}

	protected virtual IEnumerator SendLogs(List<string> logs)
	{
		foreach (string log in logs)
		{
			string crashPath = "apps/[APPID]/crashes/upload";
			string url = GetBaseURL() + crashPath.Replace("[APPID]", appID);
			WWWForm postForm = CreateForm(log);
			string lContent2 = postForm.headers["Content-Type"].ToString();
			lContent2 = lContent2.Replace("\"", string.Empty);
			WWW www = new WWW(headers: new Hashtable
			{
				{
					"Authorization",
					" FD 5F20D8F8-9411-45D7-ADAC-F186C5B3574C:72C6967910F6B3FD03DF0AAF9C692860409908D8AD8CCC9E"
				},
				{
					"Content-Type",
					lContent2
				}
			}, url: url, postData: postForm.data);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				try
				{
					File.Delete(log);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					if (Debug.isDebugBuild)
					{
						UnityEngine.Debug.Log("Failed to delete exception log: " + e);
					}
				}
			}
		}
	}

	protected virtual void WriteLogToDisk(string logString, string stackTrace)
	{
	}

	protected virtual string GetBaseURL()
	{
		return string.Empty;
	}

	protected virtual bool IsConnected()
	{
		return false;
	}

	protected virtual void HandleException(string logString, string stackTrace)
	{
	}

	public void OnHandleLogCallback(string logString, string stackTrace, LogType type)
	{
	}

	public void OnHandleUnresolvedException(object sender, UnhandledExceptionEventArgs args)
	{
	}
}
