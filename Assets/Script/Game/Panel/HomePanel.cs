using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{
	public class HomePanel : PanelBase
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
			//string path= Path.Combine(Application.dataPath, "Scenes", "Level1.unity");
			//Debug.Log(path);
		}

		// Update is called once per frame
		void Update()
		{

		}

		void OnClickEvent(GameObject but)
		{
			switch (but.name)
			{
				case "PalyBtn":
					transform.gameObject.SetActive(false);
					UIManager.Instance.SetUiPanelAction("GamePanel", true);
					GameManager.Instance.LoadLevel(1);
					break;
				case "":

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
