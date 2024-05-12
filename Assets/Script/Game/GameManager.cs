using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Tooltip("网格方块预制体，通用型")]
	[Header("网格预制体")]
	public GameObject blockPrefab; // 预制方块
	public Vector2Int mapSize = new Vector2Int(5, 5);
	private Vector3 blockSize = new Vector3(5.22f, 0, 6);//偏移值
	private Vector3 blockSizeDeviation = new Vector3(0, 0, 3);//偏移值
	public GameObject ItemParent;
	private Vector3 startPosition = new Vector3(0, 0, 0);
	
	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public Dictionary<string, int> PropNumber = new Dictionary<string, int>();
	private int NowLevel=1;
	private static GameManager instance;
	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				// 在场景中查找 GameManager 实例
				instance = FindObjectOfType<GameManager>();

				// 如果实例不存在，则创建一个新的 GameObject 并添加 GameManager 组件
				if (instance == null)
				{
					GameObject go = new GameObject("Manager");
					instance = go.AddComponent<GameManager>();
				}
			}
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);  // 确保单例对象跨场景不被销毁
		}
		else if (instance != this)
		{
			Destroy(gameObject);  // 销毁多余的实例
		}
		PropNumber = new Dictionary<string, int>();
		LoadlevelData();
		LoadLevel(1);
	}

	



	void Start()
	{
		
	}

	void Update()
	{

	}

	public LevelData GetNowLevelData() {
		return LevelDataRoot.GetLevelData(NowLevel);
	}

	private void LoadlevelData() {
		var levelDataJson=Resources.Load<TextAsset>("LevelData");
		LevelDataRoot = JsonConvert.DeserializeObject<LevelDataRoot>(levelDataJson.text);
	}

	public void LoadLevel(int level) {
		NowLevel = level;
		LevelData levedata=LevelDataRoot.GetLevelData(level);
		GenerateBoxMatrix(levedata.ChapterSize.x,levedata.ChapterSize.y);
		PropNumber.Clear();
		PropNumber.Add(levedata.Item_1ID.ToString(), levedata.Item_1Number);

		PropNumber.Add(levedata.Item_2ID.ToString(), levedata.Item_2Number);

		PropNumber.Add(levedata.Item_3ID.ToString(), levedata.Item_3Number);
		Debug.Log("触发更新道具数量UI逻辑");
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString());
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


	/// <summary>
	/// 新建一个指定大小的空的地图网格
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public void GenerateBoxMatrix(int width, int height)
	{
		RemoveBoxMatrix();
		GoundBackItemArray2D = new GoundBackItem[width, height];
		bool isOn = true;
		for (int x = 0; x < width; x++)
		{
			for (int z = 0; z < height; z++)
			{
				Vector3 position = new Vector3(
					startPosition.x + (x == 0 ? 0 : x * blockSize.x) + blockSizeDeviation.x, 0,
					startPosition.z + (isOn ? z * blockSize.z + blockSizeDeviation.z : z * blockSize.z));
				GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, ItemParent.transform);
				block.transform.position = new Vector3(block.transform.position.x + ItemParent.transform.position.x,
					ItemParent.transform.position.y + block.transform.position.y,
					ItemParent.transform.position.z + block.transform.position.z);
				//block.transform.localScale = new Vector3(1, 1, 1);
				block.name = string.Format("Surface_{0},{1}", x, z);
				GoundBackItem goundBackItem = block.AddComponent<GoundBackItem>();
				goundBackItem.SetData(x, z, $"{x},{z}");
				GoundBackItemArray2D[x, z] = goundBackItem;
			}
			isOn = !isOn;
		}
	}

	/// <summary>
	/// 移除当前地图网格
	/// </summary>
	public void RemoveBoxMatrix()
	{
		int childCount = ItemParent.transform.childCount;
		for (int i = childCount - 1; i >= 0; i--)
		{
			Transform child = ItemParent.transform.GetChild(i);
			DestroyImmediate(child.gameObject);
		}
	}
}

