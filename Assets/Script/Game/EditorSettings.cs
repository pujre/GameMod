using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorSettings : MonoBehaviour
{

	[Tooltip("网格方块预制体，通用型")]
	[Header("网格预制体")]
	public GameObject blockPrefab; // 预制方块
	[Header("地图尺寸")]
	public Vector2Int mapSize = new Vector2Int(5,5);
	[Header("初始坐标")]
	public Vector3 startPosition;
	[Header("模型大小基础值")]
	public Vector3 blockSize = new Vector3(5.22f,0,6);//偏移值
	[Header("偏移值")]
	public Vector3 blockSizeDeviation = new Vector3(0, 0, 3);//偏移值
	[Header("父类")]
	public GameObject ItemParent;
	private Transform CubeRect;//背景六边形网格的宽高

	//private Rect rect;//背景六边形网格的宽高

	public void GenerateMatrix()
	{
		//rect = blockPrefab.GetComponent<RectTransform>().rect;
		//bool isOn = false;
		//Vector2 pos = startPosition;
		//for (int x = 0; x < mapSize; x++)
		//{
		//	for (int y = 0; y < mapSize; y++)
		//	{
		//		Vector3 position = new Vector3(
		//			startPosition.x + x * rect.width - x * 42,
		//			startPosition.y + y * rect.height + (isOn ? rect.height / 2 : 0)
		//			, 0);
		//		GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, transform);
		//		block.transform.SetParent(ItemParent.transform);
		//		block.GetComponent<RectTransform>().localPosition = position;
		//		block.transform.localScale = new Vector3(1, 1, 1);
		//		block.name = string.Format("Image_{0},{1}", x, y);
		//	}
		//	isOn = !isOn;
		//}
	}

	public void GenerateBoxMatrix(){
		//GameManager.Instance.GenerateBoxMatrix(mapSize.x, mapSize.y);
	}

	public void RemoveBoxMatrix() {
		int childCount = ItemParent.transform.childCount;

		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = ItemParent.transform.GetChild(i);
			DestroyImmediate(child.gameObject);
		}
	}
}
