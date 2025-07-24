using DevonLocalization.Core;
using SoundStudio.Model;
using strange.extensions.command.impl;
using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowMopubAdCommand : EventCommand
	{
		private string pendingAdUnitID;

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		public override void Execute()
		{
			if (isDisplayingAd())
			{
				pendingAdUnitID = GetAdUnitID();
				if (pendingAdUnitID != null)
				{
					RequestInterstitial(pendingAdUnitID);
				}
			}
		}

		private bool isDisplayingAd()
		{
			if (application.currentPlayer.AccountStatus == MembershipStatus.MEMBER)
			{
				return false;
			}
			bool result = false;
			int @int = PlayerPrefs.GetInt("MOPUB_AD_COUNT", 5);
			@int++;
			if (@int >= 5)
			{
				result = true;
				@int = 0;
			}
			PlayerPrefs.SetInt("MOPUB_AD_COUNT", @int);
			return result;
		}

		private string GetAdUnitID()
		{
			string result = MoPubAdUnits.GOOGLE_GENRE_PHONE;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = (isTablet() ? MoPubAdUnits.GOOGLE_GENRE_TABLET : MoPubAdUnits.GOOGLE_GENRE_PHONE);
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = (isTablet() ? MoPubAdUnits.APPLE_GENRE_TABLET : MoPubAdUnits.APPLE_GENRE_PHONE);
			}
			return result;
		}

		private bool isTablet()
		{
			float dpi = Screen.dpi;
			bool flag = false;
			if (dpi == 0f)
			{
				return false;
			}
			if ((float)Screen.width / dpi > 6f)
			{
				return true;
			}
			return false;
		}

		private void RequestInterstitial(string adUnitID)
		{
			AddInterstitialListeners();
			string keywords = GetLanguageKeyword() + "," + GetAgeKeyword() + "," + GetPayKeyword() + "," + GetRotationKeyword() + "," + GetDeviceKeyword();
			MoPubHelper.RequestInterstitialWithKeywords(adUnitID, keywords);
		}

		private string GetLanguageKeyword()
		{
			return "language:" + LocalizationLanguage.GetLanguageString(Localizer.Instance.Language);
		}

		private string GetAgeKeyword()
		{
			return "DAS_rp_age:0";
		}

		private string GetPayKeyword()
		{
			int num = 0;
			foreach (GenreVO item in application.genreData.Collection)
			{
				if (application.currentPlayer.HasGenre(item.id))
				{
					num = 1;
				}
			}
			if (application.currentPlayer.AccountStatus == MembershipStatus.MEMBER)
			{
				num = 1;
			}
			return "DAS_rp_mon:" + num.ToString();
		}

		private string GetRotationKeyword()
		{
			return "DAS_rp_rot:1";
		}

		private string GetDeviceKeyword()
		{
			string deviceModel = SystemInfo.deviceModel;
			deviceModel = ((deviceModel == null) ? "Unknown" : deviceModel.Replace(',', '_'));
			return "DAS_rp_dev:" + deviceModel;
		}

		private void AddInterstitialListeners()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				MoPubAndroidManager.onInterstitialLoadedEvent += OnLoadInterstitialComplete;
				MoPubAndroidManager.onInterstitialFailedEvent += OnLoadInterstitialFail;
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}

		private void OnLoadInterstitialComplete()
		{
			RemoveInterstitialListeners();
			MoPubHelper.ShowInterstitial(pendingAdUnitID);
			pendingAdUnitID = null;
		}

		private void OnLoadInterstitialFail()
		{
			RemoveInterstitialListeners();
		}

		private void RemoveInterstitialListeners()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				MoPubAndroidManager.onInterstitialLoadedEvent -= OnLoadInterstitialComplete;
				MoPubAndroidManager.onInterstitialFailedEvent -= OnLoadInterstitialFail;
			}
			else if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
			}
		}
	}
}
