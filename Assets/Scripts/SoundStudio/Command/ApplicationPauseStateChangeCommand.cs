using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	internal class ApplicationPauseStateChangeCommand : EventCommand
	{
		public override void Execute()
		{
			if (base.evt.data != null)
			{
				PauseState pauseState = (PauseState)(int)base.evt.data;
				PauseState pauseState2 = pauseState;
				if (pauseState2 == PauseState.UnPaused)
				{
				}
			}
		}
	}
}
