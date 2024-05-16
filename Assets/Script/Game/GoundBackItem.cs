using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
	public float delayBetweenMoves = 0.5f;  // ÿ�������ƶ�֮����ӳ�

	private float Assign_Y;
	/// <summary>
	/// �ýڵ�����������
	/// </summary>
	public Vector2Int ItemPosition;
    public List<Surface> SurfacesList = new List<Surface>();

	private void Awake()
	{
		SurfacesList = new List<Surface>();
		Assign_Y = 1.2f;
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
			SurfacesList[i].transform.localPosition = new Vector3(0,transform.localPosition.y+ Assign_Y+( i * Assign_Y), 0);
		}
	}

	public List<Vector3> GetEndListVector3(int x) { 
		List<Vector3> vectors = new List<Vector3>();
		Vector3 startVectpr3 = SurfacesList[SurfacesList.Count-1].transform.localPosition;
		for (int i = 0; i < x; i++)
		{
			vectors.Add(new Vector3(startVectpr3.x, startVectpr3.y + Assign_Y + (i * Assign_Y), startVectpr3.z));
		}
		return vectors;
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
        for (int i = SurfacesList.Count-1; i >=0 ; i--)
        {
            if (SurfacesList[i].GetColorType() == itemColor)
            {
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
				Vector3 targetPosition = new Vector3(transform.localPosition.x, (SurfacesList.Count * (1+ Assign_Y))+i * Assign_Y, transform.localPosition.z);
				listsurface[i].transform.SetParent(transform);
				listsurface[i].transform.DOMove(targetPosition, 1).SetEase(Ease.InOutQuad).OnComplete(() =>
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
			Sequence sequence = DOTween.Sequence();  // ����һ��DoTween����
			List<Vector3> o3s = GetEndListVector3(listsurface.Count);

			for (int i = 0; i < listsurface.Count; i++)
			{
				var obj= listsurface[i];

				obj.transform.SetParent(transform);
				Vector3 ka = new Vector3((o3s[i].x + obj.transform.localPosition.x) / 2, o3s[i].y + 4, (o3s[i].z + obj.transform.localPosition.z) / 2);

				// Ϊÿ���������һ���ƶ����յ�Ķ���������ǰһ������������ʼ
				sequence.Append(obj.transform.DOLocalMove(ka, 0.25f).SetEase(Ease.Linear));
				sequence.Append(obj.transform.DOLocalMove(o3s[i], 0.25f).SetEase(Ease.Linear));
				sequence.AppendInterval(delayBetweenMoves);  // ��ÿ�������ƶ�������ӳ�
				SurfacesList.Add(obj);
			}

			sequence.OnComplete(() => {
				SetChinderPosition();
				action?.Invoke();
			});
			sequence.Play();  // ��������
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
