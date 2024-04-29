using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : PanelBase
{
    // Start is called before the first frame update
    void Start()
    {
		var buts = transform.GetComponentsInChildren<Button>();
		for (int i = 0; i < buts.Length; i++)
		{
			Button button = buts[i];
			button.onClick.AddListener(() => { OnClickEvent(button.gameObject); });
		}

	}

   
	void OnClickEvent(GameObject but)
	{
		switch (but.name)
		{
			case "Prop_1Btn":
				break;
			case "Prop_2Btn":
				break;
			case "Prop_3Btn":
				break;
			case "StopBtn":
				DelegateManager.Instance.Broadcast(OnEventKey.OnStop.ToString());
				break;
		}

	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		// 使用反射来调用方法
		MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}
