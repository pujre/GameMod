using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
	
    public GoundBackItem[,] GoundBackItemArray2D;
	private Dictionary<string, PanelBase> PanelDic = new Dictionary<string, PanelBase>();

	private void Awake()
	{
		FindPanelBase();

	}
	void Start()
	{

	}

	void Update()
	{

	}

	public void SetGoundBack(int x,int y){
        GoundBackItemArray2D= new GoundBackItem[x, y];
        for (int i = 0; i < GoundBackItemArray2D.LongLength; i++)
        {
            
        }
    }

    public void EnterSurface(int x,int y)
    {

    }


	public void FindPanelBase() {
		PanelBase[] panelBase = FindObjectsOfType<PanelBase>();
		for (int i = 0; i < panelBase.Length; i++)
		{
			PanelDic.Add(panelBase[i].gameObject.name, panelBase[i]);
		}
	}




	public void ActivatePanel(string panelName)
	{
		if (PanelDic.TryGetValue(panelName, out PanelBase panel))
		{
			panel.ActivatePanel();
		}
	}

	// 隐藏面板
	public void DeactivatePanel(string panelName)
	{
		if (PanelDic.TryGetValue(panelName, out PanelBase panel))
		{
			panel.DeactivatePanel();
		}
	}

	// 触发特定的方法
	public void TriggerPanelMethod(string panelName, string methodName, object[] parameters)
	{
		if (PanelDic.TryGetValue(panelName, out PanelBase panel))
		{
			panel.CallSpecificMethod(methodName, parameters);
		}
	}
}

