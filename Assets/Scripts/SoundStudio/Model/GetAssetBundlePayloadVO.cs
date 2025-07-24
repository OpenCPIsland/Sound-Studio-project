using UnityEngine;

namespace SoundStudio.Model
{
	internal class GetAssetBundlePayloadVO
	{
		public GameObject ParentGameObject
		{
			get;
			set;
		}

		public string BundleURL
		{
			get;
			set;
		}

		public int Version
		{
			get;
			set;
		}
	}
}
