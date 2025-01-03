using System.Collections;
using UnityEngine;

public static class DelayManager
{
	// 开始一个延时事件
	public static void StartDelay(float delayTime, System.Action callback)
	{
		MonoBehaviour monoBehaviour = GameObject.FindObjectOfType<MonoBehaviour>();
		if (monoBehaviour != null)
		{
			monoBehaviour.StartCoroutine(DelayCoroutine(delayTime, callback));
		}
		else
		{
			Debug.LogError("DelayManager: MonoBehaviour not found in the scene.");
		}
	}

	// 延时协程
	private static IEnumerator DelayCoroutine(float delayTime, System.Action callback)
	{
		yield return new WaitForSeconds(delayTime);
		callback?.Invoke();
	}
}
