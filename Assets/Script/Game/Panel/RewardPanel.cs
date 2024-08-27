using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : PanelBase
{
	public Text Tietletext;
	public Image TietleImage;
	public Sprite[] SpritesList;
	public string[] TietleList;
	private int TypeIndex;
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

	public void ShowObtain(int x) {
		Tietletext.text = TietleList[x];
		TietleImage.sprite = SpritesList[x];
		TietleImage.SetNativeSize();
		TypeIndex = x;
	}

	void OnClickEvent(GameObject but)
	{
		switch (but.name)
		{
			case "X":
				transform.gameObject.SetActive(false);
				break;
			case "ObtainPropsBtn":
				Debug.Log("�ۿ������ȡ");
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
