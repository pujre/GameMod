using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{
	public class OverPanel : PanelBase
	{
		public GameObject Win;
		public GameObject Lose;

		private void Awake()
		{
			TYQEventCenter.Instance.AddListener<bool>(OnEventKey.OnGameOverWin, Over);
		}
		// Start is called before the first frame update
		void Start()
		{
			var buts = transform.GetComponentsInChildren<Button>(true);
			for (int i = 0; i < buts.Length; i++)
			{
				Button button = buts[i];
				button.onClick.AddListener(() => { OnClickEvent(button.gameObject); });
			}
		}

		public void GameOver(bool isWin)
		{
			Win.SetActive(isWin);
			Lose.SetActive(!isWin);
		}


		private void Over(bool isOn)
		{
			GameOver(isOn);
		}


		void OnClickEvent(GameObject but)
		{
			Debug.Log("点击了按钮：" + but.name);
			switch (but.name)
			{
				case "XBtn":
				case "GoundBack":
				case "ContinueBtn"://继续游戏
					gameObject.SetActive(false);
					GameManager.Instance.LoadNextLevel();
					break;
				case "ADBtn":
					break;
				case "LoseRe":
					break;
			}
			AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");
		}



		// 实现基类的抽象方法
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
