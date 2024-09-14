using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ӿڣ��ɳػ�����
/// </summary>
public interface IPoolable
{
	void OnCreate();
	void OnFetch();
	void OnRecycle();
	void OnDestroy();
}

/// <summary>
/// ����������࣬�����ض����͵Ķ���
/// </summary>
public class Pool
{
	private Stack<GameObject> availableObjects = new Stack<GameObject>();
	private GameObject prefab;

	public Pool(GameObject prefab)
	{
		this.prefab = prefab;
	}

	/// <summary>
	/// ��ȡ����
	/// </summary>
	public GameObject GetObject(Transform parent = null)
	{
		GameObject obj;
		if (availableObjects.Count > 0)
		{
			obj = availableObjects.Pop();
			obj.SetActive(true);
			obj.transform.SetParent(parent);
		}
		else
		{
			obj = Object.Instantiate(prefab, parent);
			obj.name = prefab.name;
			IPoolable poolable = obj.GetComponent<IPoolable>();
			poolable?.OnCreate();
		}

		IPoolable fetchPoolable = obj.GetComponent<IPoolable>();
		fetchPoolable?.OnFetch();

		return obj;
	}

	/// <summary>
	/// ���ն���
	/// </summary>
	public void ReturnObject(GameObject obj)
	{
		IPoolable poolable = obj.GetComponent<IPoolable>();
		poolable?.OnRecycle();

		obj.SetActive(false);
		obj.transform.SetParent(null);
		availableObjects.Push(obj);
	}

	/// <summary>
	/// ��������
	/// </summary>
	public void Clear()
	{
		while (availableObjects.Count > 0)
		{
			GameObject obj = availableObjects.Pop();
			IPoolable poolable = obj.GetComponent<IPoolable>();
			poolable?.OnDestroy();
			Object.Destroy(obj);
		}
	}
}

/// <summary>
/// ����ع�����������ģʽ
/// </summary>
public class PoolManager : SingletonMono<PoolManager>
{
	private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

	/// <summary>
	/// ��ȡ����
	/// </summary>
	public GameObject GetObject(string prefabName, Transform parent = null)
	{
		if (string.IsNullOrEmpty(prefabName))
		{
			Debug.LogError("Prefab name is null or empty.");
			return null;
		}

		if (!pools.ContainsKey(prefabName))
		{
			// ����Ԥ����
			GameObject prefab = Resources.Load<GameObject>(ResPath.GetPath(prefabName));
			if (prefab == null)
			{
				Debug.LogError($"Prefab not found: {prefabName}");
				return null;
			}
			pools[prefabName] = new Pool(prefab);
		}

		return pools[prefabName].GetObject(parent);
	}

	/// <summary>
	/// ��ȡ����ͨ��Ԥ���壩
	/// </summary>
	public GameObject GetObject(GameObject prefab, Transform parent = null)
	{
		if (prefab == null)
		{
			Debug.LogError("Prefab is null.");
			return null;
		}

		string prefabName = prefab.name;

		if (!pools.ContainsKey(prefabName))
		{
			pools[prefabName] = new Pool(prefab);
		}

		return pools[prefabName].GetObject(parent);
	}

	/// <summary>
	/// ���ն���
	/// </summary>
	public void ReturnObject(GameObject obj)
	{
		if (obj == null)
		{
			Debug.LogError("GameObject is null.");
			return;
		}

		string prefabName = obj.name.Replace("(Clone)", "");

		if (pools.ContainsKey(prefabName))
		{
			pools[prefabName].ReturnObject(obj);
		}
		else
		{
			Debug.LogWarning($"No pool found for object: {prefabName}. Destroying object.");
			IPoolable poolable = obj.GetComponent<IPoolable>();
			poolable?.OnDestroy();
			Destroy(obj);
		}
	}

	/// <summary>
	/// �����ض������
	/// </summary>
	public void ClearPool(string prefabName)
	{
		if (pools.ContainsKey(prefabName))
		{
			pools[prefabName].Clear();
			pools.Remove(prefabName);
		}
	}

	/// <summary>
	/// �������ж����
	/// </summary>
	public void ClearAllPools()
	{
		foreach (var pool in pools.Values)
		{
			pool.Clear();
		}
		pools.Clear();
	}

	/// <summary>
	/// �ж϶�����Ƿ����
	/// </summary>
	public bool PoolExists(string prefabName)
	{
		return pools.ContainsKey(prefabName);
	}
}
