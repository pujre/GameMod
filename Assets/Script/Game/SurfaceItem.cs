using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class SurfaceItem : MonoBehaviour
{
	public bool IsOnMove = false; // �Ƿ������϶�
	public Vector3 QreVector3; // ��ʼ����
	private float Assign_Y = 0.65f;
	public List<Surface> Surfaces = new List<Surface>();

	public void QurStart(Vector3 pos)
	{
		GameObject bottomtext = Resources.Load<GameObject>("Prefab/bottomText");
		GameObject oth = Instantiate(bottomtext, Vector3.zero, Quaternion.Euler(90, 0, 0), transform);
		oth.transform.localPosition = new Vector3(0,(Surfaces.Count * Assign_Y), 0);
		oth.GetComponent<TextMeshPro>().text = GetTopColorNumber().ToString();
		QreVector3 = pos;
		MoveToPosition(pos, () => IsOnMove = true);
	}

	public void QueMoveCancel()
	{
		MoveToPosition(QreVector3, () => IsOnMove = true);
	}

	public void QueMoveEnd()
	{
		IsOnMove = false;
	}

	private void MoveToPosition(Vector3 pos, TweenCallback onComplete)
	{
		transform.DOMove(pos, 0.3f).OnComplete(onComplete);
	}


	public int GetTopColorNumber()
	{
		if (Surfaces == null || Surfaces.Count == 0) return 0;
		string colorTypeName = Surfaces[Surfaces.Count - 1].GetColorType().ToString();
		int colorNumber = 0;
		for (int i = Surfaces.Count - 1; i >= 0; i--)
		{
			if (colorTypeName == Surfaces[i].GetColorType().ToString() || Surfaces[i].GetColorType() == ItemColorType.StarAll)
			{
				colorNumber++;
			}
			else
			{
				break;
			}
		}
		return colorNumber;
	}


	/// <summary>
	/// ����ָ����ɫ�������surface
	/// </summary>
	/// <param name="colorNumber"></param>
	public void CreatorSurface(int colorNumber = 3)
	{
		List<int> colors = GenerateRandomColors(9, colorNumber);
		colors = GroupNumbersTogether(colors);

		for (int i = 0; i < colors.Count; i++)
		{
			GameObject gameObject = PoolManager.Instance.CreateGameObject("surface");
			gameObject.transform.SetParent(transform);
			gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + (i * Assign_Y), transform.position.z);
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.GetComponent<Surface>().SetColor(colors[i]);
			Surfaces.Add(gameObject.GetComponent<Surface>());
		}
	}

	private List<int> GenerateRandomColors(int count, int colorNumber)
	{
		List<int> colors = new List<int>();
		for (int i = 0; i < count; i++)
		{
			colors.Add(Random.Range(0, colorNumber));
		}
		return colors;
	}

	private void Shuffle<T>(List<T> list)
	{
		System.Random rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	private List<int> GroupNumbersTogether(List<int> lst)
	{
		// ����һ���ֵ����洢ÿ�����ֳ��ֵĴ���
		Dictionary<int, List<int>> countDict = lst.GroupBy(x => x).ToDictionary(g => g.Key, g => g.ToList());

		// �����ְ��շ���������б������ı�����˳��
		List<int> groupedList = new List<int>();
		List<int> keys = countDict.Keys.ToList();
		Shuffle(keys);  // ���Ҽ���˳��
		foreach (var key in keys)
		{
			groupedList.AddRange(countDict[key]);
		}

		return groupedList;
	}
}
