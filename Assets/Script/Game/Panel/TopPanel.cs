using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{
	public class TopPanel : PanelBase
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
		}

		// Update is called once per frame
		void Update()
		{

		}

		void OnClickEvent(GameObject but)
		{
			switch (but.name)
			{
				case "SettingBtn":
					UIManager.Instance.SetUiPanelAction("PausePanel", true);
					TYQEventCenter.Instance.Broadcast(OnEventKey.OnStop);
					break;
				case "IconBtn":
					Debug.Log("金币界面等待加入");
					break;
				case "PSBtn":
					Debug.Log("钻石界面等待加入");
					break;
				case "StoreBtn":
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