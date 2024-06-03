using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
public class CellBase
{
	public Vector2Int GridPosition;
	public StackableItem Items;
}

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

}

/// <summary>
/// ��Ϸ�¼�
/// </summary>
public enum OnEventKey
{
	OngameStar,//
	OnLoadGameLevel,//������Ϸ�ؿ�
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
	//��ͼ��С
	public Vector2Int ChapterSize;
	//��ס����Щ����
	public List<Vector2Int> GridLock;
	public List<Vector2Int> ChapterDefault;
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
		GridLock = new List<Vector2Int>();
		ChapterDefault = new List<Vector2Int>();
	}

	public bool IsLock(int x,int y) {
		if(GridLock==null) { return false; }
        for (int i = 0; i < GridLock.Count; i++)
        {
			if (GridLock[i].x==x&& GridLock[i].y==y) {
				return true;
			}
        }
        return false;
	}
}

