using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	public class SongPlayCommand : EventCommand
	{
		private SongVO song;

		[Inject]
		public SongPlayer songPlayer
		{
			get;
			set;
		}

		[Inject]
		public GenreAudioManager genreAudioManager
		{
			get;
			set;
		}

		[Inject]
		public ApplicationState application
		{
			get;
			set;
		}

		[Inject(ContextKeys.CONTEXT_VIEW)]
		public GameObject contextView
		{
			get;
			set;
		}

		public override void Execute()
		{
			songPlayer.Stop();
			song = (base.evt.data as SongVO);
			if (song == null)
			{
				throw new InvalidOperationException("SongVO cannot be null.");
			}
			GenreVO genreByID = application.genreData.getGenreByID(song.GenreID);
			if (genreByID != null)
			{
				if (!genreByID.Equals(application.CurrentGenre))
				{
					AudioClipPlayer.ClearMemoryAsNeeded();
				}
				if (application.CurrentLevel == SceneNames.Radio.ToString())
				{
					genreAudioManager.loadGenreAudio(genreByID, song.CalculateUniqueSoundIDs());
					songPlayer.Play(song);
				}
				else
				{
					Retain();
					base.dispatcher.AddListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnGenreLoadComplete);
					MonoBehaviour component = contextView.GetComponent<SoundStudioRoot>();
					component.StartCoroutine(genreAudioManager.LoadGenreAudioAsync(genreByID, song.CalculateUniqueSoundIDs()));
				}
				application.CurrentGenre = genreByID;
			}
		}

		private void OnGenreLoadComplete()
		{
			base.dispatcher.RemoveListener(SoundStudioEvent.LOAD_GENRE_AUDIO_COMPLETE, OnGenreLoadComplete);
			songPlayer.Play(song);
			Release();
		}
	}
}
