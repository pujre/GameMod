using System;
using UnityEngine;
using WeChatWASM;

public class WXAD : IAdManager
{
	public string BannerAdUnit;
	public string VideoAdUnit;
	public string InterstitialAdUnit;
	public string CustomAdUnit;


	private WXBannerAd BannerAd;
	private WXRewardedVideoAd RewardedVideoAd;
	private WXInterstitialAd InterstitialAd;
	private WXCustomAd CustomAd;

	public void InitAD()
	{
		WX.InitSDK((code) =>
		{
			CreateBannerAd();

			CreateRewardedVideoAd();

			CreateCustomAd();
		});
	}


	public void CreateAD(ADType adType)
	{
		switch (adType)
		{
			case ADType.Banner:
				CreateBannerAd();
				break;
			case ADType.Video:
				CreateRewardedVideoAd();
				break;
			case ADType.Interstitial:
				break;
			case ADType.Custom:
				break;
			default:
				break;
		}
	}

	public bool GetAD(ADType adType)
	{
		switch (adType)
		{
			case ADType.Banner:
				return false;
			case ADType.Video:
				return false;
			case ADType.Interstitial:
				return false;
			case ADType.Custom:
				return false;
		}
		return false;
	}

	public void ShowAD(ADType adType,Action<bool> callBack=null)
	{
		switch (adType)
		{
			case ADType.Banner:
				BannerAd.Show();
				callBack?.Invoke(true);
				break;
			case ADType.Video:
				RewardedVideoAd.Show();
				break;
			case ADType.Interstitial:
				break;
			case ADType.Custom:
				CustomAd.Show();
				break;
			default:
				break;
		}
	}

	public void HideAD(ADType adType)
	{
		switch (adType)
		{
			case ADType.Banner:
				BannerAd.Hide();
				BannerAd.Destroy();
				break;
			case ADType.Video:
				break;
			case ADType.Interstitial:
				break;
			case ADType.Custom:
				CustomAd.Hide();
				break;
			default:
				break;
		}
	}


	#region ______________________________________________

	
	/// <summary>
	/// 创建banner广告
	/// </summary>
	private void CreateBannerAd()
	{
		BannerAd = WX.CreateFixedBottomMiddleBannerAd(BannerAdUnit, 30, 200);
	}



	/// <summary>
	/// 创建视频广告
	/// </summary>
	private void CreateRewardedVideoAd()
	{
		RewardedVideoAd = WX.CreateRewardedVideoAd(new WXCreateRewardedVideoAdParam()
		{
			adUnitId = VideoAdUnit,
		});
		RewardedVideoAd.OnLoad((res) =>
		{
			IADDebugManager.Log("RewardedVideoAd.OnLoad:" + JsonUtility.ToJson(res));
			var reportShareBehaviorRes = RewardedVideoAd.ReportShareBehavior(new RequestAdReportShareBehaviorParam()
			{
				operation = 1,
				currentShow = 1,
				strategy = 0,
				shareValue = res.shareValue,
				rewardValue = res.rewardValue,
				depositAmount = 100,
			});
			IADDebugManager.Log("ReportShareBehavior.Res:" + JsonUtility.ToJson(reportShareBehaviorRes));
		});
		RewardedVideoAd.OnError((WXADErrorResponse err) =>
		{
			IADDebugManager.Log("RewardedVideoAd.OnError:" + JsonUtility.ToJson(err));
		});
		RewardedVideoAd.OnClose((res) =>
		{
			IADDebugManager.Log("RewardedVideoAd.OnClose:" + JsonUtility.ToJson(res));
			RewardedVideoAd.Destroy();
			RewardedVideoAd = null;
			DelayManager.StartDelay(6f,() => {
				CreateRewardedVideoAd();
			});
		});
		RewardedVideoAd.Load();
	}


	private void CreatorInterstitialAd() {
		InterstitialAd = WX.CreateInterstitialAd(new WXCreateInterstitialAdParam {
			adUnitId= InterstitialAdUnit,
		});
	}



	/// <summary>
	/// 创建原生广告
	/// </summary>
	private void CreateCustomAd()
	{
		CustomAd = WX.CreateCustomAd(new WXCreateCustomAdParam()
		{
			adUnitId = CustomAdUnit,
			adIntervals = 30,
			style = {
				left = 0,
				top = 100,
			},
		});
		CustomAd.OnLoad((res) =>
		{
			IADDebugManager.Log("CustomAd.OnLoad:" + JsonUtility.ToJson(res));
		});
		CustomAd.OnError((res) =>
		{
			IADDebugManager.Log("CustomAd.onError:" + JsonUtility.ToJson(res));
		});
		CustomAd.OnHide(() =>
		{
			IADDebugManager.Log("CustomAd.onHide:");
			CustomAd.Destroy();
		});
		CustomAd.OnClose(() =>
		{
			IADDebugManager.Log("CustomAd.onClose");
		});
	}



	#endregion

}
