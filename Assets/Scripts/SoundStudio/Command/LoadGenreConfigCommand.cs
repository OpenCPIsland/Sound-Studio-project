using Newtonsoft.Json;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using UnityEngine;

namespace SoundStudio.Command
{
	public class LoadGenreConfigCommand : EventCommand
	{
		[Inject]
		public ApplicationState applicationState
		{
			get;
			set;
		}

		public GenreListVO DeserializeResourceFile()
		{
			TextAsset textAsset = (TextAsset)Resources.Load("Data/genre_data", typeof(TextAsset));
			return JsonConvert.DeserializeObject<GenreListVO>(textAsset.text);
		}

		public override void Execute()
		{
			try
			{
				GenreListVO genreListVO = DeserializeResourceFile();
				if (genreListVO != null)
				{
					applicationState.genreData = genreListVO;
				}
			}
			catch (JsonSerializationException)
			{
			}
			catch (NullReferenceException)
			{
			}
			catch (JsonReaderException)
			{
			}
			base.dispatcher.Dispatch(SoundStudioEvent.LOAD_GENRES_COMPLETED);
		}
	}
}
