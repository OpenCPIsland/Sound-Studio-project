using InAppPurchases;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;
using System.Collections.Generic;

namespace SoundStudio.Command
{
	public class LoadPlayerPurchasesCommand : EventCommand
	{
		private ICollection<string> purchasedItems;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject]
		public IMWSClientService MWSClient
		{
			get;
			set;
		}

		public override void Execute()
		{
			SavedStorePurchasesCollection savedStorePurchasesCollection = new SavedStorePurchasesCollection();
			savedStorePurchasesCollection.LoadFromDisk();
			purchasedItems = savedStorePurchasesCollection.GetAllPurchasedProdIds(includeMwsUnregistered: true);
			if (purchasedItems.Count > 0)
			{
				foreach (GenreVO item in application.genreData.Collection)
				{
					if (purchasedItems.Contains(item.productIdentifier))
					{
						application.currentPlayer.AddGenre(item.id);
					}
				}
			}
			if (application.currentPlayer.AccountStatus != 0)
			{
				MWSClient.GetIAPPurchasesForPlayer();
			}
		}
	}
}
