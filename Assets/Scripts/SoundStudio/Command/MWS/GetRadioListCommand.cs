namespace SoundStudio.Command.MWS
{
	public class GetRadioListCommand : MWSCommand
	{
		public override void Execute()
		{
			GetRadioListCommandPayload getRadioListCommandPayload = (GetRadioListCommandPayload)base.evt.data;
			switch (getRadioListCommandPayload.RadioCategory)
			{
			case RadioCategory.FRIENDS:
				break;
			case RadioCategory.RANDOM:
				base.MWSClientService.GetRandomSharedTracks(Utils.SystemLanguageToMWSLocale(), getRadioListCommandPayload.SongCount);
				break;
			case RadioCategory.NEW:
				base.MWSClientService.GetNewSharedTracks(Utils.SystemLanguageToMWSLocale(), getRadioListCommandPayload.SongCount, getRadioListCommandPayload.BeforeTrackID);
				break;
			}
		}
	}
}
