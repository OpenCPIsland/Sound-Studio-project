using DevonLocalization.Core;
using Disney.ClubPenguin.WebPageViewer;
using SoundStudio.Command;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class GenreSelectMediator : EventMediator
	{
		private GenreVO selectedGenre;

		[Inject]
		public GenreSelectView view
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState applicationState
		{
			get;
			set;
		}

		public override void OnRegister()
		{
			view.dispatcher.AddListener("GENRE_CLICK_EVENT", OnGenreClicked);
			view.dispatcher.AddListener("BACK_CLICK_EVENT", OnBackClicked);
			view.dispatcher.AddListener("TERMS_CLICK_EVENT", OnTermsClick);
			LoadGenres();
			base.dispatcher.Dispatch(SoundStudioEvent.RESUME_DMO_ANALYTICS);
		}

		private void LoadGenres()
		{
			foreach (GenreVO item in applicationState.genreData.Collection)
			{
				view.loadGenre(item);
			}
		}

		public void OnTermsClick()
		{
			base.dispatcher.Dispatch(ShowProtectedWebPageEvent.SHOW_PROTECTED_WEB_PAGE, new ShowProtectedWebPagePayload(view.canvas.transform as RectTransform, Localizer.Instance.GetTokenTranslation("soundstudio.settings.terms")));
		}

		public override void OnRemove()
		{
			view.dispatcher.RemoveListener("GENRE_CLICK_EVENT", OnGenreClicked);
			view.dispatcher.RemoveListener("BACK_CLICK_EVENT", OnBackClicked);
			view.dispatcher.RemoveListener("TERMS_CLICK_EVENT", OnTermsClick);
		}

		private void OnGenreClicked(IEvent evt)
		{
			selectedGenre = (GenreVO)evt.data;
			if (selectedGenre != null)
			{
				if (!selectedGenre.Equals(applicationState.CurrentGenre))
				{
					AudioClipPlayer.ClearMemoryAsNeeded();
				}
				AudioClip data = Resources.Load(selectedGenre.buttonSound) as AudioClip;
				base.dispatcher.Dispatch(SFXEvent.SFX_WITH_PAYLOAD_IN_EVENT, data);
				base.dispatcher.Dispatch(SelectGenreEvent.SELECT_GENRE, selectedGenre);
			}
		}

		private void OnBackClicked()
		{
			base.dispatcher.Dispatch(SFXEvent.SFX_CLICK_BACK);
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.StartScreen);
		}

		public void DisplayURLInPopUp(string url)
		{
			InitializeInlineWebViewer();
			if (view.nativeWebPageinstance != null)
			{
				view.nativeWebPageinstance.DisplayPage(url);
			}
		}

		private void InitializeInlineWebViewer()
		{
			if (view.nativeWebPageinstance == null)
			{
				view.nativeWebPageinstance = (UnityEngine.Object.Instantiate(view.nativeWebPagePrefab) as DisplayNativeWebPage);
				view.nativeWebPageinstance.GetComponent<RectTransform>().SetParent(view.canvas, worldPositionStays: false);
				DisplayNativeWebPage nativeWebPageinstance = view.nativeWebPageinstance;
				nativeWebPageinstance.Closed = (DisplayNativeWebPage.ClosedDelegate)Delegate.Combine(nativeWebPageinstance.Closed, new DisplayNativeWebPage.ClosedDelegate(OnWebPageClosed));
			}
		}

		public void OnWebPageClosed()
		{
			if (view.nativeWebPageinstance != null)
			{
				DisplayNativeWebPage nativeWebPageinstance = view.nativeWebPageinstance;
				nativeWebPageinstance.Closed = (DisplayNativeWebPage.ClosedDelegate)Delegate.Remove(nativeWebPageinstance.Closed, new DisplayNativeWebPage.ClosedDelegate(OnWebPageClosed));
				UnityEngine.Object.Destroy(view.nativeWebPageinstance.gameObject);
			}
		}
	}
}
