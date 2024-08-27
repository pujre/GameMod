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
				Debug.Log("观看广告后获取");
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
