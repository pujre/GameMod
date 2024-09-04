using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class CellBase
{
	public Vector2Int GridPosition;
	public StackableItem Items;

}

class InstDataCalculus
{
	public int Index;
	/// <summary>
	/// δ���ʹ��ĵ�ļ���
	/// </summary>
	public List<Vector2Int> VisitedNodes;
	/// <summary>
	/// �Ѿ����ʹ��ĵ�ļ���
	/// </summary>
	public List<Vector2Int> FoldNodes;

	/// <summary>
	/// δ���ʹ��ĵ�ļ���
	/// </summary>
	/// <param name="index"></param>
	/// <param name="visitedNodes"></param>
	public InstDataCalculus(int index, List<Vector2Int> visitedNodes)
	{
		if(VisitedNodes==null) VisitedNodes=new List<Vector2Int>();
		if (FoldNodes == null) FoldNodes = new List<Vector2Int>();
		Index = index;
		VisitedNodes = visitedNodes;
		
	}
	/// <summary>
	/// ��һ��������Ѿ����ʵĵ�,���Ұ����Ƴ���δ���ʵĵ�ļ���
	/// </summary>
	/// <param name="fold"></param>
	public void SetFoldNodes(Vector2Int fold) {
		if(!FoldNodes.Contains(fold)){ FoldNodes.Add(fold); }
		if (VisitedNodes.Contains(fold)) { VisitedNodes.Remove(fold); }
	}

};

/// <summary>
/// һ������
/// </summary>
public class StackableItem
{
	public List<Item> ListItems;
}

/// <summary>
/// ��������
/// </summary>
public class Item
{
	public ItemColorType ItemColor;
	public int ItemIndex;
}

/// <summary>
/// ������ɫö��
/// </summary>
public enum ItemColorType
{
	Gray,
	Red,
	Green,
	Blue,
	Yello,
	Organge,
	StarAll,
}

/// <summary>
/// ��Ϸ�¼�
/// </summary>
public enum OnEventKey
{
	OnLoadGameLevel,//������Ϸ�ؿ�
	OnGameStar,//
	OnStop,//
	OnGameOverWin,//��Ϸ����ʤ��
	OnGameOverLose,//��Ϸ����ʧ��
	OnAd,//�����
	OnApplyProp,//ʹ�õ���
	OnBonusEvent,//�ӷ�
	OnCalculate,//�����¼�
}

/// <summary>
/// �洢�¼�Keyֵ
/// </summary>
public enum OnDataKey
{
	Cion,
	OnProp_1,
	OnProp_2,
	OnProp_3,
	Sound_On,
	Music_On,
	Shake_On,
	Other,
}

/// <summary>
/// �ƶ������¼�
/// </summary>
public enum MoveTweenType
{
	One,
	Continuity,
}

public class LevelDataRoot
{
	public List<LevelData> LevelDatas;

	public LevelDataRoot() {
	}
	/// <summary>
	/// ��ȡָ���ؿ�����
	/// </summary>
	/// <param name="level"></param>
	/// <returns></returns>
	public LevelData GetLevelData(int level)
	{
		if (LevelDatas==null|| LevelDatas.Count==0) {
			Debug.Log(string.Format("��ȡָ��{0}�ؿ�����Ϊ��", level));
			return null;
		}
		foreach (LevelData item in LevelDatas)
		{
			if (item.Level == level)
			{
				return item;
			}
		}
		Debug.Log(string.Format("��ȡָ��{0}�ؿ�����Ϊ��", level));
		return null;
	}
}

public class LevelData
{
	public int Level;
	public int ClearanceScore;
	public int ColourNum;
	public int MaxNum;
	/// <summary>
	/// ����ID
	/// </summary>
	public int Item_1ID;
	/// <summary>
	/// ��������
	/// </summary>
	public int Item_1Number;
	/// <summary>
	/// ��������
	/// </summary>
	public string Describe_1;
	/// <summary>
	/// ����ID
	/// </summary>
	public int Item_2ID;
	/// <summary>
	/// ��������
	/// </summary>
	public int Item_2Number;
	/// <summary>
	/// ��������
	/// </summary>
	public string Describe_2;/// <summary>
							 /// ����ID
							 /// </summary>
	public int Item_3ID;
	/// <summary>
	/// ��������
	/// </summary>
	public int Item_3Number;
	/// <summary>
	/// ��������
	/// </summary>
	public string Describe_3;
	public LevelData() {
		
	}

	
}

