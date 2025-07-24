using Disney.ClubPenguin.Service.PDR;
using SoundStudio.Command.PDR;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Service
{
	public interface IPDRClientService
	{
		IEventDispatcher dispatcher
		{
			get;
			set;
		}

		IPDRClient PDRClient
		{
			get;
			set;
		}

		void GetPaperDollImage(GetPaperDollCommandPayload payload);
	}
}
