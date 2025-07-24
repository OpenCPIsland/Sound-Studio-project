using SoundStudio.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SongRecorder
{
	public delegate void RecordStartAction();

	public delegate void RcordStopAction();

	private List<RecordDataVO> _songData = new List<RecordDataVO>();

	private float startTime;

	private bool isRecording;

	public bool IsRecording => isRecording;

	public static event RecordStartAction OnRecordStart;

	public static event RcordStopAction OnRecordStop;

	public void Record()
	{
		isRecording = true;
		ClearData();
		dispatchRecordStart();
		startTime = Time.time * 1000f;
	}

	public void Stop()
	{
		Stop(Mathf.Round(Time.time * 1000f - startTime));
	}

	public void Stop(float elapsedTimeInMilliseconds)
	{
		isRecording = false;
		_songData.Add(new RecordDataVO("1111111111111111", elapsedTimeInMilliseconds));
		dispatchRecordStop();
	}

	private void ClearData()
	{
		_songData = new List<RecordDataVO>();
	}

	public void AddData(string gridStatus)
	{
		AddData(gridStatus, Mathf.Round(Time.time * 1000f - startTime));
	}

	public void AddData(string gridStatus, float elapsedTimeInMilliseconds)
	{
		if (_songData.Count > 1 && _songData[_songData.Count - 1].timeStamp == elapsedTimeInMilliseconds)
		{
			_songData[_songData.Count - 1] = new RecordDataVO(gridStatus, elapsedTimeInMilliseconds);
		}
		else
		{
			_songData.Add(new RecordDataVO(gridStatus, elapsedTimeInMilliseconds));
		}
	}

	public SongVO createSong(string name, int genre)
	{
		SongVO songVO = new SongVO();
		songVO.songName = name;
		songVO.GenreID = genre;
		songVO.recordDataList = _songData;
		songVO.timeStamp = DateTime.Now;
		return songVO;
	}

	private void dispatchRecordStart()
	{
		if (SongRecorder.OnRecordStart != null)
		{
			SongRecorder.OnRecordStart();
		}
	}

	private void dispatchRecordStop()
	{
		if (SongRecorder.OnRecordStop != null)
		{
			SongRecorder.OnRecordStop();
		}
	}
}
