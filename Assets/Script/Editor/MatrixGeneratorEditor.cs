using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(GoundBackItem))]
public class MatrixGeneratorEditor : Editor
{
	private Color targetColor = Color.white;  // �ڱ༭�����ж�����ɫ����
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();


		GoundBackItem generator = (GoundBackItem)target;


		GUILayout.BeginHorizontal();
		if (GUILayout.Button("�����ӽڵ�"))
		{
			generator.SetChinderPosition();
		}
		if (GUILayout.Button("�����ӽڵ�"))
		{
			if (generator.GetTopColorNumber() >= 10)
			{
				generator.RemoveTopColorObject();
			}
			else
			{
				Debug.Log(string.Format("δ������������ǰ��Ϊ{0}", generator.GetTopColorNumber()));

			}
		}
		GUILayout.EndHorizontal();
		// ��ʾ��ɫѡ����
		targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
		if (GUILayout.Button("�ı���ɫ"))
		{
			generator.SetAllColor(targetColor,4,ItemColorType.Gray);
		}
	}
}
