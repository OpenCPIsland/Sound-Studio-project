using SoundStudio;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RecordProgressView : View
{
	private const float TICK_TIME = 1f;

	private const float PROGRESS_POS_START = -492.2f;

	private const float PROGRESS_POS_END = 254.74f;

	public Slider progressSlider;

	public Text timeText;

	private bool _isRecording;

	private float _recordStartTime;

	[Inject]
	public SongRecorder songRecorder
	{
		get;
		set;
	}

	protected override void Start()
	{
		SongRecorder.OnRecordStart += SongRecorder_Record_Handler;
		SongRecorder.OnRecordStop += SongRecorder_Stop_Handler;
	}

	protected override void OnDestroy()
	{
		SongRecorder.OnRecordStart -= SongRecorder_Record_Handler;
		SongRecorder.OnRecordStop -= SongRecorder_Stop_Handler;
	}

	private void Update()
	{
		if (_isRecording)
		{
			float time = Time.time;
			float value = (time - _recordStartTime) / 180f;
			progressSlider.value = value;
			timeText.text = Utils.secondsToMinutes(Mathf.RoundToInt(time - _recordStartTime));
			if (Mathf.RoundToInt(time - _recordStartTime) >= 180)
			{
				songRecorder.Stop();
			}
		}
	}

	private void SongRecorder_Record_Handler()
	{
		_isRecording = true;
		_recordStartTime = Time.time;
	}

	private void SongRecorder_Stop_Handler()
	{
		_isRecording = false;
		progressSlider.value = 0f;
		timeText.text = "0:00";
	}
}
