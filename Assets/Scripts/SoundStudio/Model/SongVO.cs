using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SoundStudio.Model
{
	public class SongVO
	{
		public delegate void SongStateUpdated();

		public const int INVALID_GENRE_ID = -1;

		internal const string SYNC_EVENT = "SYNC_EVENT";

		private int genreID = -1;

		private string _rawData;

		private List<RecordDataVO> _recordDataList;

		public string songName
		{
			get;
			set;
		}

		public int GenreID
		{
			get
			{
				return genreID;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("The genre id was not set properly");
				}
				genreID = value;
			}
		}

		[JsonIgnore]
		public int totalLikes
		{
			get;
			set;
		}

		public bool isShared
		{
			get;
			set;
		}

		public long playerid
		{
			get;
			set;
		}

		public long serverID
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public DateTime timeStamp
		{
			get;
			set;
		}

		public bool cachedDelete
		{
			get;
			set;
		}

		[JsonIgnore]
		public bool HasServerID => serverID != 0;

		public string rawData
		{
			get
			{
				return _rawData;
			}
			set
			{
				_rawData = value;
				_recordDataList = RawData_To_RecordData(value);
			}
		}

		[JsonIgnore]
		public List<RecordDataVO> recordDataList
		{
			get
			{
				return _recordDataList;
			}
			set
			{
				_recordDataList = value;
				_rawData = RecordData_To_RawData(value);
			}
		}

		[JsonIgnore]
		public float LengthInInMilliseconds => Mathf.Floor(_recordDataList[_recordDataList.Count - 1].timeStamp);

		[JsonIgnore]
		public TimeSpan Length => new TimeSpan(0, 0, 0, 0, (int)LengthInInMilliseconds);

		public event SongStateUpdated SongNameEvent;

		public event SongStateUpdated SongShareEvent;

		public event SongStateUpdated SongSyncEvent;

		public event SongStateUpdated SongPlayEvent;

		public event SongStateUpdated SongStopEvent;

		public event SongStateUpdated SongDeleteEvent;

		public event SongStateUpdated SongCompleteEvent;

		public void dispatchCompleteEvent()
		{
			if (this.SongCompleteEvent != null)
			{
				this.SongCompleteEvent();
			}
		}

		public void dispatchSyncEvent()
		{
			if (this.SongSyncEvent != null)
			{
				this.SongSyncEvent();
			}
		}

		public void dispatchNameEvent()
		{
			if (this.SongNameEvent != null)
			{
				this.SongNameEvent();
			}
		}

		public void dispatchShareEvent()
		{
			if (this.SongShareEvent != null)
			{
				this.SongShareEvent();
			}
		}

		public void dispatchPlayEvent()
		{
			if (this.SongPlayEvent != null)
			{
				this.SongPlayEvent();
			}
		}

		public void dispatchStopEvent()
		{
			if (this.SongStopEvent != null)
			{
				this.SongStopEvent();
			}
		}

		public void dispatchDeleteEvent()
		{
			if (this.SongDeleteEvent != null)
			{
				this.SongDeleteEvent();
			}
		}

		private string RecordData_To_RawData(List<RecordDataVO> recordDataList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < recordDataList.Count; i++)
			{
				int num = (int)Math.Floor(recordDataList[i].timeStamp);
				stringBuilder.Append(Utils.binaryToHex(recordDataList[i].gridStatus)).Append("|").Append(num.ToString("X4"));
				if (i < recordDataList.Count - 1)
				{
					stringBuilder.Append(",");
				}
			}
			return stringBuilder.ToString();
		}

		public static List<RecordDataVO> RawData_To_RecordData(string rawData)
		{
			string[] array = rawData.Split(',');
			List<RecordDataVO> list = new List<RecordDataVO>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string gridStatus = Utils.hexToBinary(text.Split('|')[0]);
				float timeStamp = Utils.hexToDecimal(text.Split('|')[1]);
				list.Add(new RecordDataVO(gridStatus, timeStamp));
			}
			return list;
		}

		protected bool Equals(SongVO other)
		{
			return object.Equals(_recordDataList, other._recordDataList) && string.Equals(songName, other.songName) && GenreID == other.GenreID && serverID == other.serverID;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((SongVO)obj);
		}

		public override int GetHashCode()
		{
			int num = (_recordDataList != null) ? _recordDataList.GetHashCode() : 0;
			num = ((num * 397) ^ ((songName != null) ? songName.GetHashCode() : 0));
			num = ((num * 397) ^ GenreID);
			return (num * 397) ^ (int)serverID;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (RecordDataVO recordData in _recordDataList)
			{
				stringBuilder.Append("[").Append(recordData).Append("]");
			}
			return $"songName: {songName}, GenreID: {GenreID}, isShared: {isShared}, playerid: {playerid}, serverID: {serverID}, timeStamp: {timeStamp}, length: {Length}, Recorded Data: {stringBuilder.ToString()}";
		}

		public bool IsEquivalentData(SongVO other)
		{
			return songName.Equals(other.songName) && rawData.Equals(other.rawData);
		}

		public HashSet<int> CalculateUniqueSoundIDs()
		{
			HashSet<int> hashSet = new HashSet<int>();
			long num = 0L;
			foreach (RecordDataVO recordData in _recordDataList)
			{
				if (recordData.GridValue != 65535)
				{
					num |= recordData.GridValue;
				}
			}
			for (int i = 0; i < 40; i++)
			{
				long num2 = 1L << 39 - i;
				if ((num & num2) != 0L)
				{
					hashSet.Add(i);
				}
			}
			return hashSet;
		}
	}
}
