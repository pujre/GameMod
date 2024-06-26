using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
	private Vector3 BlockSize = new Vector3(5.22f, 0, 6);//偏移值
	private Vector3 BlockSizeDeviation = new Vector3(0, 0, 3);//偏移值
	public GameObject ItemParent;
	public Material[] DefaultORHightMaterial;
	private Vector3 StartPosition = new Vector3(0, 0, 0);
	public List<Vector2Int> OperationPath=new List<Vector2Int>();

	private Camera Cam;
	private bool IsDragging = false;
	private GameObject SelectedObject;


	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public Dictionary<string, int> PropNumber = new Dictionary<string, int>();
	public List<InstructionData> FilterLinked = new List<InstructionData>();
	private int NowLevel = 1;

	#region 局部变量private
	private RaycastHit hit;
	private Ray ray;
	private bool foundBottom = false;
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
		AudioManager.Instance.PlayBGM("bgm2（游戏界面）");
	}


	void Update()
	{
		if (!IsDragging && SelectedObject == null && Input.GetMouseButtonDown(0))
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
				newPosition.y = 3; //SelectedObject.transform.position.y; // 保持Y轴不变
				SelectedObject.transform.position = newPosition;
			}

			// 使用 RaycastAll 检测所有碰撞
			RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
			foundBottom = false;

			foreach (RaycastHit hit in hits)
			{
				if (hit.transform.gameObject.tag == "bottom" &&
					hit.transform.GetComponent<GoundBackItem>() &&
					hit.transform.GetComponent<GoundBackItem>().IsAddSurface())
				{
					if (lastHighlightedObject != null)
					{
						lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
						lastHighlightedObject.transform.GetComponent<GoundBackItem>().SetvolumetricLine(false);
					}

					lastHighlightedObject = hit.transform;
					lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[1];
					lastHighlightedObject.GetComponent<GoundBackItem>().SetvolumetricLine(true);
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
					lastHighlightedObject.GetComponent<GoundBackItem>().SetvolumetricLine(false);

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
				lastHighlightedObject.transform.GetComponent<GoundBackItem>().SetvolumetricLine(false);

				obj.transform.position = lastHighlightedObject.transform.position + new Vector3(0, 1.2f, 0);
				SurfaceItem si = obj.transform.GetComponent<SurfaceItem>();
				si.QueMoveEnd();
				GoundBackItem lj = lastHighlightedObject.GetComponent<GoundBackItem>();
				lj.AddSurfacesList(si.Surfaces);
				AudioManager.Instance.PlaySFX("Put（放下堆叠物）");
				si.Surfaces.Clear();
				OperationPath.Clear();
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
			else
			{
				obj.transform.GetComponent<SurfaceItem>().QueMoveCancel();
				SelectedObject = null;
			}
		}
	}

	public LevelData GetNowLevelData()
	{
		return LevelDataRoot.GetLevelData(NowLevel);
	}

	private void LoadlevelData()
	{
		var levelDataJson = Resources.Load<TextAsset>("LevelData");
		LevelDataRoot = JsonConvert.DeserializeObject<LevelDataRoot>(levelDataJson.text);
	}

	public void LoadNextLevel() {
		LoadLevel(NowLevel+1);
	}

	public void LoadLevel(int level)
	{
		NowLevel = level;
		LevelData levedata = LevelDataRoot.GetLevelData(level);
		GenerateBoxMatrix(levedata.ChapterSize.x, levedata.ChapterSize.y);
		PropNumber.Clear();
		PropNumber.Add(levedata.Item_1ID.ToString(), levedata.Item_1Number);
		PropNumber.Add(levedata.Item_2ID.ToString(), levedata.Item_2Number);
		PropNumber.Add(levedata.Item_3ID.ToString(), levedata.Item_3Number);
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString());
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnLoadGameLevel.ToString());
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnGameStar.ToString());
	}


	public void ScelfJob()
	{
		GameObject obj = PoolManager.Instance.CreateGameObject("surfaceItem", GameObject.Find("Game/Panel"));
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
	#region MyRegion


	public void CalculateElimination(int x, int y)
	{
		if (GoundBackItemArray2D != null)
		{
			FilterLinked.Clear();
			List<Vector2Int> GoundBackItemList = GetGoundBackItems(x, y);
			if (GoundBackItemList == null || GoundBackItemList.Count == 0)
			{
				ChainCall();
				return;
			}
			ProcessCoordinates(GoundBackItemList);
			if (FilterLinked == null || FilterLinked.Count == 0)
			{
				ChainCall();
				return;
			}
			void StartNextAnimation(int index)
			{
				var ops = GoundBackItemArray2D[FilterLinked[index].StarVector2.x, FilterLinked[index].StarVector2.y].RemoveSurfaces();
				OperationPath.Add(FilterLinked[index].StarVector2);
				//Debug.Log("添加了坐标数据：x:" + FilterLinked[index].StarVector2.x + " Y:" + FilterLinked[index].StarVector2.y);
				GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].AddSurfaces(ops, () =>
				{
					if ((index + 1)>= FilterLinked.Count)
					{
						GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].RemoveTopColorObject(() =>
						{
							ChainCall();
						});
						return;
					}
					else
					{
						StartNextAnimation(index + 1);
					}
				});

			}
			StartNextAnimation(0);
		}
	}


	void ChainCall() {
		//Debug.Log("开始连锁数据");
		for (int i = OperationPath.Count - 1; i >= 0; i--)
		{
			var po = OperationPath[i];
			//Debug.Log(OperationPath.Count + "开始连锁数据----x:" + po.x + "y:" + po.y);
			OperationPath.Remove(po);
			if (GoundBackItemArray2D[po.x, po.y].GetComponent<GoundBackItem>().IsSurface())
			{
				//if (OperationPath.Count > 0)
				//{
				//	Debug.Log(OperationPath.Count + "余下的坐标的位置为 X:" + OperationPath[OperationPath.Count - 1].x + "Y:" + OperationPath[OperationPath.Count - 1].y);
				//}
				CalculateElimination(po.x, po.y);
				break;
			}
		}
	}

	#endregion




	/// <summary>
	/// 获取当前坐标顶部的颜色及其整个盘的颜色做坐标集合然后筛选
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetGoundBackItems(int x, int y)
	{
		var topColor = GetGoundBackItem(x, y).GetTopColor();
		List<Vector2Int> coordinates = new List<Vector2Int>();
		string log = "";
		for (int i = 0; i < GoundBackItemArray2D.GetLength(0); i++)
		{
			for (int j = 0; j < GoundBackItemArray2D.GetLength(1); j++)
			{
				if (GoundBackItemArray2D[i, j].IsSurface() && GoundBackItemArray2D[i, j].GetTopColor() == topColor)
				{
					coordinates.Add(new Vector2Int(i, j));
				}
			}
		}
		if (!coordinates.Contains(new Vector2Int(x,y))) {
			return null;
		}
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		List<Vector2Int> resultCoordinates = new List<Vector2Int>();
		foreach (var coord in coordinates)
		{
			if (!visited.Contains(coord))
			{
				List<Vector2Int> group = new List<Vector2Int>();
				UnitSDF.DFS(coord, coordinates, visited, group);
				if (group.Count > 1)
				{
					foreach (var position in group)
					{
						resultCoordinates.Add(new Vector2Int(position.x, position.y));
						log += position + "、";
					}
				}
			}
		}
		//Debug.Log("顶部相同颜色的坐标为：" + log);
		return resultCoordinates;
	}

	void ProcessCoordinates(List<Vector2Int> coordinates)
	{
		bool needsResorting = false;
		for (int i = 0; i < coordinates.Count - 1; i++)
		{
			if (!GetAroundPos(coordinates[i].x, coordinates[i].y).Contains(coordinates[i + 1]))
			{
				needsResorting = true;
				break;
			}
		}
		if (needsResorting)
		{
			Debug.Log("——需要重新排序——");
			FilterLinked = UnitSDF.FilterLinkedCoordinates(coordinates);
			Debug.Log("FilterLinked的长度为：" + FilterLinked.Count);
		}
		else {
			FilterLinked.Clear();
			for (int i = 0; i < coordinates.Count - 1; i++)
			{
				Debug.Log(string.Format("点{0}移动到点{1}", coordinates[i], coordinates[i + 1]));
				FilterLinked.Add(new InstructionData(coordinates[i], coordinates[i + 1]));
			}
		}
		
	}


	/// <summary>
	/// 将给定的数据按照指定的规则排序
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public List<Vector2Int> SortAndFilter(List<Vector2Int> coordinates)
	{
		if (coordinates == null || coordinates.Count == 0)return null;
		List<Vector2Int> sortedList = new List<Vector2Int>();
		HashSet<Vector2Int> remainingCoords = new HashSet<Vector2Int>(coordinates);
		Stack<(Vector2Int current, List<Vector2Int> path)> stack = new Stack<(Vector2Int, List<Vector2Int>)>();
		Vector2Int start = coordinates[0];
		for (int i = 0; i < coordinates.Count; i++)
		{
			if (GetAroundPos(coordinates[i].x, coordinates[i].y).Count==1) {
				start = coordinates[i];
				break;
			}
		}
		stack.Push((start, new List<Vector2Int> { start }));
		remainingCoords.Remove(start);

		while (stack.Count > 0)
		{
			var (current, path) = stack.Pop();
			if (path.Count == coordinates.Count)
			{
				return path;
			}
			var around = GetAroundPos(current.x, current.y);// 获取当前坐标周围的坐标
			var nextSteps = around.Where(remainingCoords.Contains).ToList();// 筛选出待访问的周围坐标
			if (nextSteps.Count == 0 && path.Count < coordinates.Count)
			{
				continue;
			}
			foreach (var next in nextSteps)// 遍历所有可行的下一步
			{
				List<Vector2Int> newPath = new List<Vector2Int>(path) { next };// 创建包含下一步的新路径
				stack.Push((next, newPath));// 将新路径和坐标压入栈
				remainingCoords.Remove(next); // 从待访问集合中移除当前坐标
			}
			if (nextSteps.Count == 0 && path.Count == coordinates.Count)
			{
				return path;
			}
		}
		return null;
	}







	// 检查特定点是否满足条件
	public bool CheckAndAddIfMatch(int x, int y, Vector2Int vex)
	{
		if (GoundBackItemArray2D[x, y].IsSurface() && GetGoundBackItem(vex.x, vex.y).IsSurface() &&
				(GoundBackItemArray2D[x, y].GetTopColor() == GetGoundBackItem(vex.x, vex.y).GetTopColor()))
		{
			return true;
		}
		return false;
	}


	/// <summary>
	/// 获取指定坐标的周围的点
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetAroundPos(int x, int y)
	{
		return UnitSDF.GetCreatorPos(x,y).Where(v => v.x >= 0 && v.y >= 0 && x < GoundBackItemArray2D.GetLength(0) 
		&& y < GoundBackItemArray2D.GetLength(1)
		).ToList();
	}


	/// <summary>
	/// 获取当前指定的组
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public GoundBackItem GetGoundBackItem(int x, int y)
	{
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
				if (GetNowLevelData().IsLock(x, z))
				{
					Debug.Log(string.Format("设置lock，x：{0}，y：{1}", x, z));
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

