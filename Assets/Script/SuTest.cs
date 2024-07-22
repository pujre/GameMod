using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuTest : MonoBehaviour
{
	public Material targetMaterial;
	public Color targetColor;
	public float duration = 2.0f;

	private Color initialColor;
	private float elapsedTime = 0f;
	private bool isTransitioning = false;

	void Start()
	{
		if (targetMaterial == null)
		{
			Debug.LogError("Please assign a material with the standard shader.");
			enabled = false;
			return;
		}

		// Get the initial color of the material
		initialColor = targetMaterial.color;
	}

	void Update()
	{
		if (isTransitioning)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / duration);

			// Interpolate between the initial color and the target color
			targetMaterial.color = Color.Lerp(initialColor, targetColor, t);

			// Stop transitioning when the duration is reached
			if (t >= 1.0f)
			{
				isTransitioning = false;
			}
		}
	}

	// Call this method to start the color transition
	public void StartColorTransition()
	{
		if (!isTransitioning)
		{
			elapsedTime = 0f;
			initialColor = targetMaterial.color;
			isTransitioning = true;
		}
	}
}
