using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class ArrayManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GameManager script = (GameManager)target;

		if (GUILayout.Button("初始化一个5*5的数组"))
		{
			script.SetGoundBack(5, 5);  // Example dimensions or read from user input

		}

		if (script.GoundBackItemArray2D != null)
		{
			//for (int y = 0; y < script.height; y++)
			//{
			//	EditorGUILayout.BeginHorizontal();
			//	for (int x = 0; x < script.width; x++)
			//	{
			//		script.array[x + y * script.width].intValue = EditorGUILayout.IntField(script.array[x + y * script.width].intValue);
			//		script.array[x + y * script.width].stringValue = EditorGUILayout.TextField(script.array[x + y * script.width].stringValue);
			//	}
			//	EditorGUILayout.EndHorizontal();
			//}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
