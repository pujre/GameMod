using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
	public class UIManager : MonoBehaviour
	{
		private static UIManager instance;
		private Dictionary<string, PanelBase> PanelDic = new Dictionary<string, PanelBase>();
		public static UIManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UIManager();
				}
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		void FindAllPanel(){
			PanelBase[] allPanel = FindObjectsOfType<PanelBase>(true);
			for (int i = 0; i < allPanel.Length; i++)
			{
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

		public void SetUiPanelAction(string panelName,bool isOn){
			PanelBase p;
			PanelDic.TryGetValue(panelName, out p);
			if (p)
			{
				p.ActivatePanel(isOn);
			}
			else {
				Debug.Log(string.Format("未找到名为 {0} 的面板", panelName));
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