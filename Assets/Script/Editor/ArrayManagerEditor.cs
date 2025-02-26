using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;


[CustomEditor(typeof(GameManager))]
public class ArrayManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GameManager script = (GameManager)target;

		//if (GUILayout.Button(string.Format("初始化一个{0}*{1}的数组", script.MapSize.x, script.MapSize.y)))
		//{
		//	script.GenerateBoxMatrix(script.MapSize.x, script.MapSize.y);  // Example dimensions or read from user input
		//}
		//if (GUILayout.Button(string.Format("生成一个测试json,{0}", "5关数据")))
		//{
		//	TestLevel(5);
		//}

		EditorGUILayout.Space();

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

	public void TestLevel(int x)
	{
		LevelDataRoot root = new LevelDataRoot();
		root.LevelDatas = new List<LevelData>();
		for (int i = 0; i < x; i++)
		{
			LevelData levelData = new LevelData
			{
				Level = 1,
				ClearanceScore = 100,
				ColourNum = 3,
				MaxNum = 5,
				Item_1ID = 1,
				Item_1Number = 1,
				Describe_1 = "",
				Item_2ID = 2,
				Item_2Number = 1,
				Describe_2 = "",
				Item_3ID = 3,
				Item_3Number = 1,
				Describe_3 = "",
			};
			root.LevelDatas.Add(levelData);
		}

		string json = JsonConvert.SerializeObject(root);
		Debug.Log(json);
		string path = Path.Combine(Application.dataPath, "Resources/LevelData.json");
		File.WriteAllText(path, json);
		Debug.Log("成功");
	}
}
