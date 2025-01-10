using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GoundBackItem))]
public class MatrixGeneratorEditor : Editor
{
	private Color targetColor = Color.white;  // 在编辑器类中定义颜色变量
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();


		GoundBackItem generator = (GoundBackItem)target;


		GUILayout.BeginHorizontal();
		if (GUILayout.Button("排序子节点"))
		{
			generator.SetChinderPosition();
		}
		if (GUILayout.Button("消除子节点"))
		{
			if (generator.GetTopColorNumber() >= 10)
			{
				generator.RemoveTopColorObject();
			}
			else
			{
				Debug.Log(string.Format("未满足条件，当前数为{0}", generator.GetTopColorNumber()));

			}
		}
		GUILayout.EndHorizontal();
		// 显示颜色选择器
		targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
		if (GUILayout.Button("改变颜色"))
		{
			generator.SetAllColor(targetColor,4,ItemColorType.Gray);
		}
	}
}
