using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateManager : MonoBehaviour
{
	public static DelegateManager Instance { get; private set; }

	private Dictionary<string, Delegate> delegateDict = new Dictionary<string, Delegate>();

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject); // 确保只有一个实例存在
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // 使得管理器在场景加载时不被销毁
		}
	}

	// 添加委托
	public void AddDelegate(string key, Delegate del)
	{
		if (delegateDict.ContainsKey(key))
		{
			delegateDict[key] = Delegate.Combine(delegateDict[key], del);
		}
		else
		{
			delegateDict[key] = del;
		}
	}

	// 移除委托
	public void RemoveDelegate(string key, Delegate del)
	{
		if (delegateDict.ContainsKey(key))
		{
			Delegate currentDel = delegateDict[key];
			currentDel = Delegate.Remove(currentDel, del);

			if (currentDel == null)
			{
				delegateDict.Remove(key);
			}
			else
			{
				delegateDict[key] = currentDel;
			}
		}
	}

	// 广播委托
	public void Broadcast(string key, params object[] args)
	{
		if (delegateDict.ContainsKey(key))
		{
			delegateDict[key].DynamicInvoke(args);
		}
	}
}
