using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[Tooltip("���񷽿�Ԥ���壬ͨ����")]
	[Header("����Ԥ����")]
	public GameObject blockPrefab; // Ԥ�Ʒ���
	[Header("��ͼ�ߴ�")]
	public Vector2Int mapSize = new Vector2Int(5, 5);
	[Header("ģ�ʹ�С����ֵ")]
	public Vector3 blockSize = new Vector3(5.22f, 0, 6);//ƫ��ֵ
	[Header("ƫ��ֵ")]
	public Vector3 blockSizeDeviation = new Vector3(0, 0, 3);//ƫ��ֵ
	[Header("����")]
	public GameObject ItemParent;

	private Vector3 startPosition = new Vector3(0, 0, 0);
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
	/// ����ѵ��߼�
	/// </summary>
	public void CalculateElimination(int x, int y) {
		if(GoundBackItemArray2D!=null){
			var GoundBackItemList = GetGoundBackItems(x, y);
			if (GoundBackItemList!=null&&GoundBackItemList.Count > 0)
			{
				int topNumber = 0;
				GoundBackItem top=null;
				//���ȴ���ɫ�ٵģ��ѵ�����ɫ�����һ��
				//�Ƚ��Ķ���ɫ��
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
				Debug.Log("��");
			}
		}
	}

	/// <summary>
	/// ��ȡ��ǰ��ɫ���߿��Զѵ�������
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
	/// ��ȡ��ǰָ������
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
	/// �½�һ��ָ����С�Ŀյĵ�ͼ����
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
	/// �Ƴ���ǰ��ͼ����
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

