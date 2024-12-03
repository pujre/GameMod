using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{

	public class RewardPanel : PanelBase
	{
		public Text Tietletext, DescribeText;
		public Image TietleImage;
		public Sprite[] SpritesList;
		public string[] TietleList;
		public int TypeIndex;
		void Start()
		{
			var buts = transform.GetComponentsInChildren<Button>();
			for (int i = 0; i < buts.Length; i++)
			{
				Button button = buts[i];
				button.onClick.AddListener(() => { OnClickEvent(button.gameObject); });
			}
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void ShowObtain(int x)
		{
			Tietletext.text = TietleList[x];
			TietleImage.sprite = SpritesList[x];
			TietleImage.SetNativeSize();
			TypeIndex = x;
			switch (x)
			{
				case 0:
					DescribeText.text = "破坏整组大饼";
					break;
				case 1:
					DescribeText.text = "将两组大饼互换";
					break;
				case 2:
					DescribeText.text = "重新刷新三组大饼";
					break;
			}
		}

		void OnClickEvent(GameObject but)
		{
			switch (but.name)
			{
				case "X":
					transform.gameObject.SetActive(false);
					break;
				case "ObtainPropsBtn":
					Debug.Log("获取道具，道具id为：" + TypeIndex);
					ADManager.Instance.ShowAD(ADType.Video, (isOn) =>
					{
						switch (TypeIndex)
						{
							case 0:
								GameManager.Instance.GetNowLevelData().Item_1Number++;
								break;
							case 1:
								GameManager.Instance.GetNowLevelData().Item_2Number++;
								break;
							case 2:
								GameManager.Instance.GetNowLevelData().Item_3Number++;
								break;
							default:
								break;
						}
						TYQEventCenter.Instance.Broadcast(OnEventKey.OnApplyProp);
						transform.gameObject.SetActive(false);
					});
					break;
			}
			AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");

		}

		public override void CallSpecificMethod(string methodName, object[] parameters)
		{
			// 使用反射来调用方法
			MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
			methodInfo?.Invoke(this, parameters);
		}

		private void OnDestroy()
		{

		}
	}
}
