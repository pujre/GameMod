using TMPro;
using TYQ;
using UnityEngine;

public class Staging : MonoBehaviour
{
	public GameObject SurfaceItem;
	public GameObject ADIcon;
	public GameObject ShareImage;
	public bool IsOn=false;
	private void Awake()
	{
		TYQEventCenter.Instance.AddListener(OnEventKey.OnLoadGameLevel, LoadGameLevel);
	}


	void LoadGameLevel() {
		AD(true);
	}

	private void Start()
	{
		AD(true);
		if (!ShareImage) {
			ShareImage=transform.Find("ShareImage").gameObject;
		}
	}

	public bool IsStaging()
	{
		if (SurfaceItem==null&&!IsOn)
		{
			return false;
		}else
		{
			return true;
		}
	}
	public void AddAndRemoveStaging(GameObject surfaceItem = null){
		SurfaceItem = surfaceItem;
		transform.GetComponent<MeshCollider>().enabled = surfaceItem == null ? true :false;
		if (!SurfaceItem)
		{
			//IsOn = true;
			//AD(true);
		}
	}

	/// <summary>
	/// 设置是否解锁
	/// </summary>
	/// <param name="isAd"></param>
	public void AD(bool isAd)
	{
		if (!ADIcon)
		{
			ADIcon = PoolManager.Instance.GetObject("bottomText", transform);
			ADIcon.name = "ADIcon";
			ADIcon.GetComponent<TextMeshPro>().text = "临时格子";
			ADIcon.GetComponent<TextMeshPro>().fontSize = 12.5f;
			ADIcon.transform.localPosition = new Vector3(0, 1.5f, -1.03f);
			ADIcon.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(6, 6);
		}
		else {
			AudioManager.Instance.PlaySFX("Unlock（解锁新格子）");
		}
		IsOn = !isAd;
		ADIcon.SetActive(isAd);
		ShareImage.SetActive(isAd);
	}


	public void ShowAD() {
		ADManager.Instance.ShowAD(ADType.Video, (bool isShow) =>
		{
			if (isShow)
			{
				ADIcon.SetActive(false);
				ShareImage.SetActive(false);
				IsOn = true;
				Debug.Log("广告播放成功");
				GameManager.Instance.ShowPrompt("解锁成功");
			}
			else {
				Debug.Log("广告播放失败");
				GameManager.Instance.ShowPrompt("解锁失败");
			}
		});
		
	}
}
