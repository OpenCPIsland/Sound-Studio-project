using DevonLocalization.Core;
using InAppPurchases;
using InAppPurchases.Restore;
using SoundStudio.Event;
using strange.extensions.command.impl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Command
{
	public class RestorePlayerPurchasesCommand : EventCommand
	{
		public static string LocalizedIAPFilesDir = "Assets/Framework/InAppPurchases/Resources/Translations";

		public string googlePlayToken = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsaysZKi368gAdN2GXFvLxePdZNBUfX846zhZYe+A/P0bdN4HHtXJwIIMWxwnoK/V2iKqOo7StiCa2sfHFNSSMeEh2bK2Cb8PaMdlRIz1Lqjm3wIK+r/tdyHDno6bZJ7qoQ9NrK2Mx+YHci2V3HJDLlw1KBAyBEQA75B09ZDdJEwa/dMmTikC7kOMRjTn4FaVEfs0vCPGIY5EHg2sykpw03mvrRvlcpY2/tGyAdD9C56qEy0ImrGfzq27Yw00K2oP5TYNt4jFJTggi5ERMmO3CSUTGr1lwiwz71/iMrwUItSXlwJzZsuARTIOpmUXHOE4+n16DD3V83S5cGfqcGx+WQIDAQAB";

		private RestorePurchasesController RestorePurchasesInstance;

		private GameObject messageOverlayPrefab;

		private GameObject loadingPanel;

		private RectTransform canvasReference;

		private MessageDialogOverlay message;

		public override void Execute()
		{
			Retain();
			List<GameObject> list = (List<GameObject>)base.evt.data;
			GameObject gameObject = list[0];
			messageOverlayPrefab = list[1];
			if (gameObject == null)
			{
				throw new InvalidOperationException("The restore player purchase command failed as the payload was not set.");
			}
			if (RestorePurchasesInstance == null)
			{
				RestorePurchasesController component = gameObject.GetComponent<RestorePurchasesController>();
				RestorePurchasesInstance = (UnityEngine.Object.Instantiate(component) as RestorePurchasesController);
				loadingPanel = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/LoadingPanel")) as GameObject);
				if (canvasReference == null)
				{
					GameObject gameObject2 = GameObject.Find("Canvas");
					canvasReference = gameObject2.GetComponent<RectTransform>();
				}
				loadingPanel.transform.SetParent(canvasReference, worldPositionStays: false);
				RestorePurchasesController restorePurchasesInstance = RestorePurchasesInstance;
				restorePurchasesInstance.RestoreCompleted = (RestorePurchasesController.RestorePurchasesCompletedDelegate)Delegate.Combine(restorePurchasesInstance.RestoreCompleted, new RestorePurchasesController.RestorePurchasesCompletedDelegate(OnRestorePurchaseComplete));
				RestorePurchasesController restorePurchasesInstance2 = RestorePurchasesInstance;
				restorePurchasesInstance2.RestoreFailed = (RestorePurchasesController.RestorePurchasesFailedDelegate)Delegate.Combine(restorePurchasesInstance2.RestoreFailed, new RestorePurchasesController.RestorePurchasesFailedDelegate(OnRestorePurchaseFailed));
				RestorePurchasesInstance.RestorePurchases(googlePlayToken);
			}
		}

		private void OnRestorePurchaseFailed(string errorToken, int errorNumber)
		{
			ILocalizedTokenFilePath path = new ModuleTokensFilePath(LocalizedIAPFilesDir, "INAPPPURCHASES", Platform.global);
			Localizer.Instance.LoadTokensFromLocalJSON(path, ShowPopupOnFail);
		}

		private void OnRestorePurchaseComplete(SavedStorePurchasesCollection savedPurchasesCollection)
		{
			base.dispatcher.Dispatch(LoadPlayerPurchasesEvent.LOAD_PLAYER_PURCHASES);
			Cleanup();
		}

		private void ShowPopupOnFail(bool success)
		{
			Cleanup();
			GameObject gameObject = UnityEngine.Object.Instantiate(messageOverlayPrefab) as GameObject;
			message = gameObject.GetComponent<MessageDialogOverlay>();
			message.GetComponent<RectTransform>().SetParent(canvasReference, worldPositionStays: false);
			MessageDialogOverlay messageDialogOverlay = message;
			messageDialogOverlay.Closed = (MessageDialogOverlay.ClosedDelegate)Delegate.Combine(messageDialogOverlay.Closed, new MessageDialogOverlay.ClosedDelegate(OnCloseButton));
			message.ShowStatusTextFromToken("iap.commerce.error.purchaserestore.general");
		}

		private void OnCloseButton()
		{
			MessageDialogOverlay messageDialogOverlay = message;
			messageDialogOverlay.Closed = (MessageDialogOverlay.ClosedDelegate)Delegate.Remove(messageDialogOverlay.Closed, new MessageDialogOverlay.ClosedDelegate(OnCloseButton));
			UnityEngine.Object.Destroy(message.gameObject);
		}

		private void Cleanup()
		{
			RestorePurchasesController restorePurchasesInstance = RestorePurchasesInstance;
			restorePurchasesInstance.RestoreCompleted = (RestorePurchasesController.RestorePurchasesCompletedDelegate)Delegate.Remove(restorePurchasesInstance.RestoreCompleted, new RestorePurchasesController.RestorePurchasesCompletedDelegate(OnRestorePurchaseComplete));
			RestorePurchasesController restorePurchasesInstance2 = RestorePurchasesInstance;
			restorePurchasesInstance2.RestoreFailed = (RestorePurchasesController.RestorePurchasesFailedDelegate)Delegate.Remove(restorePurchasesInstance2.RestoreFailed, new RestorePurchasesController.RestorePurchasesFailedDelegate(OnRestorePurchaseFailed));
			UnityEngine.Object.Destroy(loadingPanel.gameObject);
			UnityEngine.Object.Destroy(RestorePurchasesInstance.gameObject);
		}
	}
}
