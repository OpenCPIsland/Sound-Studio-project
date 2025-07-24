using System;
using UnityEngine;

namespace SoundStudio.Command.MWS
{
	internal class CheckOnlineCommand : MWSCommand
	{
		public override void Execute()
		{
			switch (Application.internetReachability)
			{
			case NetworkReachability.NotReachable:
				Fail();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case NetworkReachability.ReachableViaCarrierDataNetwork:
			case NetworkReachability.ReachableViaLocalAreaNetwork:
				break;
			}
			Release();
		}
	}
}
