using System;
using UnityEngine;
public class ADManager : MonoBehaviour
{
	public ADTarger ADTargerSystem = ADTarger.Unity;
	public IAdManager Ad;
	private static ADManager _instance;

	public static ADManager Instance {
		get
		{
			if (_instance == null) {
				_instance = FindObjectOfType<ADManager>();
				if (_instance==null) {
					GameObject gameObject = new GameObject("AdManager");
					_instance=gameObject.AddComponent<ADManager>();
					DontDestroyOnLoad(gameObject); // 保留在场景切换时不被销毁
					_instance.Init(); // 在设置 _instance 后初始化
				}
			}
			return _instance;
		}
	}

	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			Init();
		}
	}

	public void Init()
	{
		if(Ad != null){ return; }
		//友盟SDK接入
		Debug.Log("开始调用友盟SDK");
		UMeng.HellowTest();
		UMeng.Init("67b3fcf68f232a05f116ea81", true,true);

		UMeng.TrackEvent("EnterTheGame", "end");

		switch (ADTargerSystem)
		{
			case ADTarger.Unity:
				Ad = new UnityAD();
				Debug.Log("Unity AD 已初始化。");
				break;
			case ADTarger.Douyin:
				// 处理 Douyin AD 初始化
				break;
			case ADTarger.WX:
				Ad = new WXAD();
				Debug.Log("WX AD 已初始化。");
				break;
			case ADTarger.QQ:
				// 处理 QQ AD 初始化
				break;
			default:
				Debug.LogError("不支持的广告目标系统。");
				break;
		}
		if (Ad != null)
		{
			Ad.InitAD();
		}
		else {
			Debug.Log("Ad 为 null，当前平台为：" + ADTargerSystem.ToString());
		}
	}


	public void CreateAD(ADType adType) {
		Ad.CreateAD(adType);
	}

	public bool GetAD(ADType adType) { 
		return Ad.GetAD(adType);
	}

	public void ShowAD(ADType adType, Action<bool> callBack = null) {
		Ad.ShowAD(adType,callBack);
	}

	public void HideAD(ADType adType) { 
		Ad.HideAD(adType);
	}
}


