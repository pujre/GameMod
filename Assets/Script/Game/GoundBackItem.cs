using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
	private float Assign_Y=1.3f;
	/// <summary>
	/// 该节点所属得坐标
	/// </summary>
	public Vector2Int ItemPosition;
    public List<Surface> SurfacesList = new List<Surface>();


    public GoundBackItem(int x, int y, string name) {
		SetData(x,y,name);
	}

	public void SetData(int x, int y, string name)
	{
		ItemPosition = new Vector2Int(x, y);
		if (gameObject) gameObject.name = name;
	}

	/// <summary>
	/// 从新布局位置
	/// </summary>
	public void SetChinderPosition() {
        for (int i = 0; i < SurfacesList.Count; i++)
        {
            SurfacesList[i].transform.position = new Vector3(transform.position.x, (SurfacesList.Count-i) * Assign_Y, transform.position.z);
		}
	}

	public void AddSurfacesList(List<Surface> surfacesList)
	{
		foreach (Surface item in surfacesList)
		{
			SurfacesList.Add(item);
		}
		SetChinderPosition();
	}

	/// <summary>
	/// 获取当前子节点下颜色类别总数
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
        return ColorType.Count;
    }

	/// <summary>
	/// 从当前的Surface里面从0开始移除指定颜色的surface，并返回移除的surface数组
	/// </summary>
	/// <param name="itemColor"></param>
	/// <returns></returns>
	public List<Surface> RemoveSurfaces(ItemColorType itemColor) {
		List<Surface> surfaces= new List<Surface>();
        int x = 0;
        for (int i = 0; i < SurfacesList.Count; i++)
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
		SurfacesList.RemoveRange(0, x);
        return surfaces;
	}


	public void AddSurfaces(List<Surface> listsurface, MoveTweenType moveTweenType,Action action=null)
	{
		if (listsurface[0].GetColorType() == SurfacesList[0].GetColorType())
		{
			if (moveTweenType== MoveTweenType.One) {
				int x = 0;
				for (int i = 0; i < listsurface.Count; i++)
				{
					Vector3 targetPosition = new Vector3(transform.position.x, (SurfacesList.Count + i * Assign_Y) * Assign_Y, transform.position.z);
					listsurface[x].transform.DOMove(targetPosition, 0.15f).SetEase(Ease.InOutQuad).OnComplete(() =>
					{
						listsurface[x].transform.SetParent(transform);
						if (i == listsurface.Count - 1)
						{
							SurfacesList.AddRange(listsurface);
							SetChinderPosition();
							Debug.Log("移动完成，开始下一轮的判定");
							action?.Invoke();
						}
					});
				}
			}
			else if(moveTweenType == MoveTweenType.Continuity) {
				Surface surfacePic = listsurface[0];
				Vector3 targetPosition = new Vector3(transform.position.x, (SurfacesList.Count + Assign_Y) * Assign_Y, transform.position.z);
				surfacePic.transform.DOMove(targetPosition, 0.15f).SetEase(Ease.InOutQuad).OnComplete(() =>
				{
					surfacePic.transform.SetParent(transform);
					listsurface.Remove(surfacePic);
					SurfacesList.Insert(0,surfacePic);
					SetChinderPosition();
					if (listsurface.Count<=0) {
						Debug.Log("移动一个完成，开始下一轮的判定");
						action?.Invoke();
					}
				});
			}
		}
	}


	/// <summary>
	/// 获取当前顶端的surface颜色
	/// </summary>
	/// <returns></returns>
	public ItemColorType GetTopColor(){
        return SurfacesList[0].GetColorType();

	}

	/// <summary>
	/// 叠
	/// </summary>
	/// <param name="surfacesList"></param>
	/// <param name="isOn"></param>
	public void EntrIntoWarehouse(List<Surface> surfacesList) {
        for (int i = 0; i < surfacesList.Count; i++)
        {
            
        }
    }
}
