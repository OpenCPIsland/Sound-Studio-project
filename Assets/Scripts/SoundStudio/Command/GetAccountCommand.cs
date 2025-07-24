using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Command
{
	public class GetAccountCommand : EventCommand
	{
		[Inject]
		public IMWSClientService mwsClientService
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			Retain();
			base.dispatcher.AddListener(MWSEvent.GET_ACCOUNT_COMPLETED, OnComplete);
			mwsClientService.GetAccountData();
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				base.dispatcher.RemoveListener(MWSEvent.GET_ACCOUNT_COMPLETED, OnComplete);
			}
			finally
			{
				Release();
			}
		}
	}
}
