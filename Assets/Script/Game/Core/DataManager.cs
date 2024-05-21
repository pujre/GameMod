using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	private static DataManager instance;
	public static DataManager Instance
	{
		get
		{
			if (instance == null)
			{
				var manager = new GameObject("DataManager").AddComponent<DataManager>();
				DontDestroyOnLoad(manager.gameObject);
				instance = manager;
			}
			return instance;
		}
	}


	public void SetData(OnDataKey key,int x)
	{
		PlayerPrefs.SetInt(key.ToString(),x);
	}

	public int GetData(OnDataKey key) {
		int x= PlayerPrefs.GetInt(key.ToString(),0);
		return x;
	}

	
}
