using strange.extensions.dispatcher.eventdispatcher.api;
using System.Collections.Generic;

namespace StrangeHelper
{
	public class AutoListener
	{
		private class ListenerItemPair
		{
			public object Evt
			{
				get;
				set;
			}

			public EventCallback Callback
			{
				get;
				set;
			}

			public EmptyCallback EmptyCallback
			{
				get;
				set;
			}
		}

		private IEventDispatcher dispatcher;

		private List<ListenerItemPair> listenerList = new List<ListenerItemPair>();

		public AutoListener(IEventDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public void AddListener(object evt, EmptyCallback callback)
		{
			dispatcher.AddListener(evt, callback);
			ListenerItemPair listenerItemPair = new ListenerItemPair();
			listenerItemPair.EmptyCallback = callback;
			listenerItemPair.Evt = evt;
			listenerList.Add(listenerItemPair);
		}

		public void AddListener(object evt, EventCallback callback)
		{
			dispatcher.AddListener(evt, callback);
			ListenerItemPair listenerItemPair = new ListenerItemPair();
			listenerItemPair.Callback = callback;
			listenerItemPair.Evt = evt;
			listenerList.Add(listenerItemPair);
		}

		public void RemovedListeners()
		{
			foreach (ListenerItemPair listener in listenerList)
			{
				if (listener.Callback != null)
				{
					dispatcher.RemoveListener(listener.Evt, listener.Callback);
				}
				if (listener.EmptyCallback != null)
				{
					dispatcher.RemoveListener(listener.Evt, listener.EmptyCallback);
				}
			}
			listenerList.Clear();
		}
	}
}
