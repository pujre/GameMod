using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateManager : MonoBehaviour
{
	private static DelegateManager instance;
	public static DelegateManager Instance
	{
		get
		{
			if (instance == null)
			{
				var manager = new GameObject("DelegateManager").AddComponent<DelegateManager>();
				DontDestroyOnLoad(manager.gameObject);
				instance = manager;
			}
			return instance;
		}
	}

	private Dictionary<string, Action<object[]>> eventDictionary = new Dictionary<string, Action<object[]>>();

	// 添加事件
	public void AddEvent(string eventName, Action<object[]> action)
	{
		if (eventDictionary.ContainsKey(eventName))
		{
			eventDictionary[eventName] += action;
		}
		else
		{
			eventDictionary[eventName] = action;
		}
	}

	// 移除事件
	public void RemoveEvent(string eventName, Action<object[]> action)
	{
		if (eventDictionary.ContainsKey(eventName))
		{
			eventDictionary[eventName] -= action;
			if (eventDictionary[eventName] == null)
			{
				eventDictionary.Remove(eventName);
			}
		}
	}

	// 触发事件
	public void TriggerEvent(string eventName, params object[] parameters)
	{
		if (eventDictionary.ContainsKey(eventName))
		{
			eventDictionary[eventName](parameters);
		}
	}

	void OnDestroy()
	{
		instance = null;
	}
}
