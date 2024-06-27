using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
				UIManager.Instance.SetUiPanelAction("GamePanel", true);
				GameManager.Instance.LoadLevel(1);
				GameManager.Instance.ScelfJob();
				GameManager.Instance.ScelfJob();
				GameManager.Instance.ScelfJob();
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
