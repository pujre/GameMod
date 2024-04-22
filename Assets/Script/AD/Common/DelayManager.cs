using System.Collections;
using UnityEngine;

public static class DelayManager
{
	// ��ʼһ����ʱ�¼�
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

	// ��ʱЭ��
	private static IEnumerator DelayCoroutine(float delayTime, System.Action callback)
	{
		yield return new WaitForSeconds(delayTime);
		callback?.Invoke();
	}
}
