using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : PanelBase
{
	private Toggle[] toggles;
	public Sprite[] images;
	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnStop.ToString(), DelegateCallback);
	}
	// Start is called before the first frame update
	void Start()
	{
		var buts = transform.GetComponentsInChildren<Button>();
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
				break;
			case "WinBtn":
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
				else {
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

	void DelegateCallback(object[] args)
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
		DelegateManager.Instance.RemoveEvent(OnEventKey.OnStop.ToString(), DelegateCallback);
	}
}
