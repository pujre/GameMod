using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{


	private float Assign_Y;
	/// <summary>
	/// �ýڵ�����������
	/// </summary>
	public Vector2Int ItemPosition;
    public List<Surface> SurfacesList = new List<Surface>();

	private void Awake()
	{
		SurfacesList = new List<Surface>();
		Assign_Y = 1f;
	}


	public GoundBackItem(int x, int y, string name) {
		SetData(x,y,name);
	}

	public void SetData(int x, int y, string name)
	{
		ItemPosition = new Vector2Int(x, y);
		if (gameObject) gameObject.name = name;
	}

	public bool IsAddSurface() {
		return SurfacesList.Count == 0?true:false;
	}
	/// <summary>
	/// ���²���λ��
	/// </summary>
	public void SetChinderPosition() {
		if (SurfacesList == null) return;
        for (int i = 0; i < SurfacesList.Count; i++)
        {
            SurfacesList[i].transform.position = new Vector3(0,transform.position.y+ i * Assign_Y, 0);
		}
	}

	public void AddSurfacesList(List<Surface> surfacess)
	{
		for (int i = 0; i < surfacess.Count; i++)
		{
			SurfacesList.Add(surfacess[i]);
			surfacess[i].transform.SetParent(transform);
		}
		SetChinderPosition();
	}

	/// <summary>
	/// ��ȡ��ǰ�ӽڵ�����ɫ�������
	/// </summary>
	/// <returns></returns>
	public int GetNowColorNumber() {
		List<string> ColorType = new List<string>();
        for (int i = 0; i < SurfacesList.Count; i++)
        {
            string colorTypeName = SurfacesList[i].GetColorType().ToString();
            if (!ColorType.Contains(colorTypeName)) {
                ColorType.Add(colorTypeName);
            }
        }
		Debug.Log(string.Format("����Ϊ{0}��{1}��ɫ��Ϊ��{2}", ItemPosition.x, ItemPosition.y, ColorType.Count));
        return ColorType.Count;
    }

	/// <summary>
	/// �ӵ�ǰ��Surface�����0��ʼ�Ƴ�ָ����ɫ��surface���������Ƴ���surface����
	/// </summary>
	/// <param name="itemColor"></param>
	/// <returns></returns>
	public List<Surface> RemoveSurfaces(ItemColorType itemColor) {
		List<Surface> surfaces= new List<Surface>();
        int x = 0;
        for (int i = SurfacesList.Count-1; i >0 ; i--)
        {
            if (SurfacesList[i].GetColorType() == itemColor)
            {
                x++;
                surfaces.Add(SurfacesList[i]);
			}
            else {
                break;
            }
        }
		for (int j = 0; j < surfaces.Count; j++)
		{
			SurfacesList.Remove(surfaces[j]);
		}
		Debug.Log("�����itemΪ��"+ ItemPosition.x+" "+ItemPosition.y+" "+ surfaces.Count);
        return surfaces;
	}


	public void AddSurfaces(List<Surface> listsurface, MoveTweenType moveTweenType, Action action = null)
	{
		Debug.Log("__�ѵ�__" + listsurface.Count);
		if (moveTweenType == MoveTweenType.One)
		{
			int x = 0;
			for (int i = 0; i < listsurface.Count; i++)
			{
				Vector3 targetPosition = new Vector3(transform.position.x, (SurfacesList.Count + i * Assign_Y) * Assign_Y, transform.position.z);
				listsurface[i].transform.SetParent(transform);
				listsurface[i].transform.DOMove(targetPosition,0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
				{
					SurfacesList.Add(listsurface[i]);
					if (x == listsurface.Count - 1)
					{
						SetChinderPosition();
						Debug.Log("�ƶ���ɣ���ʼ��һ�ֵ��ж�");
						action?.Invoke();
					}
				});
				x++;
			}
		}
		else if (moveTweenType == MoveTweenType.Continuity)
		{
			Surface surfacePic = listsurface[listsurface.Count-1];
			Vector3 targetPosition = new Vector3(transform.position.x, (SurfacesList.Count + Assign_Y) * Assign_Y, transform.position.z);
			surfacePic.transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InOutQuad).OnComplete(() =>
			{
				surfacePic.transform.SetParent(transform);
				listsurface.Remove(surfacePic);
				SetChinderPosition();
				if (listsurface.Count <= 0)
				{
					Debug.Log("�ƶ�һ����ɣ���ʼ��һ�ֵ��ж�");
					action?.Invoke();
				}
			});
		}

	}


	/// <summary>
	/// ��ȡ��ǰ���˵�surface��ɫ
	/// </summary>
	/// <returns></returns>
	public ItemColorType GetTopColor(){
		return SurfacesList[SurfacesList.Count-1].GetColorType();

	}

	/// <summary>
	/// ��
	/// </summary>
	/// <param name="surfacesList"></param>
	/// <param name="isOn"></param>
	public void EntrIntoWarehouse(List<Surface> surfacesList) {
        for (int i = 0; i < surfacesList.Count; i++)
        {
            
        }
    }
}
