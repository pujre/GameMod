using System;

public interface IAdManager
{
	void InitAD();
	void CreateAD(ADType adType);
	bool GetAD(ADType adType);
	void ShowAD(ADType adType,Action<bool> callBack = null);
	void HideAD(ADType adType);
	
}

public enum ADTarger
{
	Unity,
	Douyin,
	WX,
	QQ,
}

public enum ADType
{
	/// <summary>
	/// 横幅 banner
	/// </summary>
	Banner,
	/// <summary>
	///  激励视频  视频
	/// </summary>
	Video,
	/// <summary>
	/// 插屏
	/// </summary>
	Interstitial,
	/// <summary>
	/// 原生模板
	/// </summary>
	Custom,
}