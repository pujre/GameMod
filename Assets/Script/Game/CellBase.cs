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
public class StackableItem
{
	public List<Item> ListItems;
}

/// <summary>
/// 单个格子
/// </summary>
public class Item
{
	public ItemColorType ItemColor;
	public int ItemIndex;
}

/// <summary>
/// 格子颜色枚举
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
/// 游戏事件
/// </summary>
public enum OnEventKey
{
	OngameStar,//
	OnLoadGameLevel,//加载游戏关卡
	OnStop,//
	OnGameOverWin,//游戏结束胜利
	OnGameOverLose,//游戏结束失败
	OnAd,//看广告
	OnApplyProp,//使用道具
	OnBonusEvent,//加分
	OnCalculate,//消除事件
}

/// <summary>
/// 存储事件Key值
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
/// 移动动画事件
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
	/// 获取指定关卡数据
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
		Debug.Log(string.Format("获取指定{0}关卡数据为空", level));
		return null;
	}
}

public class LevelData
{
	public int Level;
	//地图大小
	public Vector2Int ChapterSize;
	//锁住了哪些格子
	public List<Vector2Int> GridLock;
	public List<Vector2Int> ChapterDefault;
	public int ClearanceScore;
	public int ColourNum;
	public int MaxNum;
	/// <summary>
	/// 道具ID
	/// </summary>
	public int Item_1ID;
	/// <summary>
	/// 道具数量
	/// </summary>
	public int Item_1Number;
	/// <summary>
	/// 道具描述
	/// </summary>
	public string Describe_1;
	/// <summary>
	/// 道具ID
	/// </summary>
	public int Item_2ID;
	/// <summary>
	/// 道具数量
	/// </summary>
	public int Item_2Number;
	/// <summary>
	/// 道具描述
	/// </summary>
	public string Describe_2;/// <summary>
							 /// 道具ID
							 /// </summary>
	public int Item_3ID;
	/// <summary>
	/// 道具数量
	/// </summary>
	public int Item_3Number;
	/// <summary>
	/// 道具描述
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

