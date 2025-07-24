using Disney.ClubPenguin.CPModuleUtils;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class MembershipPromptView : View
{
	public GameObject PopText;

	public GameObject DanceText;

	public GameObject RockText;

	public GameObject DubstepText;

	public GameObject SpookyText;

	public GameObject CadenceText;

	public Button closeButton;

	protected override void Awake()
	{
		closeButton.gameObject.SetActive(value: true);
		HardwareBackButtonDispatcher.SetTargetClickHandler(closeButton, visible: false);
	}

	public void SetGenreText(int genreID)
	{
		DisableAllText();
		switch (genreID)
		{
		case 0:
			PopText.SetActive(value: true);
			break;
		case 2:
			DanceText.SetActive(value: true);
			break;
		case 1:
			RockText.SetActive(value: true);
			break;
		case 3:
			DubstepText.SetActive(value: true);
			break;
		case 4:
			SpookyText.SetActive(value: true);
			break;
		case 5:
			CadenceText.SetActive(value: true);
			break;
		}
	}

	private void DisableAllText()
	{
		PopText.SetActive(value: false);
		DanceText.SetActive(value: false);
		RockText.SetActive(value: false);
		DubstepText.SetActive(value: false);
		SpookyText.SetActive(value: false);
		CadenceText.SetActive(value: false);
	}

	public void OnPlayButtonClick()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
