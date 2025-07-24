using Newtonsoft.Json;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using System.IO;

public class SongSaveCommand : EventCommand
{
	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	public override void Execute()
	{
		SongVO songVO = base.evt.data as SongVO;
		if (songVO != null)
		{
			if (string.IsNullOrEmpty(songVO.FileName))
			{
				songVO.timeStamp = DateTime.Now;
				songVO.FileName = string.Format("{0}{1}", "song_", Guid.NewGuid());
				songVO.playerid = application.currentPlayer.ID;
				songVO.cachedDelete = false;
				application.songData.AddReplaceSong(songVO);
			}
			try
			{
				if (!Directory.Exists(application.CurrentPlayerDataDirectory))
				{
					Directory.CreateDirectory(application.CurrentPlayerDataDirectory);
				}
				string contents = JsonConvert.SerializeObject(songVO);
				File.WriteAllText(application.CurrentPlayerDataDirectory + songVO.FileName, contents);
				base.dispatcher.Dispatch(SongSaveEvent.SONG_SAVE_SUCCESS, new SongSyncVO(songVO));
			}
			catch (IOException)
			{
			}
			catch (JsonSerializationException)
			{
			}
			catch (Exception)
			{
			}
			return;
		}
		throw new InvalidOperationException("The song VO was not set!");
	}
}
