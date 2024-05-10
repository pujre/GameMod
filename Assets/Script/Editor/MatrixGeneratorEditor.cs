using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(EditorSettings))]
public class MatrixGeneratorEditor : Editor
{
	//public override void OnInspectorGUI()
	//{
	//	base.OnInspectorGUI();

	//	EditorSettings generator = (EditorSettings)target;

	//	//if (GUILayout.Button("生成UGUI格子面板"))
	//	//{
	//	//	generator.GenerateMatrix();
	//	//}


	//	if (GUILayout.Button("生成格子面板"))
	//	{
	//		generator.RemoveBoxMatrix();
	//		generator.GenerateBoxMatrix();
	//	}
	//}
}
