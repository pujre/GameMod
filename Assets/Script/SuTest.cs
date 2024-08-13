using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
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
		targetMaterial.DOColor(targetColor, duration)
					.SetEase(Ease.Linear)
					.OnStart(() =>
					{

					})
					.OnComplete(() =>
					{

					});
	}
}
