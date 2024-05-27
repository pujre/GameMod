using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase
{
    public Text Prop_1Text, Prop_2Text, Prop_3Text;
	public Image ScoreFractionalBar;
	public List<Selected> SelectedList = new List<Selected>();



	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnApplyProp.ToString(), DelegateCallback);
		DelegateManager.Instance.AddEvent(OnEventKey.OnBonusEvent.ToString(), Score);
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

	




	void OnClickEvent(GameObject but) {
		int Value = 0;
		switch (but.name) {
            case "Prop_1Btn":
				GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_1ID.ToString(), out Value);
				if (Value > 0)
				{
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), "Prop_1");
				}
				else {
					Debug.Log("暂无该道具");
				}
				break;
            case "Prop_2Btn":
				GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_2ID.ToString(), out Value);
				if (Value > 0)
				{
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), "Prop_2");
				}
				else
				{
					Debug.Log("暂无该道具");
				}
				break;
            case "Prop_3Btn":
				GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_3ID.ToString(), out Value);
				if (Value > 0)
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
		UpdatePropNumber();
	}

	void Score(object[] args)
	{
		if (args.Length >= 1 && args[0] is int score)
		{
			ScoreFractionalBar.fillAmount += score;
		}
	}

	void UpdatePropNumber() {

		int Prop_1Value = 0;
		int Prop_2Value = 0;
		int Prop_3Value = 0;
		LevelData nowLevelData = GameManager.Instance.GetNowLevelData();
		if (nowLevelData == null)
		{
			Debug.LogError("GetNowLevelData returned null");
			return;
		}

		if (!GameManager.Instance.PropNumber.TryGetValue(nowLevelData.Item_1ID.ToString(), out Prop_1Value))
		{
			Debug.LogError($"No entry found in PropNumber for key {nowLevelData.Item_1ID}");
		}

		GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_1ID.ToString(),out Prop_1Value);
		Prop_1Text.text = Prop_1Value.ToString();
		GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_2ID.ToString(), out Prop_2Value);
		Prop_2Text.text = Prop_2Value.ToString();
		GameManager.Instance.PropNumber.TryGetValue(GameManager.Instance.GetNowLevelData().Item_3ID.ToString(), out Prop_3Value);
		Prop_3Text.text = Prop_3Value.ToString();

	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		// 使用反射来调用方法
		MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}