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

		//if (GUILayout.Button("����UGUI�������"))
		//{
		//	generator.GenerateMatrix();
		//}


		if (GUILayout.Button("�����ӽڵ�"))
		{
			generator.SetChinderPosition();
		}
	}
}
