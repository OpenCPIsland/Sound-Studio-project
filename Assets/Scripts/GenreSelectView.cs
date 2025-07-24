using DevonLocalization;
using Disney.ClubPenguin.CPModuleUtils;
using Disney.ClubPenguin.WebPageViewer;
using SoundStudio.Model;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenreSelectView : View
{
	internal const string BACK_CLICK_EVENT = "BACK_CLICK_EVENT";

	internal const string GENRE_CLICK_EVENT = "GENRE_CLICK_EVENT";

	internal const string TERMS_CLICK_EVENT = "TERMS_CLICK_EVENT";

	private const string GENRE_NAME_SPOOKY = "Spooky";

	private const string GENRE_NAME_DANCE = "Dance";

	private const string GENRE_NAME_ROCK = "Rock";

	private const string GENRE_NAME_POP = "Pop";

	private const string GENRE_NAME_DUBSTEP = "Dubstep";

	private const string GENRE_NAME_CADENCE = "Cadence";

	public AudioSource genreClipSource;

	public LocalizedWebLink localizedWebLinkTOS;

	public DisplayNativeWebPage nativeWebPagePrefab;

	public RectTransform canvas;

	public DisplayNativeWebPage nativeWebPageinstance;

	public Button backButton;

	private Dictionary<string, GenreVO> genres = new Dictionary<string, GenreVO>();

	[Inject(ContextKeys.CONTEXT_DISPATCHER)]
	public IEventDispatcher dispatcher
	{
		get;
		set;
	}

	protected override void Start()
	{
		base.Start();
		HardwareBackButtonDispatcher.SetTargetClickHandler(backButton, visible: false);
	}

	public void OnTermsClick()
	{
		dispatcher.Dispatch("TERMS_CLICK_EVENT");
	}

	public void onPopClick()
	{
		DispatchLoadGenre("Pop");
	}

	public void onDanceClick()
	{
		DispatchLoadGenre("Dance");
	}

	public void onRockClick()
	{
		DispatchLoadGenre("Rock");
	}

	public void onDubstepClick()
	{
		DispatchLoadGenre("Dubstep");
	}

	public void onSpookyClick()
	{
		DispatchLoadGenre("Spooky");
	}

	public void onCadenceClick()
	{
		DispatchLoadGenre("Cadence");
	}

	public void onBackClick()
	{
		dispatcher.Dispatch("BACK_CLICK_EVENT");
	}

	private void DispatchLoadGenre(string name)
	{
		if (genres.ContainsKey(name))
		{
			GenreVO data = genres[name];
			dispatcher.Dispatch("GENRE_CLICK_EVENT", data);
		}
		if (genres.Count != 0)
		{
		}
	}

	public void loadGenre(GenreVO genre)
	{
		genres.Add(genre.name, genre);
	}
}
