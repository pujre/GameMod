using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class OverPanel : PanelBase
{
	public GameObject Win;
	public GameObject Lose;

	private void Awake()
	{
		DelegateManager.Instance.AddEvent(OnEventKey.OnGameOverWin.ToString(), Over);
	}
	// Start is called before the first frame update
	void Start()
	{
		var buts = transform.GetComponentsInChildren<Button>(true);
		for (int i = 0; i < buts.Length; i++)
		{
			Button button = buts[i];
			Debug.Log("��ť����Ϊ��"+ button.gameObject.name);
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
		Debug.Log("����˰�ť��"+ but.name);
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
			case "LoseRe":
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
