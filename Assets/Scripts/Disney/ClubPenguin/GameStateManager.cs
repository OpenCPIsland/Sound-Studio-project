using strange.extensions.injector.api;

namespace Disney.ClubPenguin
{
	[Implements(typeof(IGameStateManager), InjectionBindingScope.CROSS_CONTEXT)]
	public class GameStateManager : IGameStateManager
	{
		private static IGameStateManager instance = new GameStateManager();

		public static IGameStateManager Instance
		{
			get
			{
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		public GameState GameState
		{
			get;
			private set;
		}

		public GameStateManager()
		{
			GameState = new GameState();
		}
	}
}
