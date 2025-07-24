using strange.extensions.command.impl;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	internal class SFXPlayCommand : EventCommand
	{
		public override void Execute()
		{
			switch ((int)base.evt.type)
			{
			case 0:
				AudioClipPlayer.PlaySoundClip(Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_Back") as AudioClip, MuteState.OFF, LoopState.OFF);
				break;
			case 1:
				AudioClipPlayer.PlaySoundClip(Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_Error") as AudioClip, MuteState.OFF, LoopState.OFF);
				break;
			case 2:
				AudioClipPlayer.PlaySoundClip(Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_SELECT") as AudioClip, MuteState.OFF, LoopState.OFF);
				break;
			case 3:
				AudioClipPlayer.PlaySoundClip(Resources.Load("Audio/SFX/SFX_App_SoundStudio_UI_Toggle") as AudioClip, MuteState.OFF, LoopState.OFF);
				break;
			case 4:
			{
				AudioClip clip = base.evt.data as AudioClip;
				AudioClipPlayer.PlaySoundClip(clip, MuteState.OFF, LoopState.OFF);
				break;
			}
			default:
				throw new NotImplementedException("Not yet implemented");
			}
		}
	}
}
