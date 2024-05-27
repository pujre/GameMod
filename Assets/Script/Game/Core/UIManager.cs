using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonMono<UIManager>
{
	private Dictionary<string, PanelBase> PanelDic = new Dictionary<string, PanelBase>();
	public Material[] Colors;


	protected override void Awake()
	{
		base.Awake();
		FindAllPanel();
	}

	void FindAllPanel()
	{
		PanelBase[] allPanel = FindObjectsOfType<PanelBase>(true);
		for (int i = 0; i < allPanel.Length; i++)
		{
			if (!PanelDic.ContainsKey(allPanel[i].gameObject.name))
				PanelDic.Add(allPanel[i].gameObject.name, allPanel[i]);
		}
	}

	void Start()
	{
		FindAllPanel();
	}

	void Update()
	{

	}

	public void SetUiPanelAction(string panelName, bool isOn)
	{
		PanelBase p;
		PanelDic.TryGetValue(panelName, out p);
		if (p)
		{
			p.ActivatePanel(isOn);
		}
		else
		{
			Debug.Log(string.Format("未找到名为 {0} 的面板", panelName));
		}
	}

	public PanelBase GetPanel(string panelName)
	{
		PanelBase p;
		PanelDic.TryGetValue(panelName, out p);
		if (p)
		{
			return p;
		}
		Debug.Log(string.Format("未找到名为 {0} 的面板", panelName));
		return null;
	}

	// 触发指定名字特定的方法
	public void TriggerPanelMethod(string panelName, string methodName, object[] parameters)
	{
		if (PanelDic.TryGetValue(panelName, out PanelBase panel))
		{
			panel.CallSpecificMethod(methodName, parameters);
		}
	}
}