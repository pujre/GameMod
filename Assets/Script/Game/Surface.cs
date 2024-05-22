using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
	public ItemColorType SurfaceColor= ItemColorType.Gray;
	
	public void SetColor(int x) {
		SurfaceColor= (ItemColorType)x;
		transform.GetComponent<MeshRenderer>().material = UIManager.Instance.Colors[x];
	}

	public ItemColorType GetColorType()
	{
		return SurfaceColor;
	}
}
