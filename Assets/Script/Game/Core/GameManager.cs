using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TYQ;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
	public bool DontSimcity = false;
	public List<Selected> SelectedList = new List<Selected>();
	public List<Staging> StagingList = new List<Staging>();
	public GameObject ItemParent;
	public Material[] DefaultORHightMaterial;
	public List<Vector2Int> OperationPath = new List<Vector2Int>();
	public List<Vector2Int> StartPos = new List<Vector2Int>();//作为起点的点

	private Camera Cam;
	public bool IsTouchInput = false;
	private bool IsDragging = false;
	public GameObject SelectedObject;
	public int NowLevel = 1;


	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public List<InstructionData> FilterLinked = new List<InstructionData>();
	private LevelData LevelData;

	private bool IsProp=false;
	private int IsPropAppUserID;
	private GameObject PropTranform_1;

	#region 局部变量private
	private RaycastHit hit;
	private Ray ray;
	private bool foundBottom = false;
	private Vector3 newPosition;
	private Transform lastHighlightedObject;
	private bool hasStacked=false;
	private Vector3 initialPosition;
	#endregion

	protected override void Awake()
	{
		base.Awake();
		Cam = Camera.main; // 获取主摄像机
		ResPath.Init();
	}

	void Start()
	{
		LoadlevelData();
		AudioManager.Instance.PlayBGM("bgm2（游戏界面）");
	}


	void Update()
	{
		if (IsTouchInput)
		{
			ray = Cam.ScreenPointToRay(Input.mousePosition);
			if (!IsDragging && SelectedObject == null && Input.GetMouseButtonDown(0))
			{
				if (Physics.Raycast(ray, out hit))
				{
					HandleClick(hit.transform.gameObject);
					initialPosition = hit.point;
				}
			}

			if (IsDragging && SelectedObject != null)
			{
				// 使用Vector3.Lerp或Vector3.MoveTowards来平滑移动
				if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
				{
					initialPosition = hitInfo.point;
					initialPosition.y = 3; //SelectedObject.transform.position.y; // 保持Y轴不变
				}
				SelectedObject.transform.position = Vector3.Lerp(SelectedObject.transform.position, initialPosition, Time.deltaTime * 15); // yourLerpSpeed根据需要调整
				// 使用 RaycastAll 检测所有碰撞
				RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
				foundBottom = false;

				foreach (RaycastHit hit in hits)
				{
					if (UserPropIsTouchMove(hit.transform)||NoUserPropIsTouchMove(hit.transform))
					{
						if (lastHighlightedObject != null)
						{
							lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
							lastHighlightedObject.transform.GetComponent<GoundBackItem>().SetvolumetricLine(false);
							if (IsProp) {
								PropTranform_1.transform.localPosition = Vector3.zero;
								PropTranform_1 = null;
							}
							
						}

						lastHighlightedObject = hit.transform;
						lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[1];
						lastHighlightedObject.GetComponent<GoundBackItem>().SetvolumetricLine(true);
						foundBottom = true;
						if (IsProp) { 
							PropTranform_1 = hit.transform.Find("ParentClass").gameObject;
							PropTranform_1.transform.position= SelectedObject.transform.parent.transform.position;
						}
						break; // 找到目标对象后退出循环
					}
					else if (!IsProp && hit.transform.gameObject.tag == "bottom" && hit.transform.gameObject.name == "Staging" && hit.transform.GetComponent<Staging>() && hit.transform.GetComponent<Staging>().IsStaging())
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
						if(lastHighlightedObject.GetComponent<GoundBackItem>()) lastHighlightedObject.GetComponent<GoundBackItem>().SetvolumetricLine(false);
						lastHighlightedObject = null;
						if (IsProp&& PropTranform_1)
						{
							PropTranform_1.transform.localPosition = Vector3.zero;
							PropTranform_1 = null;
						}
					}
				}
				
			}

			if (Input.GetMouseButtonUp(0) && IsDragging)
			{
				IsDragging = false;
				if (IsProp && SelectedObject != null && PropTranform_1 != null)
				{
					PropSnapToPropGrid(SelectedObject);
					if (lastHighlightedObject != null)
					{
						lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
						lastHighlightedObject.transform.GetComponent<GoundBackItem>().SetvolumetricLine(false);
					}
				} else if (IsProp && SelectedObject != null && PropTranform_1 == null) {
					SelectedObject.transform.parent.GetComponent<GoundBackItem>().QueMoveCancel();
					SelectedObject = null;
				}
				else if (!IsProp) {
					SnapToGrid(SelectedObject, ((lastHighlightedObject != null && lastHighlightedObject.name == "Staging") || lastHighlightedObject == null) ? false : true);
				}
			}
		}
	}

	/// <summary>
	/// 未使用道具的情况下判断拖动到的区块是否允许操作
	/// </summary>
	/// <param name="transform"></param>
	/// <returns></returns>
	private bool NoUserPropIsTouchMove(Transform hit) {
		return !IsProp && hit.transform.gameObject.tag == "bottom" 
		&& hit.transform.GetComponent<GoundBackItem>() &&
		hit.transform.GetComponent<GoundBackItem>().IsAddSurface();
	}

	/// <summary>
	/// 使用道具的情况下判断拖动到的区块是否允许操作
	/// </summary>
	/// <returns></returns>
	private bool UserPropIsTouchMove(Transform hit) {
		return IsProp&& IsPropAppUserID==2 && hit.transform.gameObject.tag == "bottom" &&
		hit.transform.GetComponent<GoundBackItem>() &&
		hit.transform.GetComponent<GoundBackItem>().IsCanBeOperated()&&
		hit.transform.GetComponent<GoundBackItem>().ParentClass!= SelectedObject;
	}




	/// <summary>
	/// 点击逻辑
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	private void HandleClick(GameObject obj) {
		if (!IsProp && obj.transform.GetComponent<SurfaceItem>() && obj.transform.GetComponent<SurfaceItem>().IsOnMove)
		{
			SelectedObject = obj.transform.gameObject;
			IsDragging = true;
		}
		else if (!IsProp && hit.transform.gameObject.tag == "bottom" && hit.transform.gameObject.name == "Staging" && hit.transform.GetComponent<Staging>() && !hit.transform.GetComponent<Staging>().IsStaging())
		{
			hit.transform.GetComponent<Staging>().ShowAD();
		}
		else if (!IsProp && obj.transform.GetComponent<GoundBackItem>() && obj.transform.GetComponent<GoundBackItem>().IsLock)
		{
			ADManager.Instance.ShowAD(ADType.Video, (isOn) =>
			{
				if (isOn)
				{
					obj.transform.GetComponent<GoundBackItem>().IsLock = false;
					obj.transform.GetComponent<GoundBackItem>().DisplayNumbers(true, "");
					AudioManager.Instance.PlaySFX("Unlock（解锁新格子）");
				}
				else
				{

				}
			});
		}
		else
		if (IsProp && obj.transform.GetComponent<GoundBackItem>() && obj.transform.GetComponent<GoundBackItem>().IsSurface())
		{
			switch (IsPropAppUserID)
			{
				case 1:
					SelectedObject = obj.transform.gameObject;
					UserProp(1);
					break;
				case 2:
					SelectedObject = obj.transform.Find("ParentClass").gameObject;
					IsDragging = true;
					#region Old prop script
					//SelectedObject = obj.transform.gameObject;
					//UserProp(2);
					//Debug.Log("PropTranform_1: " + (PropTranform_1==null? "  n ": PropTranform_1.gameObject.name));
					//if (PropTranform_1 == null)
					//{
					//	PropTranform_1 = obj.transform.gameObject;
					//	PropTranform_1.GetComponent<GoundBackItem>().DisplayNumbers(true, "换");
					//}
					//else if (PropTranform_1 != null && PropTranform_1 != obj.transform.gameObject)
					//{
					//	obj.transform.GetComponent<GoundBackItem>().PropPositionChange(PropTranform_1.GetComponent<GoundBackItem>());
					//	SelectedObject = PropTranform_1;
					//	UserProp(2);
					//}
					#endregion
					break;
				case 3:
					
					break;
			}
		}
		else
		if (obj.transform.GetComponent<GoundBackItem>() && obj.transform.GetComponent<GoundBackItem>().IsLock)
		{

		}
	}


	/// <summary>
	/// 使用道具
	/// </summary>
	/// <param name="propMoveTager">拖到了目标得地方得那个物体</param>
	private void PropSnapToPropGrid(GameObject propMoveTager)
	{
		if (propMoveTager != null && SelectedObject.transform.parent != propMoveTager.transform)
		{
			OperationPath.Add(PropTranform_1.transform.parent.GetComponent<GoundBackItem>().ItemPosition);
			propMoveTager.transform.parent.GetComponent<GoundBackItem>().PropPositionChange(PropTranform_1.transform.parent.GetComponent<GoundBackItem>());
			SelectedObject = PropTranform_1;
			UserProp(2);
		}
	}

	private void SnapToGrid(GameObject obj,bool isOn)
	{
		if (obj != null)
		{
			if (lastHighlightedObject != null)
			{
				lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
				obj.transform.position = lastHighlightedObject.transform.position + new Vector3(0, 1.2f, 0);
				SurfaceItem si = obj.transform.GetComponent<SurfaceItem>();
				if (isOn)
				{
					GoundBackItem lj = lastHighlightedObject.GetComponent<GoundBackItem>();
					if (lj) {
						lj.SetvolumetricLine(false);
						si.QueMoveEnd();
						lj.AddSurfacesList(si.Surfaces);
						AudioManager.Instance.PlaySFX("Put（放下堆叠物）");
						si.Surfaces.Clear();
						lj.DisplayNumbers(true);
						//OperationPath.Clear();
						if (!DontSimcity) {
							CalculateElimination(lj.ItemPosition.x, lj.ItemPosition.y, 0);
							LogManager.Instance.Log(string.Format("-放置- 了堆叠堆，X:{0},Y{1}", lj.ItemPosition.x, lj.ItemPosition.y));

						}
					}
					Destroy(obj);

				}
				else {
					Staging staging = lastHighlightedObject.GetComponent<Staging>();
					if (staging) {
						si.ChangeInitialPosition(lastHighlightedObject.position + new Vector3(0, 1.2f, 0));
						si.AddStaging(staging);
						staging.AddAndRemoveStaging(obj);
					}
                }
					for (int i = 0; i < SelectedList.Count; i++)
					{
						if (SelectedList[i].SelfGameMove == SelectedObject)
						{
							SelectedList[i].SelfGameMove = null;
						}
					}
				if (GetSelectedNum()==0)
				{
					ScelfJob(3);
				}
			}
			else
			{
				obj.transform.GetComponent<SurfaceItem>().QueMoveCancel();
			}
			SelectedObject = null;
		}
	}

	public int GetSelectedNum()
	{
		int number = 0;
		for (int i = 0; i < SelectedList.Count; i++)
		{
			if (SelectedList[i].SelfGameMove != null)
			{
				number++;
			}
		}
		return number;
	}

	/// <summary>
	/// 空出指定数量的空位
	/// </summary>
	/// <param name="x"></param>
	public void FreeUpSpace(int x)
	{
		int number = GetSelectedNum();
		int upSpace = SelectedList.Count - number;
		if (x > upSpace)
		{
			for (int i = 0; i < SelectedList.Count; i++)
			{
				if (SelectedList[i].SelfGameMove != null)
				{
					SelectedList[i].SelfGameMove.GetComponent<SurfaceItem>().SufaDestroy();
					SelectedList[i].SelfGameMove = null;
					upSpace++;
					if (x - upSpace == 0)
					{
						break;
					}
				}
			}
		}
	}

	public void UpSpaceAll()
	{
		for (int i = 0; i < StagingList.Count; i++)
		{
			if (StagingList[i].SurfaceItem != null)
			{
				StagingList[i].SurfaceItem.GetComponent<SurfaceItem>().SufaDestroy();
				StagingList[i].AddAndRemoveStaging (null);
			}
		}
	}

	public LevelData GetNowLevelData()
	{
		if (LevelData == null || LevelData.Level != NowLevel)
		{
			LevelData = LevelDataRoot.GetLevelData(NowLevel);
		}		
		return LevelData;
	}

	private void LoadlevelData()
	{
		var levelDataJson = Resources.Load<TextAsset>("LevelData");
		LevelDataRoot = LitJson.JsonMapper.ToObject<LevelDataRoot>(levelDataJson.text);
		
	}

	public void LoadNextLevel()
	{
		LoadLevel(NowLevel + 1);
	}

	/// <summary>
	/// 从新加载当前关卡
	/// </summary>
	public void ReLoadLevel() {
		LoadLevel(NowLevel);
	}

	public void LoadLevel(int level)
	{
		if (level>=5) {
			level = 4;
		}
		NowLevel = level;
		LevelData = LevelDataRoot.GetLevelData(level);
		//GenerateBoxMatrix(levedata.ChapterSize.x, levedata.ChapterSize.y);
		UpSpaceAll();
		LoadGenerateBoxMatrix();
		TYQEventCenter.Instance.Broadcast(OnEventKey.OnApplyProp);
		TYQEventCenter.Instance.Broadcast(OnEventKey.OnLoadGameLevel);
		TYQEventCenter.Instance.Broadcast(OnEventKey.ShowLevelTarge);
	}


	public void ScelfJob(int x=1)
	{
		FreeUpSpace(x);
		for (int j = 0; j < x; j++)
		{
			for (int i = 0; i < SelectedList.Count; i++)
			{
				if (SelectedList[i].SelfGameMove == null)
				{
					GameObject obj = PoolManager.Instance.GetObject("surfaceItem", GameObject.Find("Game/Panel").transform);
					obj.transform.localRotation = Quaternion.identity;
					obj.transform.localPosition = new Vector3(80, 1, -18);
					SelectedList[i].SelfGameMove = obj;
					obj.GetComponent<SurfaceItem>().CreatorSurface(GetNowLevelData().ColourNum);
					obj.GetComponent<SurfaceItem>().QurStart(SelectedList[i].Pos);
					AudioManager.Instance.PlaySFX("Hu（出现三个新的堆叠物）");
					break;
				}
			}
		}
		
	}




	/// <summary>
	/// 计算堆叠逻辑
	/// </summary>
	#region 堆叠逻辑


	public void CalculateElimination(int x, int y, int step=0)
	{
		if (hasStacked == true) {
			OperationPath.Add(new Vector2Int(x,y));
			LogManager.Instance.Log(string.Format("正在动画，加入数组,X:{0},y:{1}",x,y));
			return;
		}
		if (GoundBackItemArray2D != null)
		{
			FilterLinked.Clear();
			List<Vector2Int> GoundBackItemList = UnitSDF.FindConnectedPieces(new Vector2Int(x,y));
			LogManager.Instance.Log(string.Format("寻找点，是否可连线, 数组长度为：{1}", GoundBackItemList==null, GoundBackItemList == null?"0": GoundBackItemList.Count));
			if ((GoundBackItemList == null|| GoundBackItemList.Count == 1))
			{
				ChainCall(step + 1);//执行连锁数据部分逻辑
				return;
			}
			StartPos.Clear();
			FilterLinked = UnitSDF.FilterLinkedCoordinates(new Vector2Int(x,y), GoundBackItemList);
			if (FilterLinked==null|| FilterLinked.Count==0) {
				LogManager.Instance.Log("无法连线");
				return;
			}
			void StartNextAnimation(int index)
			{
				
				var linkedItem = GoundBackItemArray2D[FilterLinked[index].StarVector2.x, FilterLinked[index].StarVector2.y];
				var ops = linkedItem.RemoveSurfaces();
				linkedItem.DisplayNumbers(false);
				OperationPath.Add(FilterLinked[index].StarVector2);
				//LogManager.Instance.Log("添加了坐标数据：x:" + FilterLinked[index].StarVector2.x + " Y:" + FilterLinked[index].StarVector2.y);
				GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].AddSurfaces(ops, () =>
				{
					if (linkedItem)
					{
						linkedItem.DisplayNumbers(true);
					}
					else
					{
						LogManager.Instance.Log("linkedItem is Null");
					}
					
					if ((index + 1) >= FilterLinked.Count)
					{
						GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].RemoveTopColorObject(() =>
						{
							hasStacked = false;//单次结束
							ChainCall(step+1);
						});
						return;
					}
					else
					{
						StartNextAnimation(index + 1);
					}
				});

			}
			hasStacked = true;
			//IsTouchInput = false;
			StartNextAnimation(0);
		}
	}




	void ChainCall(int step)
	{
		LogManager.Instance.Log(string.Format("开始连锁数据,第{0}步", step));
		for (int i = OperationPath.Count - 1; i >= 0; i--)
		{
			var po = OperationPath[i];
			OperationPath.Remove(po);
			if (GoundBackItemArray2D[po.x, po.y].GetComponent<GoundBackItem>().IsSurface())
			{
				LogManager.Instance.Log(string.Format("连锁数据-点X:{0},y:{1})", po.x, po.y));
				CalculateElimination(po.x, po.y, step++);
				break;
			}
		}

		if (OperationPath==null||OperationPath.Count==0) {
			LogManager.Instance.Log("____没有可以连锁得，判断是否结束游戏______");
			TYQEventCenter.Instance.Broadcast(OnEventKey.OnStackingCompleted);//判断是否胜利
		}
	}

	#endregion


	public void SetUserProp(int propId) {
		IsProp = true;
		IsPropAppUserID = propId;
	}

	public void CloneUserProp()
	{
		IsProp = false;
		IsPropAppUserID = 0;
		if (PropTranform_1) {
			PropTranform_1.GetComponent<GoundBackItem>().DisplayNumbers(true);
			PropTranform_1 = null;
		}
	}


	/// <summary>
	/// 使用道具
	/// </summary>
	/// <param name="propId"></param>
	public void UserProp(int propId)
	{
		switch (propId)
		{
			case 1:
				LevelData.Item_1Number--;
				SelectedObject.GetComponent<GoundBackItem>().RemoveObject();
				CalculateElimination(hit.transform.GetComponent<GoundBackItem>().ItemPosition.x, hit.transform.GetComponent<GoundBackItem>().ItemPosition.y);
				SelectedObject = null;				
				break;
			case 2:
				LevelData.Item_2Number--;
				CalculateElimination(hit.transform.GetComponent<GoundBackItem>().ItemPosition.x, hit.transform.GetComponent<GoundBackItem>().ItemPosition.y);
				SelectedObject = null;
				PropTranform_1 = null;
				break;
			default:
				break;
		}
		UIManager.Instance.GetPanel("GamePanel").GetComponent<GamePanel>().SetUIAction(true, "");
		IsProp = false;
		IsPropAppUserID = 0;
		TYQEventCenter.Instance.Broadcast(OnEventKey.OnApplyProp);
	}


	/// <summary>
	/// 获取当前坐标顶部的颜色及其整个盘的颜色做坐标集合然后筛选
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetGoundBackItems(int x, int y)
	{
		List<Vector2Int> coordinates = GetSpecifyColorList(x,y);
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		List<Vector2Int> resultCoordinates = new List<Vector2Int>();
		LogManager.Instance.Log("开始筛选顶部相同颜色的坐标");
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
					}
				}
			}
		}
		return resultCoordinates;
	}

	/// <summary>
	/// 获取棋盘上与指定点相同颜色的所有棋子坐标
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetSpecifyColorList(int x,int y) {
		var topColor = GetGoundBackItem(x, y).GetTopColor();
		List<Vector2Int> coordinates = new List<Vector2Int>();
		//获取整个盘上顶部颜色相同的加入list
		for (int i = 0; i < GoundBackItemArray2D.GetLength(0); i++)
		{
			for (int j = 0; j < GoundBackItemArray2D.GetLength(1); j++)
			{
				if (GoundBackItemArray2D[i, j] != null && GoundBackItemArray2D[i, j].IsSurface() && (GoundBackItemArray2D[i, j].GetTopColor() == topColor || GoundBackItemArray2D[i, j].GetTopColor() == ItemColorType.StarAll))
				{
					coordinates.Add(new Vector2Int(i, j));
				}
			}
		}
		//如果不包含操作的点则作废
		if (!coordinates.Contains(new Vector2Int(x, y)))
		{
			return null;
		}
		return coordinates;
	}




	/// <summary>
	/// 将给定的数据按照指定的规则排序
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public List<Vector2Int> SortAndFilter(List<Vector2Int> coordinates)
	{
		if (coordinates == null || coordinates.Count == 0) return null;
		List<Vector2Int> sortedList = new List<Vector2Int>();
		HashSet<Vector2Int> remainingCoords = new HashSet<Vector2Int>(coordinates);
		Stack<(Vector2Int current, List<Vector2Int> path)> stack = new Stack<(Vector2Int, List<Vector2Int>)>();
		Vector2Int start = coordinates[0];
		for (int i = 0; i < coordinates.Count; i++)
		{
			if (GetAroundPos(coordinates[i].x, coordinates[i].y).Count == 1)
			{
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
	public bool CheckAndAddIfMatch(Vector2Int vex1, Vector2Int vex2)
	{
		if (GoundBackItemArray2D[vex1.x, vex1.y].IsSurface() && GetGoundBackItem(vex2.x, vex2.y).IsSurface() &&
			(GoundBackItemArray2D[vex1.x, vex1.y].GetTopColor() == GetGoundBackItem(vex2.x, vex2.y).GetTopColor()))
		{
			return true;
		}
		return false;
	}



	/// <summary>
	/// 获取指定坐标的周围的点,存在棋盘上的点
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetAroundPos(int x, int y)
	{
		return UnitSDF.GetCreatorPos(x, y).Where(v => v.x >= 0 && v.y >= 0 && x < GoundBackItemArray2D.GetLength(0)
		&& y < GoundBackItemArray2D.GetLength(1)&& GoundBackItemArray2D[v.x,v.y]!=null
		).ToList();
	}

	/// <summary>
	/// 获取指定点周围相同颜色且可计算的坐标
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetAroundCanBeOperatedPos(int x, int y)
	{
		if (GetGoundBackItem(x, y).IsCanBeOperated())
		{
			ItemColorType topColor = GetGoundBackItem(x, y).GetTopColor();
			return GetAroundPos(x, y).Where(pos => !GetGoundBackItem(pos.x, pos.y).IsRunAnim && GetGoundBackItem(pos.x, pos.y).IsCanBeOperated() && GetGoundBackItem(pos.x, pos.y).GetTopColor() == topColor).ToList();
		}
		else { return new List<Vector2Int>(); }
	}

	/// <summary>
	/// 获取当前指定的组
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public GoundBackItem GetGoundBackItem(int x, int y)
	{
		if (x >= 0 && y >= 0 && x < GoundBackItemArray2D.GetLength(0) && y < GoundBackItemArray2D.GetLength(1)&& GoundBackItemArray2D[x,y]!=null)
		{
			var item = GoundBackItemArray2D[x, y];
			return item;
		}
		else
		{
			return null;
		}
	}

	#region Old Scrpit


	/// <summary>
	/// 新建一个指定大小的空的地图网格
	/// </summary>
	/// <param name="width"></param>
	/// <param name="height"></param>
	//public void GenerateBoxMatrix(int width, int height)
	//{
	//	RemoveBoxMatrix();
	//	GoundBackItemArray2D = new GoundBackItem[width, height];
	//	bool isOn = true;
	//	for (int x = 0; x < width; x++)
	//	{
	//		for (int z = 0; z < height; z++)
	//		{
	//			Vector3 position = new Vector3(
	//				StartPosition.x + (x == 0 ? 0 : x * BlockSize.x) + BlockSizeDeviation.x, 0,
	//				StartPosition.z + (isOn ? z * BlockSize.z + BlockSizeDeviation.z : z * BlockSize.z));
	//			GameObject block = PoolManager.Instance.CreateGameObject("bottoms", ItemParent);
	//			block.transform.localRotation = Quaternion.Euler(0, 0, 0);
	//			block.transform.localPosition = position;
	//			block.transform.localPosition += ItemParent.transform.position;
	//			block.transform.SetParent(ItemParent.transform);
	//			GoundBackItem goundBackItem = block.AddComponent<GoundBackItem>();
	//			goundBackItem.SetData(x, z, $"{x},{z}");
	//			if (GetNowLevelData().IsLock(x, z))
	//			{
	//				Debug.Log(string.Format("设置lock，x：{0}，y：{1}", x, z));
	//				goundBackItem.LockOrUnLockTheItem(true);
	//			}
	//			GoundBackItemArray2D[x, z] = goundBackItem;
	//		}
	//		isOn = !isOn;
	//	}
	//}
	#endregion

	/// <summary>
	/// 加载level Prefab
	/// </summary>
	private void LoadGenerateBoxMatrix()
	{
		RemoveBoxMatrix();
		GameObject PrefabObj = Resources.Load<GameObject>("LevelPrefab/Lv" + NowLevel.ToString());
		//GameObject bottomtext= Resources.Load<GameObject>("Prefab/bottomText");
		if (PrefabObj == null)
		{
			LogManager.Instance.Log("未能加载到该关卡数据，加载的关卡为："+ NowLevel.ToString());
			return;
		}
		else {
			GameObject levelObj = Instantiate(PrefabObj);
			levelObj.transform.SetParent(ItemParent.transform);
			GoundBackItemArray2D = null;
			GoundBackItemArray2D = new GoundBackItem[levelObj.transform.childCount, levelObj.transform.childCount];
			for (int i = 0; i < levelObj.transform.childCount; i++)
			{
				GoundBackItem goundBackItem = levelObj.transform.GetChild(i).GetComponent<GoundBackItem>();
				if (goundBackItem != null)
				{
					GoundBackItemArray2D[goundBackItem.ItemPosition.x, goundBackItem.ItemPosition.y] = goundBackItem;
					GameObject oth = PoolManager.Instance.GetObject("bottomText", goundBackItem.transform);
					GameObject parentClass = PoolManager.Instance.GetObject("ParentClass", goundBackItem.transform);
					oth.transform.rotation = Quaternion.Euler(90, 0, 0);
					oth.transform.localPosition = Vector3.zero;
					parentClass.transform.localPosition= Vector3.zero;
					//othbottomtext,Vector3.zero, Quaternion.Euler(90, 0, 0), goundBackItem.transform);
					goundBackItem.NumberText = oth;
					goundBackItem.ParentClass = parentClass;
					oth.transform.SetParent(goundBackItem.ParentClass.transform);
					if (goundBackItem.IsLock) {
						GameObject spriteRendererPrefab = Resources.Load<GameObject>("Prefab/Lock");
						GameObject spriteRenderer = Instantiate(spriteRendererPrefab,new Vector3(0,1.4f,1), Quaternion.Euler(90, 0, 0), goundBackItem.transform);
						goundBackItem.SpriteRendener = spriteRenderer;
						spriteRenderer.transform.localPosition = new Vector3(0,1.4f,1);
					}
					goundBackItem.DisplayNumbers(true, goundBackItem.IsLock ? "解锁" : "");
				}
			}
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

	/// <summary>
	/// 还有几个已解锁未放置的空位
	/// </summary>
	/// <returns></returns>
	public int IsItAvailable() {
		int available = 0;
		for (int i = 0; i < ItemParent.transform.GetChild(0).childCount; i++)
		{
			GoundBackItem goundBackItem = ItemParent.transform.GetChild(0).GetChild(i).GetComponent<GoundBackItem>();
			if (!goundBackItem.IsLock&&goundBackItem.SurfacesList.Count==0) {
				available++;
			}
		}
		return available;
	}
}

