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
	Red,
	Green, 
	Blue,
	Yello,
	Organge,

}