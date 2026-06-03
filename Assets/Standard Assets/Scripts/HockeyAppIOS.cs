using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

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
		Application.logMessageReceived -= OnHandleLogCallback;
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= OnHandleLogCallback;
	}

	private void OnHandleLogCallback(string logString, string stackTrace, LogType type)
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
				using (UnityWebRequest www = UnityWebRequest.Post(url, postForm))
				{
					www.SetRequestHeader("Authorization", " FD 5F20D8F8-9411-45D7-ADAC-F186C5B3574C:72C6967910F6B3FD03DF0AAF9C692860409908D8AD8CCC9E");
					www.SetRequestHeader("Content-Type", lContent2);
					yield return www.SendWebRequest();
					if (www.result == UnityWebRequest.Result.Success)
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
		}

	protected virtual void HandleException(string logString, string stackTrace)
	{
	}

	public void OnHandleUnresolvedException(object sender, UnhandledExceptionEventArgs args)
	{
	}

	protected virtual string GetBaseURL()
	{
		string empty = string.Empty;
		string text = serverURL.Trim();
		if (text.Length > 0)
		{
			empty = text;
			if (!empty[empty.Length - 1].Equals("/"))
			{
				empty += "/";
			}
		}
		else
		{
			empty = "https://api.disney.com/dmn/crash/v2";
		}
		return empty;
	}

	protected virtual bool IsConnected()
	{
		bool result = false;
		if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			result = true;
		}
		return result;
	}
}
