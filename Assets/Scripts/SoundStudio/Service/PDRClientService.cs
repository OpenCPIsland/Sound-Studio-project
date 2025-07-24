using Disney.ClubPenguin.Service.PDR;
using SoundStudio.Command.PDR;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

namespace SoundStudio.Service
{
	internal class PDRClientService : IPDRClientService
	{
		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher dispatcher
		{
			get;
			set;
		}

		[Inject]
		public IPDRClient PDRClient
		{
			get;
			set;
		}

		[PostConstruct]
		public void Initialize()
		{
		}

		public void GetPaperDollImage(GetPaperDollCommandPayload payload)
		{
			PDRRequest pDRRequest = new PDRRequest();
			pDRRequest.dispatcher = dispatcher;
			pDRRequest.payload = payload;
			pDRRequest.PDRClient = PDRClient;
			UnityEngine.Debug.Log("PDRClientService.GetPaperDollImage " + payload.Swid);
			PDRClient.GetPaperDollImage(payload.Swid, payload.Size, payload.Flag, payload.Photo, payload.Language, pDRRequest.GetPaperDollImageResponseCallback);
		}
	}
}
