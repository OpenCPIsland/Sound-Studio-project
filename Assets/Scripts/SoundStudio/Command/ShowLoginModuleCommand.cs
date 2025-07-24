using Disney.ClubPenguin.CPModuleUtils;
using Disney.ClubPenguin.Login.Authentication;
using Disney.ClubPenguin.Service.MWS;
using Disney.HTTP.Client;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoundStudio.Command
{
	public class ShowLoginModuleCommand : EventCommand
	{
		private LoginContext loginContext;

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

		public override void Execute()
		{
			Retain();
			List<object> list = (List<object>)base.evt.data;
			if (list == null)
			{
				throw new InvalidOperationException("The show login command failed as the payload was not set.");
			}
			RectTransform rectTransform = list[1] as RectTransform;
			RaycastInputBlocker.Clear();
			RaycastInputBlocker.AllowGraphicRayCaster(RaycastInputBlocker.GetFirstRayCasterInHierarchy(rectTransform));
			RaycastInputBlocker.BlockRayCasterInputsArray(UnityEngine.Object.FindObjectsOfType(typeof(GraphicRaycaster)) as GraphicRaycaster[]);
			if (rectTransform != null)
			{
				loginContext = (UnityEngine.Object.Instantiate(strikeModules.loginContextPrefab) as LoginContext);
				loginContext.name = "Login";
				LoadMemberBenefitPrefabs(loginContext);
				if (loginContext != null)
				{
					LoginContext obj = loginContext;
					obj.LoginSucceeded = (LoginContext.LoginSucceededDelegate)Delegate.Combine(obj.LoginSucceeded, new LoginContext.LoginSucceededDelegate(LoginContext_Success_Handler));
					LoginContext obj2 = loginContext;
					obj2.LoginClosed = (LoginContext.LoginClosedDelegate)Delegate.Combine(obj2.LoginClosed, new LoginContext.LoginClosedDelegate(LoginContext_Close_Handler));
					loginContext.LoginFailed += LoginContext_Failed_Handler;
					loginContext.AppID = "SoundStudio";
					loginContext.AppVersion = "1.2";
					loginContext.GetComponent<RectTransform>().SetParent(rectTransform, worldPositionStays: false);
					loginContext.DetermineAndShowLoginState();
				}
			}
		}

		private void LoadMemberBenefitPrefabs(LoginContext loginContext)
		{
			loginContext.MemberBenefitViewPrefabs = new RectTransform[3]
			{
				(Resources.Load("Prefabs/SSMemberBenefit1") as GameObject).GetComponent<RectTransform>(),
				(Resources.Load("Prefabs/SSMemberBenefit2") as GameObject).GetComponent<RectTransform>(),
				(Resources.Load("Prefabs/SSMemberBenefit3") as GameObject).GetComponent<RectTransform>()
			};
		}

		private void LoginContext_Success_Handler(IGetAuthTokenResponse response, string username, string password)
		{
			RaycastInputBlocker.RestoreAllRayCastersInput();
			base.dispatcher.Dispatch(SoundStudioEvent.CHANGE_USER, response.AuthData);
			base.dispatcher.Dispatch(LoginEvent.LOGIN_SUCCESS);
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_PLAYER);
			base.dispatcher.Dispatch(SoundStudioEvent.CREATE_LOGIN_PROGRESS, GameObject.Find("Canvas"));
			RemoveListeners();
			Release();
		}

		private void LoginContext_Close_Handler()
		{
			RaycastInputBlocker.RestoreAllRayCastersInput();
			base.dispatcher.Dispatch(LoginEvent.LOGIN_CANCEL);
			RemoveListeners();
			Release();
		}

		private void LoginContext_Failed_Handler(IHTTPResponse response)
		{
			RaycastInputBlocker.RestoreAllRayCastersInput();
			base.dispatcher.Dispatch(LoginEvent.LOGIN_FAIL);
		}

		private void RemoveListeners()
		{
			LoginContext obj = loginContext;
			obj.LoginSucceeded = (LoginContext.LoginSucceededDelegate)Delegate.Remove(obj.LoginSucceeded, new LoginContext.LoginSucceededDelegate(LoginContext_Success_Handler));
			LoginContext obj2 = loginContext;
			obj2.LoginClosed = (LoginContext.LoginClosedDelegate)Delegate.Remove(obj2.LoginClosed, new LoginContext.LoginClosedDelegate(LoginContext_Close_Handler));
			loginContext.LoginFailed -= LoginContext_Failed_Handler;
		}

		private MembershipStatus AccountType_To_MembershupStatus(string accountType)
		{
			return MembershipStatus.MEMBER;
		}
	}
}
