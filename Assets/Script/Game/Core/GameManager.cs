using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TYQ;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GameManager : SingletonMono<GameManager>
{
	public bool DontSimcity = false;
	public List<Selected> SelectedList = new List<Selected>();
	public List<Staging> StagingList = new List<Staging>();
	public GameObject ItemParent;
	public Material[] DefaultORHightMaterial;
	public List<Vector2Int> OperationPath = new List<Vector2Int>();

	private Camera Cam;
	public bool IsTouchInput = false;
	private bool IsDragging = false;
	private GameObject SelectedObject;


	public GoundBackItem[,] GoundBackItemArray2D;
	public LevelDataRoot LevelDataRoot;
	public List<InstructionData> FilterLinked = new List<InstructionData>();
	public int NowLevel = 1;
	private LevelData LevelData;

	private bool IsProp=false;
	private int IsPropAppUserID;
	private GameObject PropTranform_1;

	#region �ֲ�����private
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
		Cam = Camera.main; // ��ȡ�������
		ResPath.Init();
	}

	void Start()
	{
		LoadlevelData();
		AudioManager.Instance.PlayBGM("bgm2����Ϸ���棩");
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
				// ʹ��Vector3.Lerp��Vector3.MoveTowards��ƽ���ƶ�
				if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
				{
					initialPosition = hitInfo.point;
					initialPosition.y = 3; //SelectedObject.transform.position.y; // ����Y�᲻��
				}
				SelectedObject.transform.position = Vector3.Lerp(SelectedObject.transform.position, initialPosition, Time.deltaTime * 15); // yourLerpSpeed������Ҫ����
				// ʹ�� RaycastAll ���������ײ
				RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
				foundBottom = false;

				foreach (RaycastHit hit in hits)
				{
					if (!IsProp && hit.transform.gameObject.tag == "bottom" &&
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
						break; // �ҵ�Ŀ�������˳�ѭ��
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
						break; // �ҵ�Ŀ�������˳�ѭ��
					}

				}
				// ���뿪���������ʱ���ж�
				if (!foundBottom)
				{
					if (lastHighlightedObject != null)
					{
						lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
						if(lastHighlightedObject.GetComponent<GoundBackItem>()) lastHighlightedObject.GetComponent<GoundBackItem>().SetvolumetricLine(false);
						lastHighlightedObject = null;
					}
				}
				
			}

			if (Input.GetMouseButtonUp(0) && IsDragging)
			{
				IsDragging = false;
				SnapToGrid(SelectedObject, ((lastHighlightedObject!=null&&lastHighlightedObject.name == "Staging")|| lastHighlightedObject==null) ?false:true);
			}
		}
	}


	/// <summary>
	/// ����߼�
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
					//SelectedObject = obj.transform.gameObject;
					//UserProp(2);
					Debug.Log("PropTranform_1: " + (PropTranform_1==null? "  n ": PropTranform_1.gameObject.name));
					if (PropTranform_1 == null)
					{
						PropTranform_1 = obj.transform.gameObject;
						PropTranform_1.GetComponent<GoundBackItem>().DisplayNumbers(true, "��");
					}
					else if (PropTranform_1 != null && PropTranform_1 != obj.transform.gameObject)
					{
						obj.transform.GetComponent<GoundBackItem>().PropPositionChange(PropTranform_1.GetComponent<GoundBackItem>());
						SelectedObject = PropTranform_1;
						UserProp(2);
					}
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
						AudioManager.Instance.PlaySFX("Put�����¶ѵ��");
						si.Surfaces.Clear();
						lj.DisplayNumbers(true);
						//OperationPath.Clear();
						if (!DontSimcity) {
							CalculateElimination(lj.ItemPosition.x, lj.ItemPosition.y, 0);
							Debug.Log(string.Format("�����˶ѵ��ѣ�X:{0},Y{1}", lj.ItemPosition.x, lj.ItemPosition.y));

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
	/// �ճ�ָ�������Ŀ�λ
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

	public void LoadLevel(int level)
	{
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
					AudioManager.Instance.PlaySFX("Hu�����������µĶѵ��");
					break;
				}
			}
		}
		
	}




	/// <summary>
	/// ����ѵ��߼�
	/// </summary>
	#region �ѵ��߼�


	public void CalculateElimination(int x, int y, int step=0)
	{
		if (GoundBackItemArray2D != null)
		{
			FilterLinked.Clear();
			//List<Vector2Int> GoundBackItemList = GetGoundBackItems(x, y);
			List<Vector2Int> GoundBackItemList = UnitSDF.FindConnectedPieces(new Vector2Int(x,y));
			if (GoundBackItemList == null || GoundBackItemList.Count == 0)
			{
				//ChainCall(step++);
				Debug.Log(string.Format("����-��ǰ��X:{0},Y:{1}���������޿����ߵĵ�", x,y));
				return;
			}
			ProcessCoordinates(GoundBackItemList);
			if (FilterLinked == null || FilterLinked.Count == 0)
			{
				//ChainCall(step++);
				Debug.Log(string.Format("����-��ǰ��X:{0},Y:{1}��Ҫ��ɵ����������Ѿ����", x, y));
				return;
			}
			void StartNextAnimation(int index)
			{
				var linkedItem = GoundBackItemArray2D[FilterLinked[index].StarVector2.x, FilterLinked[index].StarVector2.y];
				var ops = linkedItem.RemoveSurfaces();
				linkedItem.DisplayNumbers(false);
				OperationPath.Add(FilterLinked[index].StarVector2);
				hasStacked = true;
				Debug.Log("������������ݣ�x:" + FilterLinked[index].StarVector2.x + " Y:" + FilterLinked[index].StarVector2.y);
				GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].AddSurfaces(ops, () =>
				{
					if (linkedItem)
					{
						linkedItem.DisplayNumbers(true);
					}
					else
					{
						Debug.Log("linkedItem is Null");
					}
					
					if ((index + 1) >= FilterLinked.Count)
					{
						GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].RemoveTopColorObject(() =>
						{
							//ChainCall(step++);
							Debug.Log(string.Format("������ɺ�����-��ǰ��X:{0},Y:{1}��Ҫ��ɵ����������Ѿ����", FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y));

						});
						return;
					}
					else
					{
						StartNextAnimation(index + 1);
					}
				});

			}
			IsTouchInput = false;
			StartNextAnimation(0);
		}
	}


	void ChainCall(int step)
	{
		Debug.Log(string.Format("��ʼ��������,��{0}��", step));
		for (int i = OperationPath.Count - 1; i >= 0; i--)
		{
			var po = OperationPath[i];
			//Debug.Log(OperationPath.Count + "��ʼ��������----x:" + po.x + "y:" + po.y);
			OperationPath.Remove(po);
			if (GoundBackItemArray2D[po.x, po.y].GetComponent<GoundBackItem>().IsSurface())
			{
				//if (OperationPath.Count > 0)
				//{
				//	Debug.Log(OperationPath.Count + "���µ������λ��Ϊ X:" + OperationPath[OperationPath.Count - 1].x + "Y:" + OperationPath[OperationPath.Count - 1].y);
				//}
				CalculateElimination(po.x, po.y, step++);
				break;
			}
		}
		if (OperationPath.Count == 0)
		{
			IsTouchInput = true;
			if (hasStacked) {
				TYQEventCenter.Instance.Broadcast(OnEventKey.OnStackingCompleted);
				hasStacked = false;
			}
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
	/// ʹ�õ���
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
				PropTranform_1.GetComponent<GoundBackItem>().DisplayNumbers(true);
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
	/// ��ȡ��ǰ���궥������ɫ���������̵���ɫ�����꼯��Ȼ��ɸѡ
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetGoundBackItems(int x, int y)
	{
		List<Vector2Int> coordinates = GetSpecifyColorList(x,y);
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		List<Vector2Int> resultCoordinates = new List<Vector2Int>();
		Debug.Log("��ʼɸѡ������ͬ��ɫ������");
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
	/// ��ȡ��������ָ������ͬ��ɫ��������������
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetSpecifyColorList(int x,int y) {
		var topColor = GetGoundBackItem(x, y).GetTopColor();
		List<Vector2Int> coordinates = new List<Vector2Int>();
		//��ȡ�������϶�����ɫ��ͬ�ļ���list
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
		//��������������ĵ�������
		if (!coordinates.Contains(new Vector2Int(x, y)))
		{
			return null;
		}
		return coordinates;
	}

	/// <summary>
	/// ��������
	/// </summary>
	/// <param name="coordinates"></param>
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
			Debug.Log("������Ҫ�������򡪡�");
			FilterLinked = UnitSDF.FilterLinkedCoordinates(coordinates);
			Debug.Log("FilterLinked�ĳ���Ϊ��" + FilterLinked.Count);
		}
		else
		{
			FilterLinked.Clear();
			for (int i = 0; i < coordinates.Count - 1; i++)
			{
				Debug.Log(string.Format("��{0}�ƶ�����{1}", coordinates[i], coordinates[i + 1]));
				FilterLinked.Add(new InstructionData(coordinates[i], coordinates[i + 1]));
			}
		}

	}


	/// <summary>
	/// �����������ݰ���ָ���Ĺ�������
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
			var around = GetAroundPos(current.x, current.y);// ��ȡ��ǰ������Χ������
			var nextSteps = around.Where(remainingCoords.Contains).ToList();// ɸѡ�������ʵ���Χ����
			if (nextSteps.Count == 0 && path.Count < coordinates.Count)
			{
				continue;
			}
			foreach (var next in nextSteps)// �������п��е���һ��
			{
				List<Vector2Int> newPath = new List<Vector2Int>(path) { next };// ����������һ������·��
				stack.Push((next, newPath));// ����·��������ѹ��ջ
				remainingCoords.Remove(next); // �Ӵ����ʼ������Ƴ���ǰ����
			}
			if (nextSteps.Count == 0 && path.Count == coordinates.Count)
			{
				return path;
			}
		}
		return null;
	}







	// ����ض����Ƿ���������
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
	/// ��ȡָ���������Χ�ĵ�,���������ϵĵ�
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
	/// ��ȡָ������Χ��ͬ��ɫ�ҿɼ��������
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public List<Vector2Int> GetAroundCanBeOperatedPos(int x, int y) {
		ItemColorType topColor = GetGoundBackItem(x, y).GetTopColor();
		return GetAroundPos(x, y).Where(pos =>!GetGoundBackItem(pos.x, pos.y).IsRunAnim && GetGoundBackItem(pos.x, pos.y).IsCanBeOperated()&&GetGoundBackItem(pos.x, pos.y).GetTopColor() == topColor ).ToList();
	}

	/// <summary>
	/// ��ȡ��ǰָ������
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
	/// �½�һ��ָ����С�Ŀյĵ�ͼ����
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
	//				Debug.Log(string.Format("����lock��x��{0}��y��{1}", x, z));
	//				goundBackItem.LockOrUnLockTheItem(true);
	//			}
	//			GoundBackItemArray2D[x, z] = goundBackItem;
	//		}
	//		isOn = !isOn;
	//	}
	//}
	#endregion

	/// <summary>
	/// ����level Prefab
	/// </summary>
	private void LoadGenerateBoxMatrix()
	{
		RemoveBoxMatrix();
		GameObject PrefabObj = Resources.Load<GameObject>("LevelPrefab/Lv" + NowLevel.ToString());
		//GameObject bottomtext= Resources.Load<GameObject>("Prefab/bottomText");
		if (PrefabObj == null)
		{
			Debug.Log("δ�ܼ��ص��ùؿ����ݣ����صĹؿ�Ϊ��"+ NowLevel.ToString());
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
					oth.transform.rotation = Quaternion.Euler(90, 0, 0);
					oth.transform.position = Vector3.zero;
					//othbottomtext,Vector3.zero, Quaternion.Euler(90, 0, 0), goundBackItem.transform);
					goundBackItem.NumberText = oth;
					if (goundBackItem.IsLock) {
						GameObject spriteRendererPrefab = Resources.Load<GameObject>("Prefab/Lock");
						GameObject spriteRenderer = Instantiate(spriteRendererPrefab,new Vector3(0,1.4f,1), Quaternion.Euler(90, 0, 0), goundBackItem.transform);
						goundBackItem.SpriteRendener = spriteRenderer;
						spriteRenderer.transform.localPosition = new Vector3(0,1.4f,1);
					}
					goundBackItem.DisplayNumbers(true, goundBackItem.IsLock ? "����" : "");
				}
			}
		}
	}

	/// <summary>
	/// �Ƴ���ǰ��ͼ����
	/// </summary>
	public void RemoveBoxMatrix()
	{
		for (int i = ItemParent.transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(ItemParent.transform.GetChild(i).gameObject);
		}
	}

	/// <summary>
	/// ���м����ѽ���δ���õĿ�λ
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

