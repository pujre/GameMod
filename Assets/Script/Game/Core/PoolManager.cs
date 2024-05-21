
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PoolManager : SingletonMono<PoolManager>
{
	private static Vector3 OutOfRange = new Vector3(9000, 9000, 9000);
	private static Dictionary<string, Dictionary<int, GameObject>> createPools = new Dictionary<string, Dictionary<int, GameObject>>();
	private static Dictionary<string, Dictionary<int, GameObject>> recyclePools = new Dictionary<string, Dictionary<int, GameObject>>();

    protected override void Awake()
    {
        base.Awake();
    }
	public static Dictionary<string, Dictionary<int, GameObject>> GetCreatePool()
	{
		return createPools;
	}

	public static Dictionary<string, Dictionary<int, GameObject>> GetRecyclePool()
	{
		return recyclePools;
	}

	#region ����

	//ͨ�������������� ֱ��ʵ����
	public static GameObject CreateGameObject(string name, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(true, name, null, parent, isActive);
	}

	//ͨ��Ԥ���� ֱ��ʵ����
	public static GameObject CreateGameObject(GameObject prefab, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(true, null, prefab, parent, isActive);
	}

	//ͨ������ �����ʵ����
	public static GameObject CreateGameObjectByPool(string name, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(false, name, null, parent, isActive);
	}

	//ͨ��Ԥ���� ֱ��ʵ����
	public static GameObject CreateGameObjectByPool(GameObject prefab, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(false, null, prefab, parent, isActive);
	}

	//ͨ�������������� ֱ��ʵ����
	public static T CreateGameObject<T>(string name, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(true, name, null, parent, isActive).GetComponent<T>();
	}

	//ͨ��Ԥ���� ֱ��ʵ����
	public static T CreateGameObject<T>(GameObject prefab, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(true, null, prefab, parent, isActive).GetComponent<T>(); ;
	}

	//ͨ������ �����ʵ����
	public static T CreateGameObjectByPool<T>(string name, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(false, name, null, parent, isActive).GetComponent<T>(); ;
	}

	//ͨ��Ԥ���� ֱ��ʵ����
	public static T CreateGameObjectByPool<T>(GameObject prefab, GameObject parent = null, bool isActive = true)
	{
		return GetNewObject(false, null, prefab, parent, isActive).GetComponent<T>(); ;
	}


	/// <summary>
	/// ��ȡһ���µĶ���
	/// </summary>
	/// <param name="isNew">�Ƿ񴴽��µĶ���</param>
	/// <param name="objName">ʵ�������������</param>
	/// <param name="prefab">ʵ����Ԥ����</param>
	/// <param name="parent">ʵ����������</param>
	/// <param name="isActive"></param>
	/// <returns></returns>
	private static GameObject GetNewObject(bool isNew, string objName, GameObject prefab, GameObject parent = null, bool isActive = true)
	{
		GameObject go = null;

		//��ȡ��������
		string name = objName;
		if (string.IsNullOrEmpty(name))
		{
			name = prefab.name;
		}

		if (!isNew && IsExist(name))
		{
			if (!recyclePools.ContainsKey(name))
			{
				//�����µĶ���
				if (prefab != null)
				{
					go = InstantiateObject(prefab, parent);
				}
				else
				{
					go = NewGameObject(name, parent);
				}
			}
			else
			{
				//�ӻ��ճ��м���
				List<int> ids = new List<int>(recyclePools[name].Keys);
				int id = ids[0];
				go = recyclePools[name][id];
				recyclePools[name].Remove(id);
				if (recyclePools[name].Count == 0) recyclePools.Remove(name);
			}
		}
		else
		{
			//ֱ�Ӵ����µĶ���
			if (prefab == null && !string.IsNullOrEmpty(objName))
			{
				go = NewGameObject(name, parent);

			}
			else if (prefab != null && string.IsNullOrEmpty(objName))
			{
				go = InstantiateObject(prefab, parent);
			}
		}

		if (go == null)
		{
			Debug.LogError("PoolManager ����ʧ�ܣ�" + name);
			return go;
		}

		//��¼���浽createPool
		if (!createPools.ContainsKey(name)) createPools.Add(name, new Dictionary<int, GameObject>());
		createPools[name].Add(go.GetInstanceID(), go);

		//��ʼ��
		PoolObject obj = go.GetComponent<PoolObject>();
		if (obj) obj.OnFetch();

		go.SetActive(isActive);
		go.transform.SetParent(parent ? parent.transform : null);

		return go;
	}

	//ʵ����
	private static GameObject InstantiateObject(GameObject prefab, GameObject parent = null)
	{
		if (prefab == null)
		{
			throw new Exception("CreateGameObject error : prefab  is null");
		}
		Transform transform = parent == null ? null : parent.transform;
		GameObject instanceTmp = UnityEngine.Object.Instantiate(prefab, transform);
		instanceTmp.name = prefab.name;

		PoolObject p = instanceTmp.GetComponent<PoolObject>();
		if (p != null) p.OnCreate();

		return instanceTmp;
	}

	//����һ������ʵ����
	private static GameObject NewGameObject(string gameObjectName, GameObject parent = null)
	{
		/****��ʱ��Resources��ȡ****/
		GameObject goTmp = Resources.Load<GameObject>(ResPath.GetPath(gameObjectName));

		if (goTmp == null)
		{
			throw new Exception("CreateGameObject error dont find :" + gameObjectName);
		}

		return InstantiateObject(goTmp, parent);
	}

	#endregion

	#region ����

	//���ն���
	public static void DestoryByRecycle(GameObject gameObject, bool isSetInactive = true)
	{
		if (!gameObject) return;

		string key = gameObject.name.Replace("(Clone)", "");
		if (!recyclePools.ContainsKey(key)) recyclePools.Add(key, new Dictionary<int, GameObject>());

		if (recyclePools[key].ContainsKey(gameObject.GetInstanceID()))
		{
			Debug.LogError("Recycle repeat by Destory -> " + gameObject.name);
			return;
		}
		recyclePools[key].Add(gameObject.GetInstanceID(), gameObject);

		if (isSetInactive) gameObject.SetActive(false);
		else gameObject.transform.position = OutOfRange;

		gameObject.name = key;
		PoolObject obj = gameObject.GetComponent<PoolObject>();
		if (obj) obj.OnRecycle();

		if (createPools.ContainsKey(key) && createPools[key].ContainsKey(gameObject.GetInstanceID()))
		{
			createPools[key].Remove(gameObject.GetInstanceID());
		}
		else Debug.LogError("����ز�����GameObject��" + gameObject + " ���ܻ��գ�");
	}

	//��ʱ��� ����
	public static void DestoryByRecycle(GameObject gameObject, float time)
	{
		DestoryByRecycle(gameObject);
	}


	//ֱ������
	public static void Destory(GameObject gameObject)
	{
		if (gameObject == null) return;

		string key = gameObject.name.Replace("(Clone)", "");

		PoolObject obj = gameObject.GetComponent<PoolObject>();
		if (obj) obj.OnDestory();

		//�Ƴ�
		if (createPools.ContainsKey(key) && createPools[key].ContainsKey(gameObject.GetInstanceID()))
		{
			createPools[key].Remove(gameObject.GetInstanceID());
			if (createPools[key].Count == 0) createPools.Remove(key);
		}

		UnityEngine.Object.Destroy(gameObject);
	}

	//ֱ������
	public static void Destory(GameObject gameObject, float time)
	{
		if (gameObject == null) return;

		string key = gameObject.name.Replace("(Clone)", "");

		PoolObject obj = gameObject.GetComponent<PoolObject>();
		if (obj) obj.OnDestory();

		//�Ƴ�
		if (createPools.ContainsKey(key) && createPools[key].ContainsKey(gameObject.GetInstanceID()))
		{
			createPools[key].Remove(gameObject.GetInstanceID());
			if (createPools[key].Count == 0) createPools.Remove(key);
		}

		UnityEngine.Object.Destroy(gameObject, time);
	}

	#endregion

	//�ж��Ƿ���ڸĶ���
	public static bool IsExist(string objName)
	{
		if (string.IsNullOrEmpty(objName))
		{
			Debug.LogError("PoolManager objName is null");
			return false;
		}

		if ((createPools.ContainsKey(objName) && createPools[objName].Count > 0)
			|| recyclePools.ContainsKey(objName) && recyclePools[objName].Count > 0)
		{
			return true;
		}

		return false;
	}

	public static bool IsExist(GameObject gameObject)
	{
		if (gameObject == null) return false;

		return IsExist(gameObject.name.Replace("(Clone)", ""));
	}

	//����ػ��� �Ѿ��������˵Ķ����  ��û��Ԫ�ص����  ���������ڻ��ն����
	public static void PoolClean()
	{
		//�崴�����˵Ķ����
		List<string> cleanPool = new List<string>();
		foreach (var item in createPools)
		{
			if (item.Value.Count == 0) cleanPool.Add(item.Key);
		}

		cleanPool.ForEach((s) => createPools.Remove(s));

		//��ջ��ն����
		List<GameObject> cleanObj = new List<GameObject>();
		foreach (var item in recyclePools)
		{
			foreach (var v in item.Value)
			{
				cleanObj.Add(v.Value);
			}
		}
		cleanObj.ForEach((s) =>
		{
			PoolObject obj = s.GetComponent<PoolObject>();
			if (obj) obj.OnDestory();
			UnityEngine.Object.Destroy(s);
		});
		recyclePools.Clear();
	}

	//����ĳһ����ĳ�
	public static void PoolClean(string name)
	{
		if (recyclePools.ContainsKey(name))
		{
			foreach (var item in recyclePools[name])
			{
				PoolObject obj = item.Value.GetComponent<PoolObject>();

				if (obj) obj.OnDestory();
				UnityEngine.Object.Destroy(item.Value);

			}
			recyclePools.Remove(name);
		}

		if (createPools.ContainsKey(name) && createPools[name].Count == 0) createPools.Remove(name);
	}

	
}
