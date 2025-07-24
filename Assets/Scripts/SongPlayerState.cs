using System;

public class SongPlayerState
{
	private PlayState playState;

	public PlayState PlayState => playState;

	public SongPlayerState()
	{
		playState = PlayState.STOPPED;
	}

	public PlayState SwitchState(PlayState newState)
	{
		switch (playState)
		{
		case PlayState.STOPPED:
			if (newState == PlayState.PLAYING)
			{
				playState = newState;
			}
			break;
		case PlayState.PLAYING:
		case PlayState.PAUSED:
			playState = newState;
			break;
		default:
			throw new InvalidOperationException("Possible logic error: If you have added a new state you need to specify how it transitions!");
		}
		return playState;
	}

	public override string ToString()
	{
		return playState.ToString();
	}
}
