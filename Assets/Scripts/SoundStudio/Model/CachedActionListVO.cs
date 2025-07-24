using System.Collections.Generic;

namespace SoundStudio.Model
{
	public class CachedActionListVO
	{
		public List<CachedActionVO> actionlist
		{
			get;
			set;
		}

		public void addNewAction(CachedActionVO action)
		{
			if (!isActionSaved(action))
			{
				actionlist.Add(action);
			}
		}

		private bool isActionSaved(CachedActionVO action)
		{
			return actionlist.Contains(action);
		}
	}
}
