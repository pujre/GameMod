using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class OverPanel : PanelBase
{
	private GameObject Win;
	private GameObject Lose;

	private void Awake()
	{
		Win = transform.Find("Win").gameObject;
		Lose = transform.Find("Lose").gameObject;
		DelegateManager.Instance.AddEvent(OnEventKey.OnBonusEvent.ToString(), Over);

	}
	// Start is called before the first frame update
	void Start()
	{
		var buts = transform.GetComponentsInChildren<Button>();
		for (int i = 0; i < buts.Length; i++)
		{
			Button button = buts[i];
			button.onClick.AddListener(() => { OnClickEvent(button.gameObject);});
		}
	}

	public void GameOver(bool isWin) {
		Win.SetActive(isWin);
		Lose.SetActive(!isWin);
	}


	private void Over(object[] args)
	{
		if (args.Length >= 1 && args[0] is bool isOn)
		{
			GameOver(isOn);
		}
	}


	void OnClickEvent(GameObject but)
	{
		switch (but.name)
		{
			case "XBtn":
			case "GoundBack":
			case "ContinueBtn"://������Ϸ
				gameObject.SetActive(false);
				GameManager.Instance.LoadNextLevel();
				break;
			case "ADBtn":
				break;
		}
		AudioManager.Instance.PlaySFX("click_ui�����UI��ť��");
	}



	// ʵ�ֻ���ĳ��󷽷�
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
