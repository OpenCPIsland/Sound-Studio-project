using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingBoardView : View
{
	internal const string BACK_CLICK_EVENT = "BACK_CLICK_EVENT";

	internal const string SOUND_CLICK_EVENT = "SOUND_CLICK_EVENT";

	internal const string RECORD_CLICK_EVENT = "RECORD_CLICK_EVENT";

	internal const string STOP_CLICK_EVENT = "STOP_CLICK_EVENT";

	public GameObject EditSongPrefab;

	public GameObject CountDownPrefab;

	public GameObject CanvasObject;

	public GameObject RecordButton;

	public GameObject StopButton;

	public GameObject LockedRecordButton;

	public GameObject LoadingPanel;

	public Button backButton;

	public GameObject MovingLights;

	public GameObject Twinkles;

	public GameObject SmallLights;

	public GameObject Sparkles;

	public GameObject MovingLights2;

	public Image BottomFlashImage;

	public Image LeftFlashImage;

	public Image RightFlashImage;

	public Image TopFlashImage;

	private float flashSpeed = 5f;

	private Color flashColor = new Color(1f, 1f, 1f, 0.1f);

	private bool buttonClicked;

	private GenreVO genre;

	private Dictionary<int, List<MixingBoardButtonView>> buttonViewColumns;

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public Dictionary<int, List<MixingBoardButtonView>> ButtonViewColumns => buttonViewColumns;

	protected override void Awake()
	{
		base.Awake();
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	public void SetGenre(GenreVO genre)
	{
		this.genre = genre;
		SetupSoundButtons();
	}

	public void OnBackButtonClick()
	{
		dispatcher.Dispatch("BACK_CLICK_EVENT");
	}

	public void SetupSoundButtons()
	{
		buttonViewColumns = new Dictionary<int, List<MixingBoardButtonView>>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("MixerSoundButton");
		for (int i = 0; i < array.Length; i++)
		{
			MixingBoardButtonView component = array[i].GetComponent<MixingBoardButtonView>();
			component.Sound = genre.GetSoundByID(component.id);
			if (component.Sound == null)
			{
			}
			component.OnSoundButtonPressed += MixingBoardButtonView_Click_Handler;
			SaveButtonViewReference(component);
		}
	}

	public void recordButton_Click_Handler()
	{
		dispatcher.Dispatch("RECORD_CLICK_EVENT");
	}

	public void stopButton_Click_Handler()
	{
		dispatcher.Dispatch("STOP_CLICK_EVENT");
	}

	private void SaveButtonViewReference(MixingBoardButtonView buttonView)
	{
		List<MixingBoardButtonView> list = (!buttonViewColumns.ContainsKey(buttonView.groupID)) ? new List<MixingBoardButtonView>() : buttonViewColumns[buttonView.groupID];
		list.Add(buttonView);
		buttonViewColumns[buttonView.groupID] = list;
	}

	private void MixingBoardButtonView_Click_Handler(SoundVO sound)
	{
		buttonClicked = true;
		dispatcher.Dispatch("SOUND_CLICK_EVENT", sound);
	}

	private void Update()
	{
		if (buttonClicked)
		{
			BottomFlashImage.color = flashColor;
			LeftFlashImage.color = flashColor;
			RightFlashImage.color = flashColor;
			TopFlashImage.color = flashColor;
		}
		else
		{
			Image topFlashImage = TopFlashImage;
			Color color = Color.Lerp(BottomFlashImage.color, Color.clear, flashSpeed * Time.deltaTime);
			BottomFlashImage.color = color;
			color = color;
			LeftFlashImage.color = color;
			color = color;
			RightFlashImage.color = color;
			topFlashImage.color = color;
		}
		buttonClicked = false;
	}
}
