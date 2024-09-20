using System;
using System.Collections.Generic;
namespace TYQ
{
	public class TYQEventCenter : SingletonMono<TYQEventCenter>
	{
		public Dictionary<OnEventKey, Delegate> MyEvent = new Dictionary<OnEventKey, Delegate>();
		public void OnListenerAdding(OnEventKey eventType, Delegate action)
		{
			//判断委托如果不存在就创建一个
			if (!MyEvent.ContainsKey(eventType))
			{
				MyEvent.Add(eventType, null);
			}
			Delegate oldAction = MyEvent[eventType];
			if (oldAction != null && oldAction.GetType() != action.GetType())
			{
				throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1}，要添加的委托类型为{2}", eventType, oldAction.GetType(), action.GetType()));
			}
		}
		public void OnListenerRemoving(OnEventKey eventType, Delegate action)
		{
			if (MyEvent.ContainsKey(eventType))
			{
				Delegate oldAction = MyEvent[eventType];
				if (oldAction == null)
				{

					throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", eventType));
				}
				else if (oldAction.GetType() != action.GetType())
				{
					throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1}，要移除的委托类型为{2}", eventType, oldAction.GetType(), action.GetType()));
				}
			}
			else
			{
				throw new Exception(string.Format("移除监听错误：没有事件码{0}", eventType));
			}
		}
		private void OnListenerRemoved(OnEventKey eventType)
		{
			if (MyEvent[eventType] == null)
			{
				MyEvent.Remove(eventType);
			}
		}
		public void AddListener(OnEventKey eventType, Action action)
		{
			OnListenerAdding(eventType, action);
			MyEvent[eventType] = (Action)MyEvent[eventType] + action;
		}
		public void AddListener<T>(OnEventKey eventType, Action<T> action)
		{
			OnListenerAdding(eventType, action);
			MyEvent[eventType] = (Action<T>)MyEvent[eventType] + action;
		}
		public void AddListener<T, W>(OnEventKey eventType, Action<T, W> action)
		{
			OnListenerAdding(eventType, action);
			MyEvent[eventType] = (Action<T, W>)MyEvent[eventType] + action;
		}
		public void AddListener<T, W, E>(OnEventKey eventType, Action<T, W, E> action)
		{
			OnListenerAdding(eventType, action);
			MyEvent[eventType] = (Action<T, W, E>)MyEvent[eventType] + action;
		}
		public void AddListener<T, W, E, Q>(OnEventKey eventType, Action<T, W, E, Q> action)
		{
			OnListenerAdding(eventType, action);
			MyEvent[eventType] = (Action<T, W, E, Q>)MyEvent[eventType] + action;
		}
		public void RemoveListener(OnEventKey eventType, Action action)
		{
			OnListenerRemoving(eventType, action);
			MyEvent[eventType] = (Action)MyEvent[eventType] - action;
			OnListenerRemoved(eventType);
		}
		public void RemoveListener<T>(OnEventKey eventType, Action<T> action)
		{
			OnListenerRemoving(eventType, action);
			MyEvent[eventType] = (Action<T>)MyEvent[eventType] - action;
			OnListenerRemoved(eventType);
		}
		public void RemoveListener<T, W>(OnEventKey eventType, Action<T, W> action)
		{
			OnListenerRemoving(eventType, action);
			MyEvent[eventType] = (Action<T, W>)MyEvent[eventType] - action;
			OnListenerRemoved(eventType);

		}
		public void RemoveListener<T, W, E>(OnEventKey eventType, Action<T, W, E> action)
		{
			OnListenerRemoving(eventType, action);
			MyEvent[eventType] = (Action<T, W, E>)MyEvent[eventType] - action;
			OnListenerRemoved(eventType);
		}
		public void RemoveListener<T, W, E, Q>(OnEventKey eventType, Action<T, W, E, Q> action)
		{
			OnListenerRemoving(eventType, action);
			MyEvent[eventType] = (Action<T, W, E, Q>)MyEvent[eventType] - action;
			OnListenerRemoved(eventType);
		}

		public void Broadcast(OnEventKey eventType)
		{
			Delegate d;
			if (MyEvent.TryGetValue(eventType, out d))
			{
				Action callBack = d as Action;
				if (callBack != null)
				{
					callBack();
				}
				else
				{
					throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
				}
			}
		}
		public void Broadcast<T>(OnEventKey eventType, T arg)
		{
			Delegate d;
			if (MyEvent.TryGetValue(eventType, out d))
			{
				Action<T> callBack = d as Action<T>;
				if (callBack != null)
				{
					callBack(arg);
				}
				else
				{
					throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
				}
			}
		}
		public void Broadcast<T, W>(OnEventKey eventType, T arg1, W arg2)
		{
			Delegate d;
			if (MyEvent.TryGetValue(eventType, out d))
			{
				Action<T, W> callBack = d as Action<T, W>;
				if (callBack != null)
				{
					callBack(arg1, arg2);
				}
				else
				{
					throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
				}
			}
		}

		public void Broadcast<T, W, E>(OnEventKey eventType, T arg1, W arg2, E arg3)
		{
			Delegate d;
			if (MyEvent.TryGetValue(eventType, out d))
			{
				Action<T, W, E> callBack = d as Action<T, W, E>;
				if (callBack != null)
				{
					callBack(arg1, arg2, arg3);
				}
				else
				{
					throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
				}
			}
		}
		public void Broadcast<T, W, E, Q>(OnEventKey eventType, T arg1, W arg2, E arg3, Q arg4)
		{
			Delegate d;
			if (MyEvent.TryGetValue(eventType, out d))
			{
				Action<T, W, E, Q> callBack = d as Action<T, W, E, Q>;
				if (callBack != null)
				{
					callBack(arg1, arg2, arg3, arg4);
				}
				else
				{
					throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", eventType));
				}
			}
		}

	}
}
