using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class SongStopCommand : EventCommand
	{
		[Inject]
		public SongPlayer songPlayer
		{
			get;
			set;
		}

		public override void Execute()
		{
			songPlayer.Stop();
		}
	}
}
