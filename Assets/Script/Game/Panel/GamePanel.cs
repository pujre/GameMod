using DG.Tweening;
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
		if (currentTween != null && currentTween.IsActive())
		{
			currentTween.Kill(); // ֹͣ��ǰ�Ķ���
			buttonRectTransform.localScale = Vector3.one; // ��������
		}
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
			BtnAnim(button.transform.Find("Prop").gameObject);
		}
		else
		{
			Debug.Log("���޸õ���");
		}

		AudioManager.Instance.PlaySFX("click_ui�����UI��ť��");
	}


	public void BtnAnim(GameObject button) {
		buttonRectTransform = button.GetComponent<RectTransform>();
		currentTween = buttonRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)  // �Ŵ�1.2��������ʱ��0.1��
		.OnComplete(() =>
		{
			buttonRectTransform.DOScale(Vector3.one, 0.1f);  // ��С��ԭ���Ĵ�С������ʱ��0.1��
		});
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
			Debug.Log($"�ڼ� {itemId} ��PropNumber���Ҳ�����Ŀ");
		}
	}

	// ʵ�ֻ���ĳ��󷽷�
	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		MethodInfo methodInfo = typeof(GamePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}
}
