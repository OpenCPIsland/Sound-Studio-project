using Disney.ClubPenguin.Service.MWS.Domain;
using SoundStudio.Model;
using System;
using System.Globalization;
using UnityEngine;

namespace SoundStudio
{
	public static class Utils
	{
		private static readonly ulong EOF = 65535uL;

		public static string binaryToHex(string binary)
		{
			return Convert.ToUInt64(binary, 2).ToString("X");
		}

		public static string hexToBinary(string hex)
		{
			ulong num = Convert.ToUInt64(hex, 16);
			if (num == EOF)
			{
				return Convert.ToString((long)num, 2);
			}
			return Convert.ToString((long)num, 2).PadLeft(40, '0').Trim();
		}

		public static int hexToDecimal(string hex)
		{
			return Convert.ToInt32(hex, 16);
		}

		public static string secondsToMinutes(int seconds)
		{
			string str = (seconds / 60).ToString();
			string text = (seconds % 60).ToString();
			if (text.Length == 1)
			{
				text = "0" + text;
			}
			return str + ":" + text;
		}

		public static SongVO ConvertSoundStudioTrackDataToSongVO(SoundStudioTrackData trackData)
		{
			if (trackData == null)
			{
				throw new InvalidOperationException("The track cannot be null");
			}
			if (string.IsNullOrEmpty(trackData.Data))
			{
				throw new InvalidOperationException("The track cannot be ");
			}
			SongVO songVO = new SongVO();
			int num = trackData.Data.IndexOf(',');
			if (num < 1)
			{
				throw new InvalidOperationException("The track data is not valid as the genre id is not at the beginning of the data");
			}
			try
			{
				songVO.GenreID = int.Parse(trackData.Data.Substring(0, num));
				songVO.rawData = trackData.Data.Substring(trackData.Data.IndexOf(',') + 1);
				songVO.songName = trackData.Name;
				songVO.playerid = trackData.PlayerId;
				songVO.serverID = trackData.TrackId;
				songVO.timeStamp = trackData.GetLastModifieDateTime();
				songVO.isShared = (trackData.TrackShareState == TrackShareState.SHARED);
				return songVO;
			}
			catch (FormatException)
			{
				return songVO;
			}
		}

		public static SoundStudioTrackData ConvertSongVOToSoundStudioTrackData(SongVO song)
		{
			SoundStudioTrackData soundStudioTrackData = new SoundStudioTrackData();
			soundStudioTrackData.Data = song.GenreID + "," + song.rawData;
			soundStudioTrackData.Name = song.songName;
			if (song.playerid == 0L)
			{
				throw new InvalidOperationException("The player id must be set!!!");
			}
			soundStudioTrackData.PlayerId = song.playerid;
			return soundStudioTrackData;
		}

		public static double unixTimeStamp()
		{
			long num = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
			num /= 10000000;
			return num;
		}

		public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
		{
			DateTime result = start;
			while (result.DayOfWeek != day)
			{
				result = result.AddDays(1.0);
			}
			return result;
		}

		public static string GetGuid()
		{
			string text;
			if (PlayerPrefs.HasKey("GUID"))
			{
				text = PlayerPrefs.GetString("GUID");
			}
			else
			{
				text = Guid.NewGuid().ToString();
				PlayerPrefs.SetString("GUID", text);
			}
			return text;
		}

		public static string SystemLanguageToMWSLocale()
		{
			return SystemLanguageToMWSLocale(Application.systemLanguage);
		}

		public static string SystemLanguageToMWSLocale(SystemLanguage systemLanguage)
		{
			string result = "en";
			switch (systemLanguage)
			{
			case SystemLanguage.French:
				result = "fr";
				break;
			case SystemLanguage.German:
				result = "de";
				break;
			case SystemLanguage.Spanish:
				result = "es";
				break;
			case SystemLanguage.Portuguese:
				result = "pt";
				break;
			case SystemLanguage.Russian:
				result = "ru";
				break;
			}
			return result;
		}

		public static Color32 PenguinColorIDToColor(int colorID)
		{
			switch (colorID)
			{
			case 1:
				return HexToColor("003A6F");
			case 2:
				return HexToColor("00C234");
			case 3:
				return HexToColor("FF5ADF");
			case 4:
				return HexToColor("4A4A4A");
			case 5:
				return HexToColor("E5001E");
			case 6:
				return HexToColor("FF5A00");
			case 7:
				return HexToColor("FFD02F");
			case 8:
				return HexToColor("983EC8");
			case 9:
				return HexToColor("703801");
			case 10:
				return HexToColor("FF436E");
			case 11:
				return HexToColor("007A26");
			case 12:
				return HexToColor("0080F1");
			case 13:
				return HexToColor("56F21B");
			case 14:
				return HexToColor("0098C0");
			case 15:
				return HexToColor("0098C0");
			case 16:
				return HexToColor("F1EEDA");
			default:
				UnityEngine.Debug.LogWarning("Penguin colour id not found: " + colorID);
				return HexToColor("003A6F");
			}
		}

		public static Color32 HexToColor(string HexString)
		{
			int num = int.Parse(HexString, NumberStyles.HexNumber);
			byte r = (byte)((num >> 16) & 0xFF);
			byte g = (byte)((num >> 8) & 0xFF);
			byte b = (byte)(num & 0xFF);
			return new Color32(r, g, b, byte.MaxValue);
		}
	}
}
