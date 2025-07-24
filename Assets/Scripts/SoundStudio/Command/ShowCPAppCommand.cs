using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class ShowCPAppCommand : EventCommand
	{
		public override void Execute()
		{
			ApplicationHelper.OpenCPApp();
		}
	}
}
