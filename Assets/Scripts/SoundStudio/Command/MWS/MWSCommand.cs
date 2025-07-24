using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;

namespace SoundStudio.Command.MWS
{
	public class MWSCommand : EventCommand
	{
		[Inject]
		public IMWSClientService MWSClientService
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState ApplicationState
		{
			get;
			set;
		}

		protected void BeforeExecute()
		{
			Retain();
		}

		public override void Execute() {
		}
	}
}
