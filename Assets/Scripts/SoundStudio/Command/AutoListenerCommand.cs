using strange.extensions.command.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using StrangeHelper;

namespace SoundStudio.Command
{
	public abstract class AutoListenerCommand : EventCommand
	{
		protected AutoListener contextDispatcherAutoListener;

		[PostConstruct]
		public void Initialize()
		{
			contextDispatcherAutoListener = new AutoListener(base.dispatcher);
		}

		public void AddListener(object evt, EmptyCallback callback)
		{
			contextDispatcherAutoListener.AddListener(evt, callback);
		}

		public void AddListener(object evt, EventCallback callback)
		{
			contextDispatcherAutoListener.AddListener(evt, callback);
		}

		public override void Release()
		{
			base.Release();
			contextDispatcherAutoListener.RemovedListeners();
		}

		public override void Fail()
		{
			base.Fail();
			contextDispatcherAutoListener.RemovedListeners();
		}
	}
}
