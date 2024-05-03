using Assets.Script.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : PanelBase
{

	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnStop.ToString(), DelegateCallback);
	}
	// Start is called before the first frame update
	void Start()
    {
		var buts = transform.GetComponentsInChildren<Button>();
		for (int i = 0; i < buts.Length; i++)
		{
			Button button = buts[i];
			button.onClick.AddListener(() => { OnClickEvent(button.gameObject);});
		}

		var toggles = transform.GetComponentsInChildren<Toggle>();
		for (int i = 0; i < toggles.Length; i++)
		{
			Toggle toggle = toggles[i];
			toggle.onValueChanged.AddListener((ison) => { OnValueChang(toggle.gameObject,ison);});
		}

	}

   
	void OnClickEvent(GameObject but)
	{
		switch (but.name)
		{
			case "XBtn":
			case "GoundBack":
				UIManager.Instance.SetUiPanelAction(gameObject.name,false);
				break;
			case "LoseBtn":
				break;
			case "WinBtn":
				break;
		}

	}

	void OnValueChang(GameObject toggle,bool isOn) {
		Debug.Log(string.Format("�����Toggle {0}��ֵΪ��{1}", toggle.name, isOn));
		switch (toggle.name)
		{
			case "MusicToggle":
				break;
			case "ShakeToggle":
				break;
			case "SoundToggle":
				break;
		}
	}

	void DelegateCallback(object[] args)
	{
		Debug.Log("��ͣ�����ʾ");
	}

	// ʵ�ֻ���ĳ��󷽷�
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		// ʹ�÷��������÷���
		MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}

	private void OnDestroy()
	{
		DelegateManager.Instance.RemoveEvent(OnEventKey.OnStop.ToString(), DelegateCallback);
	}
}
