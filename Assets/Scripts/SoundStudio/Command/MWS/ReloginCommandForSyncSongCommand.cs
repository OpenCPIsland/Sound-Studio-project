using SoundStudio.Event;
using SoundStudio.Model;
using System;

namespace SoundStudio.Command.MWS
{
	internal class ReloginCommandForSyncSongCommand : MWSCommand
	{
		private SongSyncVO songSyncVo;

		public override void Execute()
		{
			Retain();
			base.dispatcher.AddListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
			base.dispatcher.AddListener(LoginEvent.LOGIN_FAIL, OnLoginFail);
			songSyncVo = (base.evt.data as SongSyncVO);
			if (songSyncVo == null)
			{
				throw new ArgumentNullException("SongSyncVO cannot be null");
			}
			base.MWSClientService.LoginRetry(base.ApplicationState.currentPlayer.Username, base.ApplicationState.currentPlayer.Password);
		}

		private void RemoveListeners()
		{
			base.dispatcher.RemoveListener(LoginEvent.LOGIN_FAIL, OnLoginFail);
			base.dispatcher.RemoveListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
		}

		public void OnLoginFail()
		{
			RemoveListeners();
			Fail();
			base.dispatcher.Dispatch(SoundStudioEvent.RELOGIN_FOR_SAVE_FAILED);
			Release();
		}

		public void OnLoginSuccess()
		{
			RemoveListeners();
			base.dispatcher.Dispatch(SongSyncEvent.SONG_SYNC, songSyncVo);
			Release();
		}
	}
}
