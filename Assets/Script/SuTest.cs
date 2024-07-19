using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuTest : MonoBehaviour
{
	public Material targetMaterial;
	[Header("Shader Parameters")]
	public Color targetColor1 = Color.white;
	public Color targetColor2 = Color.black;
	public float rainbowTime = 5f;
	public float gradientTime = 5f;
	public float checkerSize = 1f;

	private float elapsedTime = 0f;
	void Start()
    {
		if (targetMaterial == null)
		{
			Debug.LogError("Please assign a material with the custom shader.");
		}
		else
		{
			// Initialize shader properties
			targetMaterial.SetColor("_Color1", targetColor1);
			targetMaterial.SetColor("_Color2", targetColor2);
			targetMaterial.SetFloat("_RainbowTime", rainbowTime);
			targetMaterial.SetFloat("_GradientTime", gradientTime);
			targetMaterial.SetFloat("_CheckerSize", checkerSize);
		}
	}

    void Update()
    {
		if (targetMaterial != null)
		{
			elapsedTime += Time.deltaTime;
			targetMaterial.SetFloat("_Time", elapsedTime);
		}
	}
}
