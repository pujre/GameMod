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
		if (GUILayout.Button("消除子节点"))
		{
			if (generator.GetTopColorNumber() >= 10)
			{
				generator.RemoveTopColorObject(generator.ItemPosition.x, generator.ItemPosition.y);
			}
			else
			{
				Debug.Log(string.Format("未满足条件，当前数为{0}", generator.GetTopColorNumber()));

			}
		}
	}
}
