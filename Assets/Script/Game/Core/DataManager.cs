using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMono<DataManager>
{
	public void SetData(OnDataKey key,int x)
	{
		PlayerPrefs.SetInt(key.ToString(),x);
	}

	public int GetData(OnDataKey key) {
		return PlayerPrefs.GetInt(key.ToString(),0);
	}

	
}
