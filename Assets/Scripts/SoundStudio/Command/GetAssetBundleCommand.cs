using Disney.ClubPenguin.CPModuleUtils;
using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using UnityEngine.UI;

namespace SoundStudio.Command
{
	internal class GetAssetBundleCommand : EventCommand
	{
		private GetAssetBundlePayloadVO getAssetBundlePayloadVO;

		private GameObject theProgressWindow;

		[Inject]
		public ApplicationState applicationState
		{
			get;
			set;
		}

		[Inject]
		public AssetBundleDownloadService AssetBundleDownloadService
		{
			get;
			set;
		}

		public override void Execute()
		{
			getAssetBundlePayloadVO = (base.evt.data as GetAssetBundlePayloadVO);
			if (getAssetBundlePayloadVO != null)
			{
				Retain();
				base.dispatcher.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_STARTED, OnAssetBundleStarted);
				base.dispatcher.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_SUCCESS, OnAssetBundleSuccess);
				base.dispatcher.AddListener(SoundStudioEvent.GET_ASSET_BUNDLE_FAILED, OnAssetBundleFailed);
				AssetBundleDownloadService.DownloadBundle(getAssetBundlePayloadVO.BundleURL, getAssetBundlePayloadVO.Version);
			}
		}

		private void CleanUpListeners()
		{
			Release();
			base.dispatcher.RemoveListener(SoundStudioEvent.GET_ASSET_BUNDLE_STARTED, OnAssetBundleStarted);
			base.dispatcher.RemoveListener(SoundStudioEvent.GET_ASSET_BUNDLE_SUCCESS, OnAssetBundleSuccess);
			base.dispatcher.RemoveListener(SoundStudioEvent.GET_ASSET_BUNDLE_FAILED, OnAssetBundleFailed);
			if (theProgressWindow != null)
			{
				UnityEngine.Object.Destroy(theProgressWindow);
			}
		}

		private void OnAssetBundleFailed(IEvent payload)
		{
			CleanUpListeners();
			RaycastInputBlocker.RestoreAllRayCastersInput();
			ShowFailDialogBox();
		}

		private void ShowFailDialogBox()
		{
			if (getAssetBundlePayloadVO.ParentGameObject != null)
			{
				GameObject original = Resources.Load("Prefabs/NeedWifiConnectionPanel") as GameObject;
				GameObject gameObject = Object.Instantiate(original, Vector3.zero, new Quaternion(0f, 0f, 0f, 0f)) as GameObject;
				gameObject.transform.SetParent(getAssetBundlePayloadVO.ParentGameObject.transform, worldPositionStays: false);
			}
		}

		private void OnAssetBundleSuccess(IEvent payload)
		{
			applicationState.AssetBundle = (payload.data as AssetBundle);
			RaycastInputBlocker.RestoreAllRayCastersInput();
			CleanUpListeners();
		}

		private void OnAssetBundleStarted(IEvent payload)
		{
			if (getAssetBundlePayloadVO.ParentGameObject != null)
			{
				RaycastInputBlocker.BlockRayCasterInputsArray(Object.FindObjectsOfType(typeof(GraphicRaycaster)) as GraphicRaycaster[]);
				theProgressWindow = (Object.Instantiate(LoadPrefab(), Vector3.zero, new Quaternion(0f, 0f, 0f, 0f)) as GameObject);
				theProgressWindow.transform.SetParent(getAssetBundlePayloadVO.ParentGameObject.transform, worldPositionStays: false);
			}
		}

		private GameObject LoadPrefab()
		{
			return Resources.Load("Prefabs/AssetLoadingPanel") as GameObject;
		}
	}
}
