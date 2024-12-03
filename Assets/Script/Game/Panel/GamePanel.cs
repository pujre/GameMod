using DG.Tweening;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TMPro;
using TYQ;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{
	public class GamePanel : PanelBase
	{
		public GameObject Promp;
		public Text Prop_1Text, Prop_2Text, Prop_3Text, PromptText, PrompTitleText;
		public TextMeshProUGUI LevelText, LevelTager;
		public Image ScoreFractionalBar;

		public int NowScore = 0;
		public int TagerScore = 0;
		public GameObject LevelTarget;
		public Text TagerLevelText, TagerScoreText;
		public GameObject Double_explosion_ukraine;

		private Tween currentTween;
		public List<GameObject> PropBtnList = new List<GameObject>();
		private RectTransform buttonRectTransform;
		private bool isSettlement = false;
		private void Awake()
		{
			TYQEventCenter.Instance.AddListener(OnEventKey.OnApplyProp, DelegateCallback);
			TYQEventCenter.Instance.AddListener<int>(OnEventKey.OnBonusEvent, Score);
			TYQEventCenter.Instance.AddListener(OnEventKey.OnLoadGameLevel, LoadGameLevel);
			TYQEventCenter.Instance.AddListener(OnEventKey.OnGameStar, OnGameStar);
			TYQEventCenter.Instance.AddListener(OnEventKey.OnStackingCompleted, GameWin);
			TYQEventCenter.Instance.AddListener(OnEventKey.ShowLevelTarge, ShowLevelTarge); 
		}



		private void Start()
		{
			foreach (var button in GetComponentsInChildren<Button>(true))
			{
				button.onClick.AddListener(() => OnClickEvent(button.gameObject));
				if (button.gameObject.name.Contains("Prop_"))
				{
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
						if (value > 0)
						{
							GameManager.Instance.ScelfJob(3);
							SetUIAction(true, "");
							GameManager.Instance.CloneUserProp();
							TYQEventCenter.Instance.Broadcast(OnEventKey.OnApplyProp);
							return;
						}
						break;
				}
				if (value > 0 && propName != "Prop_3")
				{
					BtnAnim(button.transform.Find("Prop").gameObject);
					SetUIAction(false, propName);
					GameManager.Instance.SetUserProp(propId);
					//TYQEventCenter.Instance.Broadcast(OnEventKey.OnApplyProp, propName);
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
						TYQEventCenter.Instance.Broadcast(OnEventKey.OnStop);
						break;
					case "PrompX":
						Promp.SetActive(false);
						SetUIAction(true, "");
						GameManager.Instance.CloneUserProp();
						break;
				}

			}
			AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");
		}


		public void BtnAnim(GameObject button)
		{
			buttonRectTransform = button.GetComponent<RectTransform>();
			currentTween = buttonRectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f)  // 放大到1.2倍，持续时间0.1秒
			.OnComplete(() =>
			{
				buttonRectTransform.DOScale(Vector3.one, 0.1f);  // 缩小回原来的大小，持续时间0.1秒
			});
		}

		/// <summary>
		/// 展示当前关卡的通关目标
		/// </summary>
		public void ShowLevelTarge() {
			GameManager.Instance.IsTouchInput = false;
			PropBtnList.ForEach(obj => obj.SetActive(false));
			LevelTarget.SetActive(true);
			TagerLevelText.text = "关卡" + GameManager.Instance.NowLevel.ToString();
			TagerScoreText.text = GameManager.Instance.GetNowLevelData().ClearanceScore.ToString();
			Transform LevelTargebackGound_2 = LevelTarget.transform.Find("LevelTargebackGound_2");
			TaTimeManager.Instance.StartTimer(1f, () => {
				Sequence sequence = DOTween.Sequence();
				sequence.Join(LevelTargebackGound_2.DOMove(transform.Find("Top/sign_1").position, 0.5f));
				sequence.Join(LevelTargebackGound_2.DOScale(Vector3.zero, 0.5f));
				sequence.OnComplete(() => {
					LevelTarget.SetActive(false);
					GameManager.Instance.IsTouchInput = true;
					PropBtnList.ForEach(obj => obj.SetActive(true));
					LevelTargebackGound_2.transform.localScale = Vector3.one;
					LevelTargebackGound_2.transform.localPosition = Vector3.one;
					GameManager.Instance.ScelfJob(3);
				});
				// 播放序列
				sequence.Play();				
			});
		}

		/// <summary>
		/// 设置道具UI的展示与消失
		/// </summary>
		public void SetUIAction(bool action, string propName)
		{
			for (int i = 0; i < PropBtnList.Count; i++)
			{
				PropBtnList[i].SetActive(action);
			}
			Promp.transform.Find("1").gameObject.SetActive(false);
			Promp.transform.Find("2").gameObject.SetActive(false);
			Promp.transform.Find("3").gameObject.SetActive(false);
			UIManager.Instance.GetPanel("Top").gameObject.SetActive(false);
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
						PrompTitleText.text = "互换";
						PromptText.text = "将两组大饼互换";
						break;
					case "Prop_3":
						Promp.transform.Find("3").gameObject.SetActive(true);
						PrompTitleText.text = "刷新";
						PromptText.text = "重新刷新三组大饼";
						break;
				}
			}
			else
			{
				Promp.SetActive(false);
				UIManager.Instance.GetPanel("Top").gameObject.SetActive(true);
			}
		}

		private void OnGameStar()
		{
			NowScore = 0;
			ScoreFractionalBar.fillAmount = 0;
		}


		private void DelegateCallback()
		{
			UpdatePropNumber();
		}

		private void Score(int score)
		{

			NowScore += score;
			LevelTager.text = string.Format("{0}/{1}", NowScore.ToString(), TagerScore.ToString());
			ScoreFractionalBar.fillAmount = (float)NowScore / TagerScore;


		}

		private void GameWin()
		{
			if (NowScore >= TagerScore)
			{
				if (!isSettlement) {
					isSettlement=true;
					GameManager.Instance.IsTouchInput = false;
					Double_explosion_ukraine.SetActive(true);
					Double_explosion_ukraine.GetComponentInChildren<ParticleSystem>().Play();
					TaTimeManager.Instance.StartTimer(3f, () => {
						UIManager.Instance.SetUiPanelAction("OverPanel", true);
						TYQEventCenter.Instance.Broadcast(OnEventKey.OnGameOverWin, true);
						Double_explosion_ukraine.SetActive(false);
						isSettlement = false;
						GameManager.Instance.IsTouchInput = true;
					});
				}
			}
		}

		private void LoadGameLevel()
		{
			ScoreFractionalBar.fillAmount = 0;
			NowScore = 0;
			LevelText.text = string.Format("第{0}关", GameManager.Instance.NowLevel);
			TagerScore = GameManager.Instance.GetNowLevelData().ClearanceScore;
			LevelTager.text = string.Format("{0}/{1}", NowScore.ToString(), TagerScore.ToString());
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
}