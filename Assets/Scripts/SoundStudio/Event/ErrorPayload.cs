using System.Collections.Generic;

namespace SoundStudio.Event
{
	public class ErrorPayload
	{
		private string errorNum;

		private List<object> args;

		public string ErrorNum => errorNum;

		public List<object> Args => args;

		public ErrorPayload(string errorNum, List<object> args = null)
		{
			this.errorNum = errorNum;
			this.args = args;
		}
	}
}
