using InAppPurchases;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowStoreCommand : EventCommand
	{
		private GameObject iapGameObject;

		private IAPContext iapContext;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			Retain();
			RectTransform parent = (RectTransform)base.evt.data;
			iapGameObject = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/IAPContext")) as GameObject);
			iapGameObject.name = "Store";
			iapContext = iapGameObject.GetComponent<IAPContext>();
			iapContext.GetComponent<RectTransform>().SetParent(parent, worldPositionStays: false);
			iapContext.AppID = "SoundStudio";
			iapContext.googlePlayToken = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAsaysZKi368gAdN2GXFvLxePdZNBUfX846zhZYe+A/P0bdN4HHtXJwIIMWxwnoK/V2iKqOo7StiCa2sfHFNSSMeEh2bK2Cb8PaMdlRIz1Lqjm3wIK+r/tdyHDno6bZJ7qoQ9NrK2Mx+YHci2V3HJDLlw1KBAyBEQA75B09ZDdJEwa/dMmTikC7kOMRjTn4FaVEfs0vCPGIY5EHg2sykpw03mvrRvlcpY2/tGyAdD9C56qEy0ImrGfzq27Yw00K2oP5TYNt4jFJTggi5ERMmO3CSUTGr1lwiwz71/iMrwUItSXlwJzZsuARTIOpmUXHOE4+n16DD3V83S5cGfqcGx+WQIDAQAB";
			MemberBenefitsClickedHandler component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/SoundStudioMemberBenefitsBG")) as GameObject).GetComponent<MemberBenefitsClickedHandler>();
			iapContext.SetMemberBenefitsBackground(component);
			IAPContext iAPContext = iapContext;
			iAPContext.IAPContextClosed = (IAPContext.IAPContextClosedDelegate)Delegate.Combine(iAPContext.IAPContextClosed, new IAPContext.IAPContextClosedDelegate(OnStoreClosed));
			iapContext.PlayerID = application.currentPlayer.ID;
		}

		private void OnStoreClosed(HashSet<string> ownedItemsSKUs)
		{
			RemoveListeners();
			UnityEngine.Object.Destroy(iapContext.gameObject);
			List<string> list = new List<string>(ownedItemsSKUs);
			foreach (GenreVO item in application.genreData.Collection)
			{
				if (list.Contains(item.productIdentifier))
				{
					application.currentPlayer.AddGenre(item.id);
				}
			}
			base.dispatcher.Dispatch(SoundStudioEvent.PURCHASES_REFRESHED);
			Release();
		}

		private void RemoveListeners()
		{
			IAPContext iAPContext = iapContext;
			iAPContext.IAPContextClosed = (IAPContext.IAPContextClosedDelegate)Delegate.Remove(iAPContext.IAPContextClosed, new IAPContext.IAPContextClosedDelegate(OnStoreClosed));
		}
	}
}
