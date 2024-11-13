using System;
using System.Collections.Generic;
using UnityEngine;
namespace TYQ
{
    public class TaTimeManager : SingletonMono<TaTimeManager>
	{
		// 存储所有活动的计时器
		private List<Timer> timers = new List<Timer>();
		/// <summary>
		/// 启动一个计时器
		/// </summary>
		/// <param name="duration">计时时长（秒）</param>
		/// <param name="callback">计时结束后的回调</param>
		/// <param name="isLoop">是否重复计时</param>
		/// <returns>返回Timer对象，可用于取消计时器</returns>
		public Timer StartTimer(float duration, Action callback, bool isLoop = false)
		{
			Timer timer = new Timer(duration, callback, isLoop);
			timers.Add(timer);
			return timer;
		}

		/// <summary>
		/// 取消一个计时器
		/// </summary>
		/// <param name="timer">需要取消的Timer对象</param>
		public void CancelTimer(Timer timer)
		{
			if (timers.Contains(timer))
			{
				timers.Remove(timer);
			}
		}

		private void Update()
		{
			float currentTime = Time.realtimeSinceStartup;
			for (int i = timers.Count - 1; i >= 0; i--)
			{
				Timer timer = timers[i];
				timer.Update(currentTime);
				if (timer.IsCompleted)
				{
					if (timer.IsLoop)
					{
						timer.Reset(currentTime);
					}
					else
					{
						timers.RemoveAt(i);
					}
				}
			}
		}
	}
	/// <summary>
	/// 内部计时器类
	/// </summary>
	public class Timer
	{
		public float Duration { get; private set; }
		private float startTime;
		private Action callback;
		public bool IsLoop { get; private set; }
		public bool IsCompleted { get; private set; }

		public Timer(float duration, Action callback, bool isLoop)
		{
			Duration = duration;
			this.callback = callback;
			IsLoop = isLoop;
			IsCompleted = false;
			startTime = Time.realtimeSinceStartup;
		}

		public void Update(float currentTime)
		{
			if (IsCompleted)
				return;

			if (currentTime - startTime >= Duration)
			{
				callback?.Invoke();
				IsCompleted = true;
			}
		}

		public void Reset(float currentTime)
		{
			startTime = currentTime;
			IsCompleted = false;
		}
	}
}
