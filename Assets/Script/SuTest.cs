using UnityEngine;
using DG.Tweening;

public class SuTest : MonoBehaviour
{
	private Material targetMaterial;
	public float duration = 2.0f;

	void Start()
	{
	
	}

	void Update()
	{
		
	}
	


	public void StartColorDOTransition(Color targetColor) {
		if (targetMaterial != null && targetMaterial.name.Contains("Instance") == false)
		{
			targetMaterial = new Material(targetMaterial);
			GetComponent<Renderer>().material = targetMaterial;
		}
		targetMaterial.DOColor(targetColor, duration);
	}
}
