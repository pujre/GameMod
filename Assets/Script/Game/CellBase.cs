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
/// 背景格子
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
	/// 未访问过的点的集合
	/// </summary>
	public List<Vector2Int> VisitedNodes;
	/// <summary>
	/// 已经访问过的点的集合
	/// </summary>
	public List<Vector2Int> FoldNodes;

	/// <summary>
	/// 未访问过的点的集合
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
	/// 把一个点加入已经访问的点,并且把他移除出未访问的点的集合
	/// </summary>
	/// <param name="fold"></param>
	public void SetFoldNodes(Vector2Int fold) {
		if(!FoldNodes.Contains(fold)){ FoldNodes.Add(fold); }
		if (VisitedNodes.Contains(fold)) { VisitedNodes.Remove(fold); }
	}

};

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
	StarAll,
}

/// <summary>
/// 游戏事件
/// </summary>
public enum OnEventKey
{
	OnLoadGameLevel,//加载游戏关卡
	OnGameStar,//
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
		if (LevelDatas==null|| LevelDatas.Count==0) {
			Debug.Log(string.Format("获取指定{0}关卡数据为空", level));
			return null;
		}
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
		
	}

	
}

