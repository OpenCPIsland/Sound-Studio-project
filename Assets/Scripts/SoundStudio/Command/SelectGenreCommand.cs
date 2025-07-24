using SoundStudio.Event;
using SoundStudio.Model;
using SoundStudio.Service;
using strange.extensions.command.impl;

namespace SoundStudio.Command
{
	public class SelectGenreCommand : EventCommand
	{
		[Inject]
		public ApplicationState applicationState
		{
			get;
			set;
		}

		[Inject]
		public IMWSClientService MWSClientService
		{
			get;
			set;
		}

		public override void Execute()
		{
			GenreVO genreVO = (GenreVO)base.evt.data;
			applicationState.CurrentGenre = genreVO;
			base.dispatcher.Dispatch(LoadLevelEvent.LOAD_LEVEL, SceneNames.MixingBoard);
			if (applicationState.currentPlayer.AccountStatus == MembershipStatus.MEMBER)
			{
				MWSClientService.AddSoundStudioGenreToCollection(applicationState.currentPlayer, genreVO.id);
			}
		}
	}
}
