using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背景格子
/// </summary>
public class CellBase
{
    public Vector2Int GridPosition;
	public StackableItem Items;
}

/// <summary>
/// 一叠格子
/// </summary>
public class StackableItem {
	public List<Item> ListItems;
}

/// <summary>
/// 单个格子
/// </summary>
public class Item {
	public ItemColorType ItemColor;
	public int ItemIndex;
}

/// <summary>
/// 格子颜色枚举
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
/// 游戏事件
/// </summary>
public enum OnEventKey
{
	OngameStar,
	OnStop,
	OnGameOver,
	OnAd,
	OnApplyProp,
	OnCalculate,//消除事件
}

/// <summary>
/// 存储事件Key值
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
/// 移动动画事件
/// </summary>
public enum MoveTweenType { 
	One,
	Continuity,
}

public class LevelDataRoot {
	public LevelData[] LevelDatas;
	/// <summary>
	/// 获取指定关卡数据
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
		/// 道具ID
		/// </summary>
		public int ItemID;
		/// <summary>
		/// 道具数量
		/// </summary>
		public int ItemNumber;
		/// <summary>
		/// 道具描述
		/// </summary>
		public string Describe;
	}
}