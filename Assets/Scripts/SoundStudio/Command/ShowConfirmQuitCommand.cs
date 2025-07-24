using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowConfirmQuitCommand : EventCommand
	{
		private GameObject quitConfirmGameObject;

		private QuitConfirmView quitConfirmView;

		[Inject(ContextKeys.CONTEXT_DISPATCHER)]
		public IEventDispatcher ContextDispatcher
		{
			get;
			set;
		}

		public override void Execute()
		{
			if (GameObject.Find("QuitConfirm") == null && GameObject.Find("AgeGate") == null && GameObject.Find("Login") == null && GameObject.Find("AboutMembership") == null && GameObject.Find("MobilePurchasesAndData") == null && GameObject.Find("Store") == null && GameObject.Find("RecommendedUpdate") == null)
			{
				Retain();
				RectTransform parent = (RectTransform)base.evt.data;
				quitConfirmGameObject = (Object.Instantiate(Resources.Load("Prefabs/QuitConfirmPrefab")) as GameObject);
				quitConfirmGameObject.name = "QuitConfirm";
				quitConfirmGameObject.transform.SetParent(parent, worldPositionStays: false);
				quitConfirmView = quitConfirmGameObject.GetComponent<QuitConfirmView>();
				ContextDispatcher.AddListener("confirm_quit", OnConfirmQuit);
				ContextDispatcher.AddListener("cancel_quit", OnCancelQuit);
				ContextDispatcher.AddListener("back_click_event", OnCancelQuit);
			}
		}

		private void OnConfirmQuit()
		{
			RemoveListeners();
			UnityEngine.Object.Destroy(quitConfirmView.gameObject);
			Release();
			Application.Quit();
		}

		private void OnCancelQuit()
		{
			RemoveListeners();
			UnityEngine.Object.Destroy(quitConfirmView.gameObject);
			Release();
		}

		private void RemoveListeners()
		{
			ContextDispatcher.RemoveListener("confirm_quit", OnConfirmQuit);
			ContextDispatcher.RemoveListener("cancel_quit", OnCancelQuit);
			ContextDispatcher.RemoveListener("back_click_event", OnCancelQuit);
		}
	}
}
