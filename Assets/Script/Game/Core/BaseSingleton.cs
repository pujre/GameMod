using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				// �ڳ����в����Ƿ��Ѵ��ڸ����͵�ʵ��
				instance = FindObjectOfType<T>();
				// ��������в����ڸ����͵�ʵ�����򴴽�һ���µ�GameObject����Ӹ����
				if (instance == null)
				{
					GameObject singletonObject = new GameObject(typeof(T).Name + "_Singleton");
					instance = singletonObject.AddComponent<T>();
					DontDestroyOnLoad(singletonObject); // �����ڳ����л�ʱ��������
				}
			}
			return instance;
		}
	}
	//ʹ��virtual�麯��������̳п��ܻ���Ҫ��Awake()
	protected virtual void Awake()
	{
		// ȷ���ڳ����л�ʱ�������ٸ�ʵ��
		DontDestroyOnLoad(gameObject);
		// ����Ƿ�����ظ���ʵ��
		if (instance == null)
		{
			instance = this as T;
		}
		else
		{
			Debug.LogWarning("�����ظ��ĵ���" + typeof(T).Name + "ɾ��");
			Destroy(gameObject);
		}
	}
}