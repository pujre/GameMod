using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuTest))]
public class ColorTransitionEditor : Editor
{
	private Color targetColor = Color.white;  // 在编辑器类中定义颜色变量
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SuTest colorTransition = (SuTest)target;
		// 显示颜色选择器
		targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
		if (GUILayout.Button("Do Color Transition")) {
			colorTransition.StartColorDOTransition(targetColor);
		}
	}
}