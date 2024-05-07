using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
	public ItemColorType SurfaceColor= ItemColorType.Gray;

	public ItemColorType GetColorType() {
		return SurfaceColor;
	}
}
