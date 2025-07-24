using Disney.ClubPenguin.Login.Authentication;
using Disney.ClubPenguin.Login.BI;
using Disney.ClubPenguin.Service.MWS;
using Disney.ClubPenguin.Service.PDR;
using Disney.HTTP.Client;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class AutoLoginCommand : EventCommand
	{
		private DoAutoLoginCMD autoLoginCMD;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public StrikeModules strikeModules
		{
			get;
			set;
		}

		[Inject]
		public IMWSClient mwsClient
		{
			get;
			set;
		}

		[Inject]
		public IPDRClient pdrClient
		{
			get;
			set;
		}

		public override void Execute()
		{
			MonoBehaviour timeoutCoRoutineBehaviour = base.evt.data as MonoBehaviour;
			LoginBIUtils loginBIUtils = new LoginBIUtils();
			loginBIUtils.ContextName = "SoundStudio";
			autoLoginCMD = new DoAutoLoginCMD("SoundStudio", "1.2", mwsClient, pdrClient, loginBIUtils, timeoutCoRoutineBehaviour);
			autoLoginCMD.LoginSucceeded += AutoLogin_Success_Handler;
			autoLoginCMD.LoginRequestSent += OnLoginStarted;
			autoLoginCMD.LoginFailed += AutoLogin_Failed_Handler;
			autoLoginCMD.Execute();
		}

		private void AutoLogin_Success_Handler(IGetAuthTokenResponse response, string username, string password)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.CHANGE_USER, response.AuthData);
			RemoveListeners();
			base.dispatcher.Dispatch(LoginEvent.LOGIN_SUCCESS);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_PLAYER);
			Release();
		}

		private void AutoLogin_Failed_Handler(IHTTPResponse response)
		{
			base.dispatcher.Dispatch(SoundStudioEvent.CHANGE_USER);
			RemoveListeners();
			base.dispatcher.Dispatch(LoginEvent.LOGIN_FAIL);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_PLAYER);
			Release();
		}

		public void OnLoginStarted()
		{
			GameObject data = GameObject.Find("Canvas");
			base.dispatcher.Dispatch(SoundStudioEvent.CREATE_LOGIN_PROGRESS, data);
		}

		private void RemoveListeners()
		{
			autoLoginCMD.LoginSucceeded -= AutoLogin_Success_Handler;
			autoLoginCMD.LoginRequestSent -= OnLoginStarted;
			autoLoginCMD.LoginFailed -= AutoLogin_Failed_Handler;
		}
	}
}
