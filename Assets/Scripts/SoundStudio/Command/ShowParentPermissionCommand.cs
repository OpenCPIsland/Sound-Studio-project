using Disney.ClubPenguin.CPModuleUtils;
using Disney.ClubPenguin.ParentPermission;
using SoundStudio.Event;
using strange.extensions.command.impl;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowParentPermissionCommand : EventCommand
	{
		private GameObject parentPermissionGameObject;

		private ParentPermissionController parentPermissionInstance;

		public override void Execute()
		{
			Retain();
			RectTransform rectTransform = null;
			if (base.evt.data is RectTransform)
			{
				rectTransform = (RectTransform)base.evt.data;
			}
			else
			{
				if (!(base.evt.data is ShowProtectedWebPagePayload))
				{
					throw new InvalidCastException("Event payload must either be a RectTransform or ShowProtectedWebPagePayload if chaining URL data.");
				}
				rectTransform = ((ShowProtectedWebPagePayload)base.evt.data).ParentPermissionContainer;
			}
			parentPermissionGameObject = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/ParentGate")) as GameObject);
			parentPermissionGameObject.name = "AgeGate";
			parentPermissionInstance = parentPermissionGameObject.GetComponent<ParentPermissionController>();
			parentPermissionInstance.GetComponent<RectTransform>().SetParent(rectTransform, worldPositionStays: false);
			parentPermissionInstance.onFailClose += OnParentPermissionFail;
			parentPermissionInstance.onSuccess += OnParentPermissionSuccess;
			HardwareBackButtonDispatcher.SetTargetClickHandler(parentPermissionInstance.CloseButton, visible: false);
		}

		private void OnParentPermissionSuccess(int value)
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
			base.dispatcher.Dispatch(SoundStudioEvent.ON_AGE_GATE_SUCCESS, value);
			CleanupParentPermissions();
			Release();
		}

		private void OnParentPermissionFail(int value)
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
			base.dispatcher.Dispatch(SoundStudioEvent.ON_AGE_GATE_FAILED, value);
			CleanupParentPermissions();
			Fail();
		}

		private void CleanupParentPermissions()
		{
			parentPermissionInstance.onFailClose -= OnParentPermissionFail;
			parentPermissionInstance.onSuccess -= OnParentPermissionSuccess;
			UnityEngine.Object.Destroy(parentPermissionInstance.gameObject);
		}
	}
}
