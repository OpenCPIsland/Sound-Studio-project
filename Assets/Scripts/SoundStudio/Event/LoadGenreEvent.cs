using SoundStudio.Model;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace SoundStudio.Event
{
	public class LoadGenreEvent : IEvent
	{
		public static readonly string LOAD_GENRE = "LOAD_GENRE";

		public object type
		{
			get;
			set;
		}

		public IEventDispatcher target
		{
			get;
			set;
		}

		public object data
		{
			get;
			set;
		}

		public GenreVO genre
		{
			get;
			set;
		}

		public LoadGenreEvent(object type, IEventDispatcher target, GenreVO genre)
		{
			this.type = type;
			this.target = target;
			this.genre = genre;
		}
	}
}
