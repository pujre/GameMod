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
public class StackableItem {
	public List<Item> ListItems;
}

/// <summary>
/// ��������
/// </summary>
public class Item {
	public ItemColorType ItemColor;
	public int ItemIndex;
}

/// <summary>
/// ������ɫö��
/// </summary>
public enum ItemColorType {
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
	OngameStar,
	OnStop,
	OnGameOver,
	OnAd,
	OnApplyProp,
	OnCalculate,//�����¼�
}

/// <summary>
/// �洢�¼�Keyֵ
/// </summary>
public enum OnDataKey { 
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
public enum MoveTweenType { 
	One,
	Continuity,
}

public class LevelDataRoot {
	public LevelData[] LevelDatas;
	/// <summary>
	/// ��ȡָ���ؿ�����
	/// </summary>
	/// <param name="level"></param>
	/// <returns></returns>
	public LevelData GetLevelData(int level) { 
		for (int i = 0; i < LevelDatas.Length; i++)
		{
			if (LevelDatas[i].Level == level) {
				return LevelDatas[i];
			}
		}
		return null;
	}

public class LevelData {
	public int Level;
	public Vector2Int ChapterSize;
	public Vector2Int GridLock;
	public Vector2Int ChapterDefault;
	public int ClearanceScore;
	public int ColourNum;
	public int MaxNum;
}

	public class LevelDataProp
	{
		/// <summary>
		/// ����ID
		/// </summary>
		public int ItemID;
		/// <summary>
		/// ��������
		/// </summary>
		public int ItemNumber;
		/// <summary>
		/// ��������
		/// </summary>
		public string Describe;
	}
}