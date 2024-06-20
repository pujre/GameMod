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
	}
}
