using Newtonsoft.Json;
using SoundStudio.Event;
using SoundStudio.Model;
using strange.extensions.command.impl;
using System;
using System.Collections.Generic;
using System.IO;

public class LoadSongListCommand : EventCommand
{
	[Inject]
	public ApplicationState application
	{
		get;
		set;
	}

	public override void Execute()
	{
		application.songData = new SongListVO();
		string displayName = application.currentPlayer.DisplayName;
		List<SongVO> newSongs = LoadSongs(displayName);
		application.songData.AddAll(newSongs);
		base.dispatcher.Dispatch(SoundStudioEvent.LOAD_SONGS_COMPLETED);
		List<long> list = new List<long>();
		list.Add(application.currentPlayer.ID);
	}

	public List<SongVO> LoadSongs(string playerName)
	{
		List<SongVO> list = new List<SongVO>();
		if (Directory.Exists(application.CurrentPlayerDataDirectory))
		{
			string[] files = Directory.GetFiles(application.CurrentPlayerDataDirectory);
			string[] array = files;
			foreach (string text in array)
			{
				try
				{
					if (text.IndexOf("song_") != -1)
					{
						string value = loadFile(text);
						SongVO item = JsonConvert.DeserializeObject<SongVO>(value);
						list.Add(item);
					}
				}
				catch (IOException)
				{
				}
				catch (NotSupportedException)
				{
				}
				catch (JsonSerializationException)
				{
				}
				catch (Exception)
				{
				}
			}
		}
		return list;
	}

	private string loadFile(string filePath)
	{
		string result = string.Empty;
		try
		{
			if (!File.Exists(filePath))
			{
				return result;
			}
			result = File.ReadAllText(filePath);
			return result;
		}
		catch (IOException)
		{
			return result;
		}
	}
}
