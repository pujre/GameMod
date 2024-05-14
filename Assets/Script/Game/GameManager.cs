using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[Tooltip("网格方块预制体，通用型")]
	[Header("网格预制体")]
	public GameObject BlockPrefab; // 预制方块
	[Header("surface预制体")]
	public GameObject SurFacePrefab; // 预制方块
	public Vector2Int MapSize = new Vector2Int(5, 5);
	private Vector3 BlockSize = new Vector3(5.22f, 0, 6);//偏移值
	private Vector3 BlockSizeDeviation = new Vector3(0, 0, 3);//偏移值
	public GameObject ItemParent;
	public Material[] defaultORHightMaterial;
	private Vector3 StartPosition = new Vector3(0, 0, 0);
	private Camera Cam;
	private bool IsDragging = false;
	private GameObject SelectedObject;

	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public Dictionary<string, int> PropNumber = new Dictionary<string, int>();
	private int NowLevel=1;
	private RaycastHit hit;
	private RaycastHit hit2;
	private Ray ray;
	private Transform lastHighlightedObject;


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
		Cam = Camera.main; // 获取主摄像机
		PropNumber = new Dictionary<string, int>();
		LoadlevelData();
		LoadLevel(1);
		ScelfJob();
	}

	



	void Start()
	{
		
	}

	void Update()
	{
		if (!IsDragging&& SelectedObject==null && Input.GetMouseButtonDown(0))
		{
			 ray = Cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.GetComponent<SurfaceItem>().IsOnMove)
				{
					SelectedObject = hit.transform.gameObject;
					IsDragging = true;
				}
			}
		}

		if (IsDragging && SelectedObject != null)
		{
			 ray = Cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
			{
				Vector3 newPosition = hitInfo.point;
				newPosition.y = SelectedObject.transform.position.y; // 保持Y轴不变
				SelectedObject.transform.position = newPosition;
			}

			// 使用 RaycastAll 检测所有碰撞
			RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
			bool foundBottom = false;

			foreach (RaycastHit hit in hits)
			{

				if (hit.transform.gameObject.tag == "bottom")
				{
					if (lastHighlightedObject != null)
					{
						lastHighlightedObject.GetComponent<MeshRenderer>().material = defaultORHightMaterial[0];
					}

					lastHighlightedObject = hit.transform;
					lastHighlightedObject.GetComponent<MeshRenderer>().material = defaultORHightMaterial[1];
					foundBottom = true;
					break; // 找到目标对象后退出循环
				}
			}

			if (!foundBottom)
			{
				if (lastHighlightedObject != null)
				{
					lastHighlightedObject.GetComponent<MeshRenderer>().material = defaultORHightMaterial[0];
					lastHighlightedObject = null;
				}
			}
		}

		if (Input.GetMouseButtonUp(0) && IsDragging)
		{
			IsDragging = false;
			SnapToGrid(SelectedObject);
		}
	}

	private void SnapToGrid(GameObject obj)
	{
		if (obj != null)
		{
			if (lastHighlightedObject != null)
			{
				lastHighlightedObject.GetComponent<MeshRenderer>().material = defaultORHightMaterial[0];
				obj.transform.position =new Vector3(lastHighlightedObject.transform.position.x, lastHighlightedObject.transform.position.y+1, lastHighlightedObject.transform.position.z);
				SurfaceItem si = obj.transform.GetComponent<SurfaceItem>();
				si.QueMoveEnd();
				lastHighlightedObject.GetComponent<GoundBackItem>().AddSurfacesList(si.Surfaces);
				si.Surfaces.Clear();
				string[] parts = lastHighlightedObject.name.Split(',');
				CalculateElimination(int.Parse(parts[0]), int.Parse(parts[1]));
				ScelfJob();
				SelectedObject = null;
				Destroy(obj);
			}
			obj.transform.GetComponent<SurfaceItem>().QueMoveCancel();
			SelectedObject = null;
		}
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
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString());
	}


	public void ScelfJob() {
		GameObject obj = Instantiate(SurFacePrefab, new Vector3(80, 1, -10), Quaternion.identity, GameObject.Find("Game/Panel").transform);
		obj.GetComponent<SurfaceItem>().CreatorSurface(GetNowLevelData().ColourNum);
		obj.GetComponent<SurfaceItem>().QurStart(new Vector3(0,1,-10));
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
					StartPosition.x + (x == 0 ? 0 : x * BlockSize.x) + BlockSizeDeviation.x, 0,
					StartPosition.z + (isOn ? z * BlockSize.z + BlockSizeDeviation.z : z * BlockSize.z));
				GameObject block = Instantiate(BlockPrefab, position, Quaternion.Euler(90, 0, 0)/*Quaternion.identity*/, ItemParent.transform);
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

