using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;

public class RadioRandomMediator : RadioCategoryMediator
{
	[Inject]
	public RadioRandomView radioRandomView
	{
		get;
		set;
	}

	public override void OnRegister()
	{
		songCollection = new RadioSongCollection();
		_radioCategory = RadioCategory.RANDOM;
		isLoadingTracks = false;
		view = radioRandomView;
		view.dispatcher.AddListener("NEXT_CLICK_EVENT", base.OnNextClick);
		view.dispatcher.AddListener("PREVIOUS_CLICK_EVENT", base.OnPreviousClick);
		view.dispatcher.AddListener("INIT_EVENT", OnInit);
		ConfigureNagivation();
	}

	protected override void LoadRadioBlock(int numSongs = 5)
	{
		if (songCollection.TotalSongs == 0)
		{
			ShowLoadingPanel();
		}
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_COMPLETE, OnLoadRadioBlockComplete);
		base.dispatcher.AddListener(SoundStudioEvent.LOAD_RADIO_BLOCK_FAIL, base.OnLoadRadioBlockFail);
		base.dispatcher.Dispatch(SoundStudioEvent.LOAD_RADIO_BLOCK, new LoadRadioBlockCommandPayload(_radioCategory, numSongs));
		isLoadingTracks = true;
	}
}
