using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance;
	public GoundBackItem[,] GoundBackItemArray2D;

	private void Awake()
	{
		instance = this;


	}

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}
	}



	void Start()
	{

	}

	void Update()
	{

	}

	/// <summary>
	/// 初始化一个2维数组
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void SetGoundBack(int width, int height)
	{
		GoundBackItemArray2D = new GoundBackItem[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				GoundBackItemArray2D[i, j] = new GoundBackItem(i,j,$"{i},{j}");
			}
		}
	}

	/// <summary>
	/// 计算堆叠逻辑
	/// </summary>
	public void CalculateElimination(int x, int y) {
		if(GoundBackItemArray2D!=null){
			var GoundBackItemList = GetGoundBackItems(x, y);
			if (GoundBackItemList!=null&&GoundBackItemList.Count > 0)
			{
				int topNumber = 0;
				GoundBackItem top=null;
				//优先从颜色少的，堆叠到颜色多的那一堆
				//比较哪堆颜色多
				for (int i = 0; i < GoundBackItemList.Count; i++)
                {
					int ux=GoundBackItemList[i].GetNowColorNumber();
					if (ux>= topNumber) {
						top = GoundBackItemList[i];
						topNumber=ux;
					}
				}
				if (GoundBackItemArray2D[x, y].GetNowColorNumber()> top.GetNowColorNumber()) {
					var ubs = top.RemoveSurfaces(GoundBackItemArray2D[x, y].GetTopColor());
					if (ubs != null) GoundBackItemArray2D[x, y].AddSurfaces(ubs,MoveTweenType.Continuity,() => {
						CalculateElimination(x, y);
					});
				}
                else
                {
					var ubs = GoundBackItemArray2D[x, y].RemoveSurfaces(top.GetTopColor());
					if (ubs != null) top.AddSurfaces(ubs, MoveTweenType.Continuity, () => {
						CalculateElimination(x,y);
					});
				}
            }
			else {
				Debug.Log("无");
			}
		}
	}

	/// <summary>
	/// 获取当前颜色六边可以堆叠的数组
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private List<GoundBackItem> GetGoundBackItems(int x, int y) {
		List<GoundBackItem> ayx=new List<GoundBackItem>();
		List<Vector2Int> vector2s = new List<Vector2Int>() {new Vector2Int(x-1,y), new Vector2Int(x - 1, y+1), new Vector2Int(x, y-1), new Vector2Int(x, y+1), new Vector2Int(x + 1, y), new Vector2Int(x - 1, y) };
        for (int i = 0; i < vector2s.Count; i++)
        {
			Vector2Int vex = vector2s[i];
			if (vex.x >= 0 && vex.y >= 0 && vex.x < GoundBackItemArray2D.GetLength(0) && vex.y < GoundBackItemArray2D.GetLength(1))
			{
				if (GoundBackItemArray2D[x, y].GetTopColor()== GetGoundBackItem(vex.x, vex.y).GetTopColor()){
					ayx.Add(GetGoundBackItem(vex.x, vex.y));
				}
			}
		}
		return ayx;
	}


	/// <summary>
	/// 获取当前指定的组
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private GoundBackItem GetGoundBackItem(int x, int y) {
		if (x >= 0 && y >= 0 && x < GoundBackItemArray2D.GetLength(0) && y < GoundBackItemArray2D.GetLength(1))
		{
			var item = GoundBackItemArray2D[x, y];
			return item;
		}
		else
		{
			return null;
		}
	}
}

