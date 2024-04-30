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
			Destroy(gameObject); // ȷ��ֻ��һ��ʵ������
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // ʹ�ù������ڳ�������ʱ��������
		}
	}

	// ���ί��
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

	// �Ƴ�ί��
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

	// �㲥ί��
	public void Broadcast(string key, params object[] args)
	{
		if (delegateDict.ContainsKey(key))
		{
			delegateDict[key].DynamicInvoke(args);
		}
	}
}
