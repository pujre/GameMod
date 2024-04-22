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
	Douyin,
	WX,
	QQ,
}

public enum ADType
{
	/// <summary>
	/// ��� banner
	/// </summary>
	Banner,
	/// <summary>
	///  ������Ƶ  ��Ƶ
	/// </summary>
	Video,
	/// <summary>
	/// ����
	/// </summary>
	Interstitial,
	/// <summary>
	/// ԭ��ģ��
	/// </summary>
	Custom,
}