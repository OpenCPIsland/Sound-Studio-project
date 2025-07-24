using Disney.ClubPenguin.Service.PDR;
using SoundStudio.Command.PDR;
using SoundStudio.Event;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class PDRRequest
{
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	public int retries
	{
		get;
		set;
	}

	public GetPaperDollCommandPayload payload
	{
		get;
		set;
	}

	public byte[] pdrImageBytes
	{
		get;
		set;
	}

	public IPDRClient PDRClient
	{
		get;
		set;
	}

	public PDRRequest()
	{
		retries = 0;
	}

	public void GetPaperDollImageResponseCallback(IGetPaperDollImageResponse response)
	{
		UnityEngine.Debug.Log("PDRRequest.GetPaperDollImageResponseCallback response.IsError: " + response.IsError + " retries: " + retries.ToString());
		if (response.IsError && retries >= 3)
		{
			dispatcher.Dispatch(PDREvent.GET_PAPER_DOLL_FAILED);
		}
		else if (response.IsError && retries < 3)
		{
			retries++;
			PDRClient.GetPaperDollImage(payload.Swid, payload.Size, payload.Flag, payload.Photo, payload.Language, GetPaperDollImageResponseCallback);
		}
		else
		{
			pdrImageBytes = response.AvatarImageBytes;
			dispatcher.Dispatch(PDREvent.GET_PAPER_DOLL_COMPLETED, this);
		}
	}
}
