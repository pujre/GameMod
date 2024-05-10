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


