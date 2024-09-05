using System;
using UnityEngine;
using WeChatWASM;

public class ADManager : SingletonMono<ADManager>
{
	public ADTarger ADTargerSystem;
	private IAdManager Ad;
	protected new void Awake()
	{
//#if UNITY_EDITOR
		Ad = new UnityAD();
//#else
		//switch (ADTargerSystem)
		//{
		//	case ADTarger.Douyin:
		//		break;
		//	case ADTarger.WX:
		//		Ad = new WXAD();
		//		break;
		//	case ADTarger.QQ:
		//		break;
		//	default:
		//		break;
		//}
		Ad.InitAD();
//#endif
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


