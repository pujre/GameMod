using DG.Tweening;
using System;
using UnityEngine;
using TYQ;
public class Surface : MonoBehaviour
{
	public ItemColorType SurfaceColor = ItemColorType.Gray;
	public Material targetMaterial;
	public void SetColor(int x) {
		targetMaterial = transform.GetComponent<MeshRenderer>().material;
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


	public void TranslateColore(Color targetColor,Action callback=null)
	{
		SurfaceColor = ItemColorType.StarAll;
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		Color originalColor = renderer.material.color;
		targetMaterial = new Material(renderer.material);
		targetMaterial.color = originalColor;
		renderer.material = targetMaterial;
		targetMaterial.DOColor(targetColor, "_Color", 1f)
				 .SetEase(Ease.Linear)
				 .OnStart(() =>
				 {
					 
				 })
				 .OnComplete(() =>
				 {
					 callback?.Invoke();
				 });
	}
}
