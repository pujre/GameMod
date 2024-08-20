using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase
{
	public GameObject Promp;
	public Text Prop_1Text, Prop_2Text, Prop_3Text, PromptText, PrompTitleText;
	public Image ScoreFractionalBar;
	public List<Selected> SelectedList = new List<Selected>();
	public int NowScore = 0;
	public int TagerScore = 0;
	private Tween currentTween;
	private RectTransform buttonRectTransform;
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
		int propId= 0;
		if (currentTween != null && currentTween.IsActive())
		{
			currentTween.Kill(); // 停止当前的动画
			buttonRectTransform.localScale = Vector3.one; // 重置缩放
		}
		switch (button.name)
		{
			case "Prop_1Btn":
				propName = "Prop_1";
				propId = levelData.Item_1ID;
				value = GetPropValue(levelData.Item_1ID);
				break;
			case "Prop_2Btn":
				propName = "Prop_2";
				propId = levelData.Item_2ID;
				value = GetPropValue(levelData.Item_2ID);
				break;
			case "Prop_3Btn":
				propName = "Prop_3";
				propId = levelData.Item_3ID;
				value = GetPropValue(levelData.Item_3ID);
				break;
			case "StopBtn":
				UIManager.Instance.SetUiPanelAction("PausePanel", true);
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnStop.ToString());
				break;
			case "PrompX":
				Promp.SetActive(false);
				break;
		}

		if (value > 0)
		{
			BtnAnim(button.transform.Find("Prop").gameObject);
			SetUIAction(false);
			GameManager.Instance.UserProp(propId);
			DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), propName);
		}
		else
		{
			UIManager.Instance.SetUiPanelAction("RewardPanel", true);
		}
		AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");
	}


	public void BtnAnim(GameObject button) {
		buttonRectTransform = button.GetComponent<RectTransform>();
		currentTween = buttonRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)  // 放大到1.2倍，持续时间0.1秒
		.OnComplete(() =>
		{
			buttonRectTransform.DOScale(Vector3.one, 0.1f);  // 缩小回原来的大小，持续时间0.1秒
		});
	}

	/// <summary>
	/// 设置UI的展示与消失
	/// </summary>
	public void SetUIAction(bool action,Action callBack=null)
	{
		transform.Find("Prop_1Btn").gameObject.SetActive(action);
		transform.Find("Prop_2Btn").gameObject.SetActive(action);
		transform.Find("Prop_3Btn").gameObject.SetActive(action);
		Promp.SetActive(action);

		callBack!.Invoke();
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
		LevelData nowLevelData = GameManager.Instance.GetNowLevelData();

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
			Debug.Log($"在键 {itemId} 的PropNumber中找不到条目");
		}
	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		MethodInfo methodInfo = typeof(GamePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}
