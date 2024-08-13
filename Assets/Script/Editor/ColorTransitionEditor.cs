using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuTest))]
public class ColorTransitionEditor : Editor
{
	private Color targetColor = Color.white;  // �ڱ༭�����ж�����ɫ����
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SuTest colorTransition = (SuTest)target;
		// ��ʾ��ɫѡ����
		targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
		if (GUILayout.Button("Do Color Transition")) {
			colorTransition.StartColorDOTransition(targetColor);
		}
	}
}