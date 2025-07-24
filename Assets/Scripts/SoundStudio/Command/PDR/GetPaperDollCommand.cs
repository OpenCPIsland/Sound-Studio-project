using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Command.PDR
{
	public class GetPaperDollCommand : EventCommand
	{
		[Inject]
		public IPDRClientService pdrClientService
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
			GetPaperDollCommandPayload payload = (GetPaperDollCommandPayload)base.evt.data;
			base.dispatcher.AddListener(PDREvent.GET_PAPER_DOLL_COMPLETED, OnComplete);
			pdrClientService.GetPaperDollImage(payload);
		}

		private void OnComplete(IEvent evt)
		{
			try
			{
				base.dispatcher.RemoveListener(PDREvent.GET_PAPER_DOLL_COMPLETED, OnComplete);
			}
			finally
			{
				Release();
			}
		}
	}
}
