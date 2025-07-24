using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class InitialSetupCommand : EventCommand
	{
		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public RetrySynchronizationService RetrySynchronizationService
		{
			get;
			set;
		}

		public override void Execute()
		{
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.StartScreen);
			base.dispatcher.Dispatch(SoundStudioEvent.SETUP_COMPLETE);
			RetrySynchronizationService.Start();
			MoPubAndroid.reportApplicationOpen();
		}
	}
}
