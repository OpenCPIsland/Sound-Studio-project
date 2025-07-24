using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowPopupCommandPayload
	{
		private GameObject parentObject;

		private string popupPath;

		public GameObject ParentObject => parentObject;

		public string PopupPath => popupPath;

		public ShowPopupCommandPayload(GameObject parentObject, string popupPath)
		{
			this.parentObject = parentObject;
			this.popupPath = popupPath;
		}
	}
}
