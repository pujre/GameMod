using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SuTest))]
public class ColorTransitionEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		SuTest colorTransition = (SuTest)target;

		if (GUILayout.Button("Start Color Transition"))
		{
			colorTransition.StartColorTransition();
		}
	}
}