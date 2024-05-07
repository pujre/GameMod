using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Surface))]
public class SurfaceGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		Surface generator = (Surface)target;

		//if (GUILayout.Button("生成UGUI格子面板"))
		//{
		//	generator.GenerateMatrix();
		//}


		if (GUILayout.Button("整理子物体坐标"))
		{
			//generator.AssignPositions();
		}
	}
}
