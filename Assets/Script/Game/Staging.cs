using TMPro;
using UnityEngine;

public class Staging : MonoBehaviour
{
	public GameObject SurfaceItem;
	public GameObject ADIcon;
	public bool IsOn=false;

	public bool IsStaging()
	{
		if (SurfaceItem)
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

	public void AD(bool isAd)
	{
		if (!ADIcon) {
			ADIcon=PoolManager.Instance.GetObject("bottomText",transform);
		}
		ADIcon.SetActive(isAd);
		ADIcon.GetComponent<TextMeshPro>().text = "½âËø";
		ADIcon.transform.localPosition = new Vector3 (0,1.5f,0);
	}
		
}
