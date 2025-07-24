public static class MuteStateExtensions
{
	public static MuteState FromBoolean(bool value)
	{
		if (value)
		{
			return MuteState.ON;
		}
		return MuteState.OFF;
	}
}
