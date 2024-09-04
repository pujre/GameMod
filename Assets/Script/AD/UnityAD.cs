using System;

public class UnityAD : IAdManager
{
	public void CreateAD(ADType adType)
	{
	}

	public bool GetAD(ADType adType)
	{
		return true;
	}

	public void HideAD(ADType adType)
	{
	}

	public void InitAD()
	{
		
	}

	public void ShowAD(ADType adType, Action<bool> callBack = null)
	{
		callBack!.Invoke(true);
	}
}
