using strange.extensions.command.impl;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowAboutMembershipCommand : EventCommand
	{
		private GameObject memberBenefitsGameObject;

		private StartScreenView parentObject;

		private MembershipBenefits aboutMembershipPanel;

		public override void Execute()
		{
			Retain();
			RectTransform parent = (RectTransform)base.evt.data;
			memberBenefitsGameObject = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/MemberBenefitsPanel")) as GameObject);
			memberBenefitsGameObject.name = "AboutMembership";
			aboutMembershipPanel = memberBenefitsGameObject.GetComponent<MembershipBenefits>();
			aboutMembershipPanel.GetComponent<RectTransform>().SetParent(parent, worldPositionStays: false);
			MembershipBenefits membershipBenefits = aboutMembershipPanel;
			membershipBenefits.BackButtonPressed = (MembershipBenefits.MembershipBenefitsBackDelegate)Delegate.Combine(membershipBenefits.BackButtonPressed, new MembershipBenefits.MembershipBenefitsBackDelegate(OnAboutMembershipClosed));
			aboutMembershipPanel.ShowWebPage();
		}

		private void OnAboutMembershipClosed()
		{
			RemoveListeners();
			UnityEngine.Object.Destroy(aboutMembershipPanel.gameObject);
			Release();
		}

		private void RemoveListeners()
		{
			MembershipBenefits membershipBenefits = aboutMembershipPanel;
			membershipBenefits.BackButtonPressed = (MembershipBenefits.MembershipBenefitsBackDelegate)Delegate.Remove(membershipBenefits.BackButtonPressed, new MembershipBenefits.MembershipBenefitsBackDelegate(OnAboutMembershipClosed));
		}
	}
}
