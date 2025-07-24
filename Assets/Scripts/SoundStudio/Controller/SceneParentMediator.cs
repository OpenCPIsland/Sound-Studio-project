using strange.extensions.mediation.impl;
using UnityEngine;

namespace SoundStudio.Controller
{
	public class SceneParentMediator : EventMediator
	{
		public void Start()
		{
			SetSceneViewChildOfContext();
		}

		private void SetSceneViewChildOfContext()
		{
			GameObject gameObject = GameObject.Find("ContextView");
			base.transform.parent = gameObject.transform;
		}
	}
}
