using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
	private Vector3 BlockSize = new Vector3(5.22f, 0, 6);//偏移值
	private Vector3 BlockSizeDeviation = new Vector3(0, 0, 3);//偏移值
	public GameObject ItemParent;
	public Material[] DefaultORHightMaterial;
	private Vector3 StartPosition = new Vector3(0, 0, 0);
	public Vector2Int OnlastObj;

	private Camera Cam;
	private bool IsDragging = false;
	private GameObject SelectedObject;


	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public Dictionary<string, int> PropNumber = new Dictionary<string, int>();
	private int NowLevel=1;

	#region 局部变量private
	private RaycastHit hit;
	private Ray ray;
	private bool foundBottom=false;
	private Vector3 newPosition;
	private Transform lastHighlightedObject;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		Cam = Camera.main; // 获取主摄像机
		PropNumber = new Dictionary<string, int>();
		ResPath.Init();
	}

	void Start()
	{
		LoadlevelData();
		LoadLevel(1);
		ScelfJob();
		ScelfJob();
		ScelfJob();
		AudioManager.Instance.PlayBGM("bgm2（游戏界面）");
	}

	void Update()
	{
		if (!IsDragging&& SelectedObject==null && Input.GetMouseButtonDown(0))
		{
			 ray = Cam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform.GetComponent<SurfaceItem>() && hit.transform.GetComponent<SurfaceItem>().IsOnMove)
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
				newPosition = hitInfo.point;
				newPosition.y = SelectedObject.transform.position.y; // 保持Y轴不变
				SelectedObject.transform.position = newPosition;
			}

			// 使用 RaycastAll 检测所有碰撞
			RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
			foundBottom = false;

			foreach (RaycastHit hit in hits)
			{
				if (hit.transform.gameObject.tag == "bottom"&&
					hit.transform.GetComponent<GoundBackItem>()&&
					hit.transform.GetComponent<GoundBackItem>().IsAddSurface())
				{
					if (lastHighlightedObject != null)
					{
						lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
					}

					lastHighlightedObject = hit.transform;
					lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[1];
					foundBottom = true;
					break; // 找到目标对象后退出循环
				}
			}
			// 当离开各自区域得时候判定
			if (!foundBottom)
			{
				if (lastHighlightedObject != null)
				{
					lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
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
				lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
				obj.transform.position = lastHighlightedObject.transform.position+ new Vector3(0,1.2f,0);
				SurfaceItem si = obj.transform.GetComponent<SurfaceItem>();
				si.QueMoveEnd();
				GoundBackItem lj = lastHighlightedObject.GetComponent<GoundBackItem>();
				lj.AddSurfacesList(si.Surfaces);
				AudioManager.Instance.PlaySFX("Put（放下堆叠物）");
				si.Surfaces.Clear();
				CalculateElimination(lj.ItemPosition.x, lj.ItemPosition.y);
				GamePanel gamePanel = UIManager.Instance.GetPanel("GamePanel") as GamePanel;
				for (int i = 0; i < gamePanel.SelectedList.Count; i++)
				{
					if (gamePanel.SelectedList[i].SelfGameMove == SelectedObject)
					{
						gamePanel.SelectedList[i].SelfGameMove = null;
					}
				}
				ScelfJob();
				SelectedObject = null;
				Destroy(obj);
			}
			else {
				obj.transform.GetComponent<SurfaceItem>().QueMoveCancel();
				SelectedObject = null;
			}
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
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnLoadGameLevel.ToString());
	}


	public void ScelfJob() {
		GameObject obj = PoolManager.Instance.CreateGameObject("surfaceItem",GameObject.Find("Game/Panel"));
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localPosition = new Vector3(80, 1, -10);
		GamePanel gamePanel = UIManager.Instance.GetPanel("GamePanel") as GamePanel;
		for (int i = 0; i < gamePanel.SelectedList.Count; i++)
		{
			if (gamePanel.SelectedList[i].SelfGameMove == null)
			{
				gamePanel.SelectedList[i].SelfGameMove = obj;
				obj.GetComponent<SurfaceItem>().CreatorSurface(GetNowLevelData().ColourNum);
				obj.GetComponent<SurfaceItem>().QurStart(gamePanel.SelectedList[i].Pos);
				AudioManager.Instance.PlaySFX("Hu（出现三个新的堆叠物）");
				break;
			}
		}
		
	}


	/// <summary>
	/// 计算堆叠逻辑
	/// </summary>
	public void CalculateElimination(int x, int y)
	{
		if (GoundBackItemArray2D != null)
		{
			List<GoundBackItem> GoundBackItemList = GetGoundBackItems(x, y);
			var o = GoundBackItemArray2D[x, y];
			if (GoundBackItemList != null && GoundBackItemList.Count > 0)
			{
				if (GoundBackItemList.Count > 1)
				{
					Debug.Log("多个堆叠逻辑");
					//从大到小得顺序排列
					GoundBackItemList.Sort((item1, item2) => item2.GetNowColorNumber().CompareTo(item1.GetNowColorNumber()));
					// 定义一个递归函数来处理连续的动画
					void StartNextAnimation(int index)
					{
						if (index < GoundBackItemList.Count)
						{
							var currentItem = GoundBackItemList[index];
							if (index == GoundBackItemList.Count - 1 && o.GetNowColorNumber() >= currentItem.GetNowColorNumber())
							{
								GoundBackItem mos = currentItem;
								currentItem = o;
								o = mos;
							}
							var ops = currentItem.RemoveSurfaces(o.GetTopColor());
							o.AddSurfaces(ops, MoveTweenType.Continuity, () =>
							{
								// 当前动画完成后，递归调用下一个动画
								StartNextAnimation(index + 1);
							});
						}
						else {
							CalculateElimination(x, y);
						}
					}
					// 开始第一个动画
					StartNextAnimation(0);
				}
				else
				{
					Debug.Log("单个堆叠逻辑");
					//从大到小得顺序排列
					GoundBackItemList.Sort((item1, item2) => item2.GetNowColorNumber().CompareTo(item1.GetNowColorNumber()));
					GoundBackItem top = GoundBackItemList[GoundBackItemList.Count -1];
					var ubs = o.RemoveSurfaces(o.GetTopColor());
					OnlastObj = top.ItemPosition;
					top.AddSurfaces(ubs, MoveTweenType.Continuity, () =>
					{
						if (GetGoundBackItems(x, y).Count > 0)
						{
							CalculateElimination(x, y);
						}
						else
						{
							CalculateElimination(OnlastObj.x, OnlastObj.y);
						}
					});
				}
			}
			else
			{
				DelegateManager.Instance.TriggerEvent(OnEventKey.OnCalculate.ToString(), x, y);
			}
		}
	}



	/// <summary>
	/// 获取当前六边数组中是否有可消除的
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<GoundBackItem> GetGoundBackItems(int x, int y) {
		List<GoundBackItem> ayx=new List<GoundBackItem>();
		List<Vector2Int> vector2s;
		string debug = "";
		if (!(x % 2 == 0))
		{
			vector2s = new List<Vector2Int>() {new Vector2Int(x-1,y),new Vector2Int(x-1, y-1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y-1) };
		}
		else {
			vector2s = new List<Vector2Int>() {
			new Vector2Int(x-1,y),new Vector2Int(x-1, y+1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y+1)};
		}
		
        for (int i = 0; i < vector2s.Count; i++)
        {
			Vector2Int vex = vector2s[i];
			//Debug.Log(string.Format("判断当前周围的点，坐标为：x：{0}，y：{1}，1：{2}  2：{3}",vex.x, vex.y, GoundBackItemArray2D.GetLength(0), GoundBackItemArray2D.GetLength(0)));
			if (vex.x >= 0 && vex.y >= 0 && vex.x < GoundBackItemArray2D.GetLength(0) && vex.y < GoundBackItemArray2D.GetLength(1))
			{
				if (!GoundBackItemArray2D[x, y].IsAddSurface()&& !GetGoundBackItem(vex.x, vex.y).IsAddSurface() && 
					(GoundBackItemArray2D[x, y].GetTopColor()== GetGoundBackItem(vex.x, vex.y).GetTopColor())){
					ayx.Add(GetGoundBackItem(vex.x, vex.y));
					debug += string.Format("x:{0},y:{1}{2}",vex.x,vex.y,"\n");
				}
			}
		}
		//if(!string.IsNullOrEmpty(debug))Debug.Log(debug);
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
				GameObject block = PoolManager.Instance.CreateGameObject("bottoms", ItemParent);
				block.transform.localRotation = Quaternion.Euler(0, 0, 0);
				block.transform.localPosition = position;
				block.transform.localPosition += ItemParent.transform.position;
				block.transform.SetParent(ItemParent.transform);
				GoundBackItem goundBackItem = block.AddComponent<GoundBackItem>();
				goundBackItem.SetData(x, z, $"{x},{z}");
				if (GetNowLevelData().IsLock(x,z)) {
					goundBackItem.LockOrUnLockTheItem(true);
				}
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
		for (int i = ItemParent.transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(ItemParent.transform.GetChild(i).gameObject);
		}
	}
}

