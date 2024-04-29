using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public GoundBackItem[,] GoundBackItemArray2D;
	private Dictionary<string, PanelBase> panels = new Dictionary<string, PanelBase>();

	private void Awake()
	{
        PanelBase[] panelBase = FindObjectsOfType<PanelBase>();
        for (int i = 0; i < panelBase.Length; i++)
        {
			panels.Add(panelBase[i].gameObject.name, panelBase[i]);
		}

	}

	void Start()
    {
        
    }
    
    void Update()
    {
        
    }

	public void ActivatePanel(string panelName)
	{
		if (panels.TryGetValue(panelName, out PanelBase panel))
		{
			panel.ActivatePanel();
		}
	}

	// �������
	public void DeactivatePanel(string panelName)
	{
		if (panels.TryGetValue(panelName, out PanelBase panel))
		{
			panel.DeactivatePanel();
		}
	}

	// �����ض��ķ���
	public void TriggerPanelMethod(string panelName, string methodName, object[] parameters)
	{
		if (panels.TryGetValue(panelName, out PanelBase panel))
		{
			panel.CallSpecificMethod(methodName, parameters);
		}
	}
}

