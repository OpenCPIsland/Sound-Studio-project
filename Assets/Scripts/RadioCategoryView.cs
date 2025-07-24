using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class RadioCategoryView : EventView
{
	internal const string NEXT_CLICK_EVENT = "NEXT_CLICK_EVENT";

	internal const string PREVIOUS_CLICK_EVENT = "PREVIOUS_CLICK_EVENT";

	internal const string INIT_EVENT = "INIT_EVENT";

	internal const string CATEGORY_CHANGE_EVENT = "CATEGORY_CHANGE_EVENT";

	internal const string NO_CONNECTION_EVENT = "NO_CONNECTION_EVENT";

	internal const string GET_CONNECTION_EVENT = "GET_CONNECTION_EVENT";

	public GameObject songContainerObject;

	public GameObject canvasObject;

	public GameObject nextButton;

	public GameObject previousButton;

	private GameObject popup;

	private GameObject loadingPanel;

	public bool isInTransition;

	public void ShowPopup(string popupPath, RadioError radioError = RadioError.AllowContinue)
	{
		HidePopup();
		if (radioError == RadioError.Stop)
		{
			previousButton.SetActive(value: false);
			nextButton.SetActive(value: false);
			foreach (Transform item in songContainerObject.transform)
			{
				item.gameObject.SetActive(value: false);
			}
		}
		popup = (Object.Instantiate(Resources.Load(popupPath)) as GameObject);
		popup.transform.SetParent(songContainerObject.transform, worldPositionStays: false);
	}

	private void SetControlsActive(bool active = true)
	{
		previousButton.SetActive(active);
		nextButton.SetActive(active);
		foreach (Transform item in songContainerObject.transform)
		{
			item.gameObject.SetActive(active);
		}
	}

	public void HidePopup()
	{
		if (popup != null)
		{
			UnityEngine.Object.Destroy(popup);
			SetControlsActive();
		}
		RemoveLoadingPanel();
	}

	public void OnNextClick()
	{
		base.dispatcher.Dispatch("NEXT_CLICK_EVENT");
	}

	public void OnPreviousClick()
	{
		base.dispatcher.Dispatch("PREVIOUS_CLICK_EVENT");
	}

	public void OnInit()
	{
		base.dispatcher.Dispatch("INIT_EVENT");
	}

	public void ShowLoadingPanel()
	{
		if (loadingPanel == null)
		{
			loadingPanel = (Object.Instantiate(Resources.Load("Prefabs/LoadingPanel")) as GameObject);
			loadingPanel.transform.SetParent(canvasObject.transform, worldPositionStays: false);
		}
		loadingPanel.SetActive(value: true);
	}

	public void RemoveLoadingPanel()
	{
		if (loadingPanel != null)
		{
			loadingPanel.SetActive(value: false);
		}
	}

	public void SetNagivationArrowState(bool isLoading)
	{
		Color color = new Color(1f, 1f, 1f, 1f);
		if (isLoading)
		{
			color = new Color(1f, 1f, 1f, 0.2f);
		}
		nextButton.GetComponent<Image>().color = color;
		previousButton.GetComponent<Image>().color = color;
	}
}
