using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

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
		LoadLevel(1);
		ScelfJob();
		ScelfJob();
		ScelfJob();
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
				newPosition.y = 10; //SelectedObject.transform.position.y; // 保持Y轴不变
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
			List<GoundBackItem> GoundBackItemList = GetGoundBackItems(x, y);
			if (GoundBackItemList == null || GoundBackItemList.Count == 0 || !GoundBackItemList.Contains(GoundBackItemArray2D[x, y]))
			{
				return;
			}
			void StartNextAnimation(int index)
			{
				if ((index + 1) >= GoundBackItemList.Count) { return; }
				var currentItem = GoundBackItemList[index];
				var ops = currentItem.RemoveSurfaces();
				OperationPath.Add(currentItem.ItemPosition);
				Debug.Log("添加了坐标数据：x:" + currentItem.ItemPosition.x + " Y:" + currentItem.ItemPosition.y);
				GoundBackItemList[index + 1].AddSurfaces(ops, () =>
				{
					if ((index + 2) >= GoundBackItemList.Count)
					{
						GoundBackItemList[index + 1].RemoveTopColorObject(() =>
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
		Debug.Log("开始连锁数据");
		for (int i = OperationPath.Count - 1; i >= 0; i--)
		{
			var po = OperationPath[i];
			Debug.Log(OperationPath.Count+"开始连锁数据----x:" + po.x+"y:"+ po.y);
			OperationPath.Remove(po);
			if (GoundBackItemArray2D[po.x, po.y].GetComponent<GoundBackItem>().IsSurface())
			{
				if (OperationPath.Count>0) {
					Debug.Log(OperationPath.Count + "余下的坐标的位置为 X:" + OperationPath[OperationPath.Count - 1].x + "Y:" + OperationPath[OperationPath.Count - 1].y);
				}
				CalculateElimination(po.x, po.y);
				break;
			}
		}
	}

	#endregion




	/// <summary>
	/// 获取当前坐标顶部的颜色及其整个盘的颜色做坐标集合
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<GoundBackItem> GetGoundBackItems(int x, int y)
	{
		var topColor = GetGoundBackItem(x, y).GetTopColor();
		List<Vector2Int> coordinates = new List<Vector2Int>();
		Debug.Log(string.Format("等待处理的颜色为： Color:{0}", topColor.ToString()));
		string log = "";
		for (int i = 0; i < GoundBackItemArray2D.GetLength(0); i++)
		{
			for (int j = 0; j < GoundBackItemArray2D.GetLength(1); j++)
			{
				if (GoundBackItemArray2D[i, j].IsSurface() && GoundBackItemArray2D[i, j].GetTopColor() == topColor)
				{
					coordinates.Add(new Vector2Int(i, j));
					log += string.Format("({0},{1})、", i, j);
				}
			}
		}
		Debug.Log("顶部相同颜色的坐标为：" + log);
		List<GoundBackItem> groupedItems = ProcessCoordinates(coordinates);
		return groupedItems;
	}

	List<GoundBackItem> ProcessCoordinates(List<Vector2Int> coordinates)
	{
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		List<Vector2Int> resultCoordinates = new List<Vector2Int>();
		List<GoundBackItem> result = new List<GoundBackItem>();
		string log = "";
		foreach (var coord in coordinates)
		{
			if (!visited.Contains(coord))
			{
				List<Vector2Int> group = new List<Vector2Int>();
				DFS(coord, coordinates, visited, group);
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
		bool needsResorting = false;
		for (int i = 0; i < resultCoordinates.Count - 1; i++)
		{
			if (!GetAroundPos(resultCoordinates[i].x, resultCoordinates[i].y).Contains(resultCoordinates[i + 1]))
			{
				needsResorting = true;
				break;
			}
		}

		if (needsResorting)
		{
			Debug.Log("初步排序的数组为：" + log);
			Debug.Log("——重新排序——");
			log +="";
			resultCoordinates = SortAndFilter(resultCoordinates);
			Debug.Log("——重新排序后的数组长度为："+ resultCoordinates.Count);
		}
		foreach (var item in resultCoordinates)
		{
			result.Add(GoundBackItemArray2D[item.x, item.y]);
			log += item + "、";
		}
		Debug.Log("排序后的数组为：" + log);
		return result;
	}


	/// <summary>
	/// 筛选出独立的那些
	/// </summary>
	/// <param name="coord"></param>
	/// <param name="coordinates"></param>
	/// <param name="visited"></param>
	/// <param name="group"></param>
	void DFS(Vector2Int coord, List<Vector2Int> coordinates, HashSet<Vector2Int> visited, List<Vector2Int> group)
	{
		visited.Add(coord);
		group.Add(coord);

		foreach (var neighbor in GetAroundPos(coord.x, coord.y))
		{
			if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
			{
				DFS(neighbor, coordinates, visited, group);
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
		if (coordinates == null || coordinates.Count == 0)
		{
			return null;
		}

		List<Vector2Int> sortedList = new List<Vector2Int>();
		HashSet<Vector2Int> remainingCoords = new HashSet<Vector2Int>(coordinates);
		Stack<(Vector2Int current, List<Vector2Int> path)> stack = new Stack<(Vector2Int, List<Vector2Int>)>();

		Vector2Int start = coordinates[0];
		stack.Push((start, new List<Vector2Int> { start }));
		remainingCoords.Remove(start);

		while (stack.Count > 0)
		{
			var (current, path) = stack.Pop();

			if (path.Count == coordinates.Count)
			{
				return path;
			}

			var around = GetAroundPos(current.x, current.y);
			var nextSteps = around.Where(remainingCoords.Contains).ToList();

			if (nextSteps.Count == 0 && path.Count < coordinates.Count)
			{
				continue;
			}

			foreach (var next in nextSteps)
			{
				List<Vector2Int> newPath = new List<Vector2Int>(path) { next };
				stack.Push((next, newPath));
				remainingCoords.Remove(next);
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
		List<Vector2Int> vector2s;
		if (!(x % 2 == 0))
		{
			vector2s = new List<Vector2Int>() {new Vector2Int(x-1,y),new Vector2Int(x-1, y-1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y-1) };
		}
		else
		{
			vector2s = new List<Vector2Int>() {
			new Vector2Int(x-1,y),new Vector2Int(x-1, y+1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y+1)};
		}
		List<Vector2Int> filteredVectors = vector2s.Where(v => v.x >= 0 && v.y >= 0 && x < GoundBackItemArray2D.GetLength(0) && y < GoundBackItemArray2D.GetLength(1)).ToList();
		return filteredVectors;
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

