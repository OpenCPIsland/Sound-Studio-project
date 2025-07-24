using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class HockeyAppAndroid : MonoBehaviour
{
	private const string HOCKEYAPP_BASEURL = "https://api.disney.com/dmn/crash/v2";

	protected const string HEADER_KEY = " FD 5F20D8F8-9411-45D7-ADAC-F186C5B3574C:72C6967910F6B3FD03DF0AAF9C692860409908D8AD8CCC9E";

	private const string HOCKEYAPP_CRASHESPATH = "apps/[APPID]/crashes/upload";

	private const int MAX_CHARS = 199800;

	protected const string LOG_FILE_DIR = "/logs/";

	public string appID = "your-hockey-app-id";

	public string packageID = "your-package-identifier";

	private string serverURL = "https://api.disney.com/dmn/crash/v2";

	public bool autoUpload;

	public bool exceptionLogging;

	public bool updateManager;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (exceptionLogging && IsConnected())
		{
			List<string> logFiles = GetLogFiles();
			if (logFiles.Count > 0)
			{
				StartCoroutine(SendLogs(GetLogFiles()));
			}
		}
		string baseURL = GetBaseURL();
		UnityEngine.Debug.LogError("Calling StartCrashManager");
		StartCrashManager(baseURL, appID, updateManager, autoUpload);
	}

	public void OnEnable()
	{
		if (exceptionLogging)
		{
			AppDomain.CurrentDomain.UnhandledException += OnHandleUnresolvedException;
			Application.RegisterLogCallback(OnHandleLogCallback);
		}
	}

	public void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void OnDestroy()
	{
		Application.RegisterLogCallback(null);
	}

	protected void StartCrashManager(string urlString, string appID, bool updateManagerEnabled, bool autoSendEnabled)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin");
		UnityEngine.Debug.LogError("Calling native startHockeyAppManager");
		androidJavaClass2.CallStatic("startHockeyAppManager", @static, urlString, appID, updateManagerEnabled, autoSendEnabled);
	}

	private string GetVersion()
	{
		string text = null;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("net.hockeyapp.unity.HockeyUnityPlugin");
		return androidJavaClass.CallStatic<string>("getAppVersion", new object[0]);
	}

	protected virtual List<string> GetLogHeaders()
	{
		List<string> list = new List<string>();
		list.Add("Package: " + packageID);
		string version = GetVersion();
		list.Add("Version: " + version);
		string[] array = SystemInfo.operatingSystem.Split('/');
		string item = "Android: " + array[0].Replace("Android OS ", string.Empty);
		list.Add(item);
		list.Add("Model: " + SystemInfo.deviceModel);
		list.Add("Date: " + DateTime.UtcNow.ToString("ddd MMM dd HH:mm:ss {}zzzz yyyy").Replace("{}", "GMT"));
		return list;
	}

	protected virtual WWWForm CreateForm(string log)
	{
		WWWForm wWWForm = new WWWForm();
		byte[] array = null;
		using (FileStream fileStream = File.OpenRead(log))
		{
			if (fileStream.Length > 199800)
			{
				string str = null;
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					streamReader.BaseStream.Seek(fileStream.Length - 199800, SeekOrigin.Begin);
					str = streamReader.ReadToEnd();
				}
				List<string> logHeaders = GetLogHeaders();
				string str2 = string.Empty;
				foreach (string item in logHeaders)
				{
					str2 = str2 + item + "\n";
				}
				str = str2 + "\n[...]" + str;
				try
				{
					array = Encoding.Default.GetBytes(str);
				}
				catch (ArgumentException arg)
				{
					if (Debug.isDebugBuild)
					{
						UnityEngine.Debug.Log("Failed to read bytes of log file: " + arg);
					}
				}
			}
			else
			{
				try
				{
					array = File.ReadAllBytes(log);
				}
				catch (SystemException arg2)
				{
					if (Debug.isDebugBuild)
					{
						UnityEngine.Debug.Log("Failed to read bytes of log file: " + arg2);
					}
				}
			}
		}
		if (array != null)
		{
			wWWForm.AddBinaryData("log", array, log, "text/plain");
		}
		return wWWForm;
	}

	protected virtual List<string> GetLogFiles()
	{
		List<string> list = new List<string>();
		string path = Application.persistentDataPath + "/logs/";
		try
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			FileInfo[] files = directoryInfo.GetFiles();
			if (files.Length <= 0)
			{
				return list;
			}
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				if (fileInfo.Extension == ".log")
				{
					list.Add(fileInfo.FullName);
				}
				else
				{
					File.Delete(fileInfo.FullName);
				}
			}
			return list;
		}
		catch (Exception arg)
		{
			if (!Debug.isDebugBuild)
			{
				return list;
			}
			UnityEngine.Debug.Log("Failed to write exception log to file: " + arg);
			return list;
		}
	}

	protected virtual IEnumerator SendLogs(List<string> logs)
	{
		string crashPath = "apps/[APPID]/crashes/upload";
		string url = GetBaseURL() + crashPath.Replace("[APPID]", appID);
		foreach (string log in logs)
		{
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
		string str = DateTime.Now.ToString("yyyy-MM-dd-HH_mm_ss_fff");
		string str2 = logString.Replace("\n", " ");
		string[] array = stackTrace.Split('\n');
		str2 = "\n" + str2 + "\n";
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length > 0)
			{
				str2 = str2 + "  at " + text + "\n";
			}
		}
		List<string> logHeaders = GetLogHeaders();
		using (StreamWriter streamWriter = new StreamWriter(Application.persistentDataPath + "/logs/LogFile_" + str + ".log", append: true))
		{
			foreach (string item in logHeaders)
			{
				streamWriter.WriteLine(item);
			}
			streamWriter.WriteLine(str2);
		}
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

	protected virtual void HandleException(string logString, string stackTrace)
	{
		WriteLogToDisk(logString, stackTrace);
	}

	public void OnHandleLogCallback(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Assert || type == LogType.Exception)
		{
			HandleException(logString, stackTrace);
		}
	}

	public void OnHandleUnresolvedException(object sender, UnhandledExceptionEventArgs args)
	{
		if (args != null && args.ExceptionObject != null && args.ExceptionObject.GetType() == typeof(Exception))
		{
			Exception ex = (Exception)args.ExceptionObject;
			HandleException(ex.Source, ex.StackTrace);
		}
	}
}
