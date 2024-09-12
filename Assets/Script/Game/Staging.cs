using UnityEngine;

public class Staging : MonoBehaviour
{
	public GameObject SurfaceItem;

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
	}
}
