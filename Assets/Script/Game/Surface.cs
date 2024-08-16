using UnityEngine;

public class Surface : MonoBehaviour
{
	public ItemColorType SurfaceColor = ItemColorType.Gray;

	public void SetColor(int x) {
		SurfaceColor = (ItemColorType)x;
		transform.GetComponent<MeshRenderer>().material = UIManager.Instance.Colors[x];
	}

	public ItemColorType GetColorType()
	{
		return SurfaceColor;
	}

	public void SetColorType(ItemColorType colorType) {
		SurfaceColor = colorType;
	}

	public void TranslateColore(){
		SurfaceColor = ItemColorType.StarAll;
		transform.GetComponent<MeshRenderer>().material = UIManager.Instance.Colors[6];
	}
}
