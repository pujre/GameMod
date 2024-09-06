using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : PanelBase
{
	public GameObject Promp;
	public Text Prop_1Text, Prop_2Text, Prop_3Text, PromptText, PrompTitleText;
	public TextMeshProUGUI LevelText,LevelTargel;
	public Image ScoreFractionalBar;
	public List<Selected> SelectedList = new List<Selected>();
	public int NowScore = 0;
	public int TagerScore = 0;
	private Tween currentTween;
	public List<GameObject> PropBtnList=new List<GameObject>();
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
		foreach (var button in GetComponentsInChildren<Button>(true))
		{
			button.onClick.AddListener(() => OnClickEvent(button.gameObject));
			if (button.gameObject.name.Contains("Prop_")) {
				PropBtnList.Add(button.gameObject);
			}
		}
		UpdatePropNumber();
	}

	private void OnClickEvent(GameObject button)
	{
		if (button.name.Contains("Prop"))
		{
			int value = 0;
			LevelData levelData = GameManager.Instance.GetNowLevelData();
			string propName = "";
			int propId = 0;
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
					value = GameManager.Instance.GetNowLevelData().Item_1Number;
					break;
				case "Prop_2Btn":
					propName = "Prop_2";
					propId = levelData.Item_2ID;
					value = GameManager.Instance.GetNowLevelData().Item_2Number;
					break;
				case "Prop_3Btn":
					propName = "Prop_3";
					propId = levelData.Item_3ID;
					value = GameManager.Instance.GetNowLevelData().Item_3Number;
					break;
			}
			if (value > 0)
			{
				BtnAnim(button.transform.Find("Prop").gameObject);
				SetUIAction(false, propName);
				GameManager.Instance.SetUserProp(propId);
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(), propName);
			}
			else
			{
				UIManager.Instance.SetUiPanelAction("RewardPanel", true);
				UIManager.Instance.GetPanel("RewardPanel").GetComponent<RewardPanel>().ShowObtain(propId - 1);
			}
		}
		else
		{
			switch (button.name)
			{
				case "StopBtn":
					UIManager.Instance.SetUiPanelAction("PausePanel", true);
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnStop.ToString());
					break;
				case "PrompX":
					Promp.SetActive(false);
					SetUIAction(true,"");
					GameManager.Instance.CloneUserProp();
					break;
			}

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
	public void SetUIAction(bool action,string propName)
	{
		for (int i = 0; i < PropBtnList.Count; i++)
		{
			PropBtnList[i].SetActive(action);
		}
		Promp.transform.Find("1").gameObject.SetActive(!action);
		Promp.transform.Find("3").gameObject.SetActive(!action);
		Promp.transform.Find("2").gameObject.SetActive(!action);
		if (!action)
		{
			Promp.SetActive(true);
			switch (propName)
			{
				case "Prop_1":
					Promp.transform.Find("1").gameObject.SetActive(true);
					PrompTitleText.text = "锤子";
					PromptText.text = "破坏整组大饼";
					break;
				case "Prop_2":
					Promp.transform.Find("2").gameObject.SetActive(true);
					PrompTitleText.text = "转换";
					PromptText.text = "将大饼颜色修改为万能的";
					break;
				case "Prop_3":
					Promp.transform.Find("3").gameObject.SetActive(true);
					PrompTitleText.text = "互换";
					PromptText.text = "将两组大饼互换";
					break;
			}
		}
		else {
			Promp.SetActive(false);
		}
	}

	private void OnGameStar(object[] args) {
		NowScore = 0;
		ScoreFractionalBar.fillAmount = 0;
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
		UpdatePropText();
	}

	private void UpdatePropText()
	{
		Prop_1Text.text = GameManager.Instance.GetNowLevelData().Item_1Number.ToString();
		Prop_2Text.text = GameManager.Instance.GetNowLevelData().Item_2Number.ToString();
		Prop_3Text.text = GameManager.Instance.GetNowLevelData().Item_3Number.ToString();
	}

	// 实现基类的抽象方法
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		MethodInfo methodInfo = typeof(GamePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}
