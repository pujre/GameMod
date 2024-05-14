using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GoundBackItem))]
public class MatrixGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GoundBackItem generator = (GoundBackItem)target;

		//if (GUILayout.Button("生成UGUI格子面板"))
		//{
		//	generator.GenerateMatrix();
		//}


		if (GUILayout.Button("排序子节点"))
		{
			generator.SetChinderPosition();
		}
	}
}
