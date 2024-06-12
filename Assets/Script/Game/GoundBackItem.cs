using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.StickyNote;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
	public bool IsLock = false;//true表示锁上，需要解锁
	public float delayBetweenMoves = 0.35f;  // 每个对象移动之间的延迟

	public float GoundBack_Y = 1.2f;
	public float Assign_Y=0.25f;
	/// <summary>
	/// 该节点所属得坐标
	/// </summary>
	public Vector2Int ItemPosition;
	public List<Surface> SurfacesList = new List<Surface>();

	private void Awake()
	{
		GoundBack_Y = 1.9f;
		Assign_Y = 0.65f;
		SurfacesList = new List<Surface>();
		DelegateManager.Instance.AddEvent(OnEventKey.OnCalculate.ToString(), DelegateCallback);
	}
	void DelegateCallback(object[] args)
	{
		if (args.Length >= 2 && args[0] is int x && args[1] is int y) {
			RemoveTopColorObject(x, y);
		}
	}

	public void LockOrUnLockTheItem(bool isOn){
		IsLock=isOn;
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
		return SurfacesList.Count == 0|| IsLock ? true:false;
	}

	/// <summary>
	/// 从新布局位置
	/// </summary>
	public void SetChinderPosition() {
		if (SurfacesList == null) return;
        for (int i = 0; i < SurfacesList.Count; i++)
        {
			SurfacesList[i].transform.localPosition = new Vector3(0,GoundBack_Y+ (i * Assign_Y), 0);
		}
	}

	public List<Vector3> GetEndListVector3(int x) { 
		List<Vector3> vectors = new List<Vector3>();
		Vector3 startVectpr3 = SurfacesList[SurfacesList.Count-1].transform.localPosition;
		for (int i = 0; i < x; i++)
		{
			vectors.Add(new Vector3(startVectpr3.x, startVectpr3.y + (i * Assign_Y), startVectpr3.z));
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
	/// 获取当前顶部同颜色的节点数
	/// </summary>
	/// <returns></returns>
	public int GetTopColorNumber()
	{
		if (SurfacesList == null || SurfacesList.Count == 0) return 0;
		string colorTypeName = SurfacesList[SurfacesList.Count-1].GetColorType().ToString();
		int colorNumber = 0;
        for (int i = SurfacesList.Count - 1; i >= 0; i--)
        {
			if (colorTypeName == SurfacesList[i].GetColorType().ToString())
			{
				colorNumber++; 
			}
			else {
				break;
			}
        }
		return colorNumber;
    }

	/// <summary>
	/// 从当前的Surface里面从0开始移除指定颜色的surface，并返回移除的surface数组
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


	public void AddSurfaces(List<Surface> listsurface, Action action = null)
	{
			Sequence sequence = DOTween.Sequence();  // 创建一个DoTween序列
			List<Vector3> o3s = GetEndListVector3(listsurface.Count);

			for (int i = 0; i < listsurface.Count; i++)
			{
				var obj= listsurface[i];

				obj.transform.SetParent(transform);
				Vector3 ka = new Vector3((o3s[i].x + obj.transform.localPosition.x) / 2, o3s[i].y + 5, (o3s[i].z + obj.transform.localPosition.z) / 2);
				// 为每个对象添加一个移动到终点的动画，并在前一个动画结束后开始
				sequence.AppendCallback(() => {
					AudioManager.Instance.PlaySFX("Flip（翻转叠加时）");
				});
				sequence.Append(obj.transform.DOLocalMove(obj.transform.localPosition + new Vector3(0,5,0), 0.08f).SetEase(Ease.Linear));
				sequence.Append(obj.transform.DOLocalMove(ka, 0.08f).SetEase(Ease.Linear));
				sequence.Append(obj.transform.DOLocalMove(o3s[i], 0.08f).SetEase(Ease.Linear));
				sequence.AppendInterval(delayBetweenMoves);  // 在每个对象移动后添加延迟
				SurfacesList.Add(obj);
			}

			sequence.OnComplete(() => {
				SetChinderPosition();
				action?.Invoke();
			});
			sequence.Play();  // 播放序列
		
	}

	/// <summary>
	/// 获取当前顶端的surface颜色
	/// </summary>
	/// <returns></returns>
	public ItemColorType GetTopColor(){
		return SurfacesList[SurfacesList.Count-1].GetColorType();
	}

	/// <summary>
	/// 获取当前从顶端开始往下数第x个不同颜色的surface颜色
	/// </summary>
	/// <param name="x"></param>
	/// <returns></returns>
	public ItemColorType GetSpecifyLayerColor(int x)
	{
		List<string> ColorType = new List<string>();
		for (int i = 0; i < SurfacesList.Count; i++)
		{
			string colorTypeName = SurfacesList[i].GetColorType().ToString();
			if (!ColorType.Contains(colorTypeName))
			{
				ColorType.Add(colorTypeName);
			}
		}
		ItemColorType colorEnum;
		colorEnum = (ItemColorType)Enum.Parse(typeof(ItemColorType), ColorType[x]);
		return colorEnum;
	}

	/// <summary>
	/// 比较器
	/// </summary>
	/// <param name="item2"></param>
	/// <returns></returns>
	public int Compare(GoundBackItem item2)
	{
		if (GetNowColorNumber() > 1 && item2.GetNowColorNumber() > 1 &&
		GetSpecifyLayerColor(1) == item2.GetSpecifyLayerColor(1))
		{
			return 1;
		}
		else {
			return -1;
		}
	}
	/// <summary>
	/// 移除满足条件的顶端的物体
	/// </summary>
	public void RemoveTopColorObject(int x,int y)
	{
		int count = GetTopColorNumber();
		if (count >= 10&& GetNowColorNumber()==1)
		{
			List<Surface> sl = RemoveSurfaces(GetTopColor());
			Sequence sequence = DOTween.Sequence();  // 创建一个DoTween序列
			for (int i = 0; i < sl.Count; i++)
			{
				var obj = sl[i];
				Vector3 ka = obj.transform.localPosition + new Vector3(0,5, 0);

				// 创建一个子序列来同时进行移动和缩放的变化
				Sequence subSequence = DOTween.Sequence();
				subSequence.AppendCallback(() => {
					AudioManager.Instance.PlaySFX("Ding（消完飞上去增加积分）");
				});
				subSequence.Join(obj.transform.DOLocalMove(ka, 0.1f).SetEase(Ease.Linear));
				subSequence.Join(obj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f).SetEase(Ease.Linear));

				// 在子序列完成时执行回调
				subSequence.OnComplete(() =>
				{
					PoolManager.Instance.DestoryByRecycle(obj.gameObject,false);
					obj.transform.localScale = Vector3.one;
					DelegateManager.Instance.TriggerEvent(OnEventKey.OnBonusEvent.ToString(), 1);
				});

				// 将子序列添加到主序列中
				sequence.Append(subSequence);
				sequence.AppendInterval(delayBetweenMoves);  // 在每个对象移动后添加延迟
			}
			sequence.OnComplete(() =>
			{
				SetChinderPosition();
				GameManager.Instance.CalculateElimination(x, y);
			});
			sequence.Play();  // 播放序列
		}
		else
		{
			//Debug.Log(string.Format("坐标 {0},{1}未满足条件，当前数为{2}", x, y, count));
		}
	}
}

