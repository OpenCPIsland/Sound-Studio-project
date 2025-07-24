using UnityEngine;

namespace SoundStudio.Command
{
	public class ShowProtectedWebPagePayload
	{
		private RectTransform parentPermissionContainer;

		private string url;

		public RectTransform ParentPermissionContainer => parentPermissionContainer;

		public string Url => url;

		public ShowProtectedWebPagePayload(RectTransform parentPermissionContainer, string url)
		{
			this.parentPermissionContainer = parentPermissionContainer;
			this.url = url;
		}
	}
}
