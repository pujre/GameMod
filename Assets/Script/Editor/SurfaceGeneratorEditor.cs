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

		//if (GUILayout.Button("����UGUI�������"))
		//{
		//	generator.GenerateMatrix();
		//}


		if (GUILayout.Button("��������������"))
		{
			//generator.AssignPositions();
		}
	}
}
