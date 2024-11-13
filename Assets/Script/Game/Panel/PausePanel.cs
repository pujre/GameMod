using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace TYQ
{
	public class PausePanel : PanelBase
	{
		private Toggle[] toggles;
		public Sprite[] images;
		public GameObject LoseBtn, WinBtn;
		private void Awake()
		{
			//TYQEventCenter.Instance.AddListener(OnEventKey.OnStop, DelegateCallback);
		}

		private void OnEnable()
		{
			bool isOn = UIManager.Instance.GetPanel("HomePanel").gameObject.activeSelf;
			LoseBtn.SetActive(!isOn);
			WinBtn.SetActive(!isOn);
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

			toggles = transform.GetComponentsInChildren<Toggle>();
			for (int i = 0; i < toggles.Length; i++)
			{
				Toggle toggle = toggles[i];
				toggle.onValueChanged.AddListener((ison) => { OnValueChang(toggle.gameObject, ison); });
			}

			UptatePausePanel();
		}


		void UptatePausePanel()
		{
			toggles.FirstOrDefault(t => t.gameObject.name == "ShakeToggle").isOn = DataManager.Instance.GetData(OnDataKey.Shake_On) == 0 ? true : false;
			toggles.FirstOrDefault(t => t.gameObject.name == "MusicToggle").isOn = DataManager.Instance.GetData(OnDataKey.Music_On) == 0 ? true : false;
			toggles.FirstOrDefault(t => t.gameObject.name == "SoundToggle").isOn = DataManager.Instance.GetData(OnDataKey.Sound_On) == 0 ? true : false;
			for (int i = 0; i < toggles.Length; i++)
			{
				OnValueChang(toggles[i].gameObject, toggles[i].isOn);
			}
		}


		

		void OnClickEvent(GameObject but)
		{
			switch (but.name)
			{
				case "XBtn":
				case "GoundBack":
					UIManager.Instance.SetUiPanelAction(gameObject.name, false);
					break;
				case "LoseBtn":
					GameManager.Instance.FreeUpSpace(3);
					GameManager.Instance.UpSpaceAll();
					UIManager.Instance.SetUiPanelAction("HomePanel", true);
					transform.gameObject.SetActive(false);
					break;
				case "WinBtn":
					GameManager.Instance.FreeUpSpace(3);
					transform.gameObject.SetActive(false);
					GameManager.Instance.LoadLevel(GameManager.Instance.NowLevel);
					break;
			}
			AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");

		}

		void OnValueChang(GameObject toggle, bool isOn)
		{
			//toggle.transform.Find("Checkmark").GetComponent<Image>().enabled=isOn;
			//toggle.transform.Find("Background").GetComponent<Image>().enabled = !isOn;
			toggle.transform.Find("Checkmark").GetComponent<Image>().sprite = images[isOn ? 0 : 1];
			toggle.transform.Find("Background").GetComponent<Image>().sprite = images[isOn ? 2 : 3];
			switch (toggle.name)
			{
				case "MusicToggle":
					DataManager.Instance.SetData(OnDataKey.Music_On, isOn ? 0 : 1);
					if (isOn)
					{
						AudioManager.Instance.PlayBGM("bgm2（游戏界面）");
					}
					else
					{
						AudioManager.Instance.StopBGM();
					}
					break;
				case "ShakeToggle":
					DataManager.Instance.SetData(OnDataKey.Shake_On, isOn ? 0 : 1);
					break;
				case "SoundToggle":
					DataManager.Instance.SetData(OnDataKey.Sound_On, isOn ? 0 : 1);
					break;
			}
			AudioManager.Instance.PlaySFX("click_ui（点击UI按钮）");
		}

		void DelegateCallback()
		{
			Debug.Log("暂停面板显示");
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
			TYQEventCenter.Instance.RemoveListener(OnEventKey.OnStop, DelegateCallback);
		}
	}
}