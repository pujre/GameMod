using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase
{
	public Text Prop_1Text, Prop_2Text, Prop_3Text;
	public Image ScoreFractionalBar;
	public List<Selected> SelectedList = new List<Selected>();
	public int NowScore = 0;
	public int TagerScore = 0;

	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnApplyProp.ToString(), DelegateCallback);
		DelegateManager.Instance.AddEvent(OnEventKey.OnBonusEvent.ToString(), Score);
		DelegateManager.Instance.AddEvent(OnEventKey.OnLoadGameLevel.ToString(), LoadGameLevel);
		DelegateManager.Instance.AddEvent(OnEventKey.OnGameStar.ToString(), OnGameStar);
	}

	private void Start()
	{
		foreach (var button in GetComponentsInChildren<Button>())
		{
			button.onClick.AddListener(() => OnClickEvent(button.gameObject));
		}
		UpdatePropNumber();
	}

	private void OnClickEvent(GameObject button)
	{
		int value = 0;
		var levelData = GameManager.Instance.GetNowLevelData();
		var propName = "";

		switch (button.name)
		{
			case "Prop_1Btn":
				propName = "Prop_1";
				value = GetPropValue(levelData.Item_1ID);
				break;
			case "Prop_2Btn":
				propName = "Prop_2";
				value = GetPropValue(levelData.Item_2ID);
				break;
			case "Prop_3Btn":
				propName = "Prop_3";
				value = GetPropValue(levelData.Item_3ID);
				break;
			case "StopBtn":
				UIManager.Instance.SetUiPanelAction("PausePanel", true);
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnStop.ToString());
				break;
		}

		if (value > 0)
		{
			DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), propName);
		}
		else
		{
			Debug.Log("暂无该道具");
		}

		AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");
	}


	private void OnGameStar(object[] args) {
		NowScore = 0;
		ScoreFractionalBar.fillAmount = 0;
	}

	private int GetPropValue(int itemId)
	{
		GameManager.Instance.PropNumber.TryGetValue(itemId.ToString(), out int value);
		return value;
	}

	private void DelegateCallback(object[] args)
	{
		UpdatePropNumber();
	}

	private void Score(object[] args)
	{
		if (args.Length >= 1 && args[0] is int score)
		{
			NowScore += score;
			ScoreFractionalBar.fillAmount = (float)NowScore / TagerScore;

			if (NowScore >= TagerScore)
			{
				UIManager.Instance.SetUiPanelAction("OverPanel",true);
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnGameOverWin.ToString(),true);
			}
		}
	}

	private void LoadGameLevel(object[] args)
	{
		ScoreFractionalBar.fillAmount = 0;
		NowScore = 0;
		TagerScore = GameManager.Instance.GetNowLevelData().ClearanceScore;
	}

	private void UpdatePropNumber()
	{
		var nowLevelData = GameManager.Instance.GetNowLevelData();

		if (nowLevelData == null)
		{
			Debug.LogError("GetNowLevelData returned null");
			return;
		}

		UpdatePropText(nowLevelData.Item_1ID, Prop_1Text);
		UpdatePropText(nowLevelData.Item_2ID, Prop_2Text);
		UpdatePropText(nowLevelData.Item_3ID, Prop_3Text);
	}

	private void UpdatePropText(int itemId, Text propText)
	{
		if (GameManager.Instance.PropNumber.TryGetValue(itemId.ToString(), out int value))
		{
			propText.text = value.ToString();
		}
		else
		{
			Debug.LogError($"在键 {itemId} 的PropNumber中找不到条目");
		}
	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		MethodInfo methodInfo = typeof(GamePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}
