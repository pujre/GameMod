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
	Red,
	Green, 
	Blue,
	Yello,
	Organge,

}