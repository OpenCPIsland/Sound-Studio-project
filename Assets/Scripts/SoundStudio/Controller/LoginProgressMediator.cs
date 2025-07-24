using DevonLocalization.Core;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class LoginProgressMediator : EventMediator
	{
		[Inject]
		public LoginProgressView view
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

		public override void OnRegister()
		{
			base.dispatcher.AddListener(MWSEvent.GET_ACCOUNT_COMPLETED, OnGetAccountComplete);
			base.dispatcher.AddListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
			base.dispatcher.AddListener(LoginEvent.LOGIN_FAIL, OnLoginFail);
			view.ProgressText.text = Localizer.Instance.GetTokenTranslation("soundstudio.startscreen.saveddata.authenticating");
		}

		public override void OnRemove()
		{
			base.dispatcher.RemoveListener(MWSEvent.GET_ACCOUNT_COMPLETED, OnGetAccountComplete);
			base.dispatcher.RemoveListener(LoginEvent.LOGIN_SUCCESS, OnLoginSuccess);
			base.dispatcher.RemoveListener(LoginEvent.LOGIN_FAIL, OnLoginFail);
		}

		private void OnLoginSuccess()
		{
			view.ProgressText.text = Localizer.Instance.GetTokenTranslation("soundstudio.startscreen.saveddata.loading");
		}

		private void OnLoginFail()
		{
			RemoveProgressView();
		}

		private void OnGetAccountComplete()
		{
			RemoveProgressView();
		}

		private void RemoveProgressView()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
