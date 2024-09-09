using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
	private Vector3 BlockSize = new Vector3(5.22f, 0, 6);//ƫ��ֵ
	private Vector3 BlockSizeDeviation = new Vector3(0, 0, 3);//ƫ��ֵ
	public GameObject ItemParent;
	public Material[] DefaultORHightMaterial;
	private Vector3 StartPosition = new Vector3(0, 0, 0);
	public List<Vector2Int> OperationPath = new List<Vector2Int>();

	private Camera Cam;
	public bool IsTouchInput = true;
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
					if (!IsProp&&hit.transform.GetComponent<SurfaceItem>() && hit.transform.GetComponent<SurfaceItem>().IsOnMove )
					{
						SelectedObject = hit.transform.gameObject;
						IsDragging = true;
					} else if (!IsProp&&hit.transform.GetComponent<GoundBackItem>() && hit.transform.GetComponent<GoundBackItem>().IsLock) {
						ADManager.Instance.ShowAD(ADType.Video, (isOn)=>{
							if (isOn)
							{
								hit.transform.GetComponent<GoundBackItem>().IsLock = false;
								hit.transform.GetComponent<GoundBackItem>().DisplayNumbers(true, "");
							}
							else { 

							}
						});
					}
					else
					if (IsProp && hit.transform.GetComponent<GoundBackItem>() && hit.transform.GetComponent<GoundBackItem>().IsSurface()) {
						switch (IsPropAppUserID)
						{
							case 1:
								SelectedObject = hit.transform.gameObject;
								UserProp(1);
								break;
							case 2:
								SelectedObject = hit.transform.gameObject;
								UserProp(2);
								break;
							case 3:
								if (PropTranform_1 == null) {
									PropTranform_1 = hit.transform.gameObject;
									PropTranform_1.GetComponent<GoundBackItem>().DisplayNumbers(true, "��");
								} else if (PropTranform_1 != null && PropTranform_1 != hit.transform.gameObject) {
									hit.transform.GetComponent<GoundBackItem>().PropPositionChange(PropTranform_1.GetComponent<GoundBackItem>());
									SelectedObject = PropTranform_1;
									UserProp(3);
								}
								break;
						}
					} else
					if (hit.transform.GetComponent<GoundBackItem>() && hit.transform.GetComponent<GoundBackItem>().IsLock) {

					}
				}
			}

			if (IsDragging && SelectedObject != null)
			{
				if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
				{
					newPosition = hitInfo.point;
					newPosition.y = 3; //SelectedObject.transform.position.y; // ����Y�᲻��
					SelectedObject.transform.position = newPosition;
				}

				// ʹ�� RaycastAll ���������ײ
				RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
				foundBottom = false;

				foreach (RaycastHit hit in hits)
				{
					if (hit.transform.gameObject.tag == "bottom" &&!IsProp&&
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
					
				}
				// ���뿪���������ʱ���ж�
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
	}

	private void SnapToGrid(GameObject obj)
	{
		if (obj != null)
		{
			if (lastHighlightedObject != null)
			{
				lastHighlightedObject.GetComponent<MeshRenderer>().material = DefaultORHightMaterial[0];
				GoundBackItem lj = lastHighlightedObject.GetComponent<GoundBackItem>();
				lj.SetvolumetricLine(false);
				obj.transform.position = lastHighlightedObject.transform.position + new Vector3(0, 1.2f, 0);
				SurfaceItem si = obj.transform.GetComponent<SurfaceItem>();
				si.QueMoveEnd();
				lj.AddSurfacesList(si.Surfaces);
				AudioManager.Instance.PlaySFX("Put�����¶ѵ��");
				si.Surfaces.Clear();
				lj.DisplayNumbers(true);
				//OperationPath.Clear();
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
		LoadLevel(1);
	}

	public void LoadNextLevel()
	{
		LoadLevel(NowLevel + 1);
	}

	public void LoadLevel(int level)
	{
		NowLevel = level;
		LevelData levedata = LevelDataRoot.GetLevelData(level);
		//GenerateBoxMatrix(levedata.ChapterSize.x, levedata.ChapterSize.y);
		LoadGenerateBoxMatrix();
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString());
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnLoadGameLevel.ToString());
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnGameStar.ToString());
	}


	public void ScelfJob()
	{
		GameObject obj = PoolManager.Instance.CreateGameObject("surfaceItem", GameObject.Find("Game/Panel"));
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localPosition = new Vector3(80, 1, -18);
		GamePanel gamePanel = UIManager.Instance.GetPanel("GamePanel") as GamePanel;
		for (int i = 0; i < gamePanel.SelectedList.Count; i++)
		{
			if (gamePanel.SelectedList[i].SelfGameMove == null)
			{
				gamePanel.SelectedList[i].SelfGameMove = obj;
				obj.GetComponent<SurfaceItem>().CreatorSurface(GetNowLevelData().ColourNum);
				obj.GetComponent<SurfaceItem>().QurStart(gamePanel.SelectedList[i].Pos);
				AudioManager.Instance.PlaySFX("Hu�����������µĶѵ��");
				break;
			}
		}

	}


	/// <summary>
	/// ����ѵ��߼�
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
				var linkedItem = GoundBackItemArray2D[FilterLinked[index].StarVector2.x, FilterLinked[index].StarVector2.y];
				var ops = linkedItem.RemoveSurfaces();
				linkedItem.DisplayNumbers(false);
				OperationPath.Add(FilterLinked[index].StarVector2);
				//Debug.Log("������������ݣ�x:" + FilterLinked[index].StarVector2.x + " Y:" + FilterLinked[index].StarVector2.y);
				GoundBackItemArray2D[FilterLinked[index].EndVector2.x, FilterLinked[index].EndVector2.y].AddSurfaces(ops, () =>
				{
					linkedItem.DisplayNumbers(true);
					if ((index + 1) >= FilterLinked.Count)
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
			IsTouchInput = false;
			StartNextAnimation(0);
		}
	}


	void ChainCall()
	{
		//Debug.Log("��ʼ��������");
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
				CalculateElimination(po.x, po.y);
				break;
			}
		}
		if (OperationPath.Count == 0)
		{
			IsTouchInput = true;
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
				SelectedObject.GetComponent<GoundBackItem>().TopTranslateColor(1);
				CalculateElimination(hit.transform.GetComponent<GoundBackItem>().ItemPosition.x, hit.transform.GetComponent<GoundBackItem>().ItemPosition.y);
				SelectedObject = null;
				//Debug.Log("������Ǹ��������");
				break;
			case 3:
				LevelData.Item_3Number--;
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
		DelegateManager.Instance.TriggerEvent(OnEventKey.OnApplyProp.ToString(),"");
	}


	/// <summary>
	/// ��ȡ��ǰ���궥������ɫ���������̵���ɫ�����꼯��Ȼ��ɸѡ
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
				if (GoundBackItemArray2D[i, j] != null&& GoundBackItemArray2D[i, j].IsSurface() && (GoundBackItemArray2D[i, j].GetTopColor() == topColor|| GoundBackItemArray2D[i, j].GetTopColor()== ItemColorType.StarAll))
				{
					coordinates.Add(new Vector2Int(i, j));
				}
			}
		}
		if (!coordinates.Contains(new Vector2Int(x, y)))
		{
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
						log += position + "��";
					}
				}
			}
		}
		//Debug.Log("������ͬ��ɫ������Ϊ��" + log);
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
			Debug.Log("������Ҫ�������򡪡�");
			FilterLinked = UnitSDF.FilterLinkedCoordinates(coordinates);
			Debug.Log("FilterLinked�ĳ���Ϊ��" + FilterLinked.Count);
		}
		else
		{
			FilterLinked.Clear();
			for (int i = 0; i < coordinates.Count - 1; i++)
			{
				//Debug.Log(string.Format("��{0}�ƶ�����{1}", coordinates[i], coordinates[i + 1]));
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
	/// ��ȡָ���������Χ�ĵ�
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

	#endregion
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

	/// <summary>
	/// ����level Prefab
	/// </summary>
	private void LoadGenerateBoxMatrix()
	{
		RemoveBoxMatrix();
		GameObject PrefabObj = Resources.Load<GameObject>("LevelPrefab/Lv" + NowLevel.ToString());
		GameObject bottomtext= Resources.Load<GameObject>("Prefab/bottomText");
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
					GameObject oth= Instantiate(bottomtext,Vector3.zero, Quaternion.Euler(90, 0, 0), goundBackItem.transform);
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
}

