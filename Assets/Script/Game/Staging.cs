using TMPro;
using UnityEngine;

public class Staging : MonoBehaviour
{
	public GameObject SurfaceItem;
	public GameObject ADIcon;
	public GameObject ShareImage;
	public bool IsOn=false;

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
		if (!ADIcon) {
			ADIcon=PoolManager.Instance.GetObject("bottomText",transform);
			ADIcon.name = "ADIcon";
			ADIcon.GetComponent<TextMeshPro>().text = "临时格子";
			ADIcon.GetComponent<TextMeshPro>().fontSize = 11;
			ADIcon.transform.localPosition = new Vector3(0, 1.5f, -1.03f);
		}
		IsOn = !isAd;
		ADIcon.SetActive(isAd);
		ShareImage.SetActive(isAd);
		
	}


	public void ShowAD() {
		ADIcon.SetActive(false);
		ShareImage.SetActive(false);
		IsOn =true;
		Debug.Log("播放广告或者询问是否播放广告");
	}
}
