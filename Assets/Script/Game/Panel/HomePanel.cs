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
				transform.gameObject.SetActive(false);
				UIManager.Instance.SetUiPanelAction("GamePanel", true);
				GameManager.Instance.ScelfJob(3);
				break;
			case "":
				
				break;
		}
		AudioManager.Instance.PlaySFX("click_ui�����UI��ť��");

	}

	public override void CallSpecificMethod(string methodName, object[] parameters)
	{
		// ʹ�÷��������÷���
		MethodInfo methodInfo = typeof(PausePanel).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
		methodInfo?.Invoke(this, parameters);
	}

	private void OnDestroy()
	{
		
	}
}
