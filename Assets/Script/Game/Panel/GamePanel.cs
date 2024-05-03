using Assets.Script.Game;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase
{
    public Text Prop_1Text, Prop_2Text, Prop_3Text;

	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnApplyProp.ToString(), DelegateCallback);
	}


	void Start()
    {
        var buts = transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < buts.Length; i++)
        {
            Button button = buts[i];
            button.onClick.AddListener(() => { OnClickEvent(button.gameObject);});
        }
		UpdatePropNumber();

	}

    void Update()
    {
        
    }

    void OnClickEvent(GameObject but) {
        switch (but.name) {
            case "Prop_1Btn":
				if (DataManager.Instance.GetData(OnDataKey.OnProp_1) > 0)
				{
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), "Prop_1");
				}
				else {
					Debug.Log("暂无该道具");
				}
				break;
            case "Prop_2Btn":
				if (DataManager.Instance.GetData(OnDataKey.OnProp_1) > 0)
				{
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), "Prop_2");
				}
				else
				{
					Debug.Log("暂无该道具");
				}
				break;
            case "Prop_3Btn":
				if (DataManager.Instance.GetData(OnDataKey.OnProp_1) > 0)
				{
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), "Prop_3");
				}
				else
				{
					Debug.Log("暂无该道具");
				}
				break;
            case "StopBtn":
                UIManager.Instance.SetUiPanelAction("PausePanel", true);
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnStop.ToString());
				break;
        }
    }

    void DelegateCallback(object[] args){
        switch (args[0])
        {
            case "Prop_1":
                Debug.Log("Use Prop_1");
                UpdatePropNumber();
				break;
            case "Prop_2":
				Debug.Log("Use Prop_2");
				UpdatePropNumber();
				break;
            case "Prop_3":
				Debug.Log("Use Prop_3");
				UpdatePropNumber();
				break;
			default:
                break;
        }
    }

    void UpdatePropNumber() {
		Prop_1Text.text = DataManager.Instance.GetData(OnDataKey.OnProp_1).ToString();
		Prop_2Text.text = DataManager.Instance.GetData(OnDataKey.OnProp_2).ToString();
		Prop_3Text.text = DataManager.Instance.GetData(OnDataKey.OnProp_3).ToString();

	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		// 使用反射来调用方法
		MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}