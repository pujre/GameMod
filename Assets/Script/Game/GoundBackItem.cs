using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VolumetricLines;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
	public bool IsLock = false;//true表示锁上，需要解锁
	public float delayBetweenMoves = 0.35f;  // 每个对象移动之间的延迟
	public GameObject NumberText;
	public VolumetricLineStripBehavior volumetricLine;
	public float GoundBack_Y;
	public float Assign_Y;
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
		//volumetricLineList.gameObject.SetActive(false);
		//DelegateManager.Instance.AddEvent(OnEventKey.OnCalculate.ToString(), DelegateCallback);
	}


	public void LockOrUnLockTheItem(bool isOn) {
		IsLock = isOn;
	}

	public GoundBackItem(int x, int y, string name) {
		SetData(x, y, name);
	}

	public void SetData(int x, int y, string name)
	{
		ItemPosition = new Vector2Int(x, y);
		if (gameObject) gameObject.name = name;
		volumetricLine = transform.Find("LineStrip-LightSaber").GetComponent<VolumetricLineStripBehavior>();
		volumetricLine.gameObject.SetActive(false);
	}

	/// <summary>
	/// 设置特效的开启和关闭
	/// </summary>
	/// <param name="isOn"></param>
	public void SetvolumetricLine(bool isOn) {
		volumetricLine.gameObject.SetActive(isOn);
	}

	/// <summary>
	/// 当前堆上没有物品且没有被锁
	/// </summary>
	/// <returns></returns>
	public bool IsAddSurface() {
		return SurfacesList.Count == 0 && !IsLock ? true : false;
	}

	/// <summary>
	/// 当前堆上有物品
	/// </summary>
	/// <returns></returns>
	public bool IsSurface()
	{
		return SurfacesList.Count > 0;
	}

	/// <summary>
	/// 指定时间内将所有物体颜色设置为指定颜色
	/// </summary>
	/// <param name="targetColor"></param>
	/// <param name="duration"></param>
	/// <param name="colorType"></param>
	public void SetAllColor(Color targetColor, float duration, ItemColorType colorType) {
		Sequence sequence = DOTween.Sequence();  // 创建一个DoTween序列
		for (int i = 0; i < SurfacesList.Count; i++)
		{
			var target = SurfacesList[i];
			var targetMaterial = target.gameObject.GetComponent<MeshRenderer>().material;
			float delay = 0.03f * i;
			sequence.Insert(delay,
				targetMaterial.DOColor(targetColor, duration)
					.SetEase(Ease.Linear)
					.OnStart(() => {
						target.SetColorType(colorType);
					})
					.OnComplete(() => {

					})
			);
		}
		sequence.OnComplete(() =>
		{

		});
		sequence.Play();  // 播放序列
	}

	/// <summary>
	/// 从新布局位置
	/// </summary>
	public void SetChinderPosition() {
		if (SurfacesList == null) return;
		for (int i = 0; i < SurfacesList.Count; i++)
		{
			SurfacesList[i].transform.localPosition = new Vector3(0, GoundBack_Y + (i * Assign_Y), 0);
		}
	}

	/// <summary>
	/// 显示最顶层的数字
	/// </summary>
	public void DisplayNumbers(bool isSet) {
		NumberText.SetActive(isSet);
		if (isSet) {
			NumberText.transform.localPosition = new Vector3(0, GoundBack_Y + (SurfacesList.Count * Assign_Y), 0);
		}
		NumberText.GetComponent<Text>().text = GetTopColorNumber().ToString();
	}

	public List<Vector3> GetEndListVector3(int x) {
		List<Vector3> vectors = new List<Vector3>();
		Vector3 startVectpr3 = SurfacesList[SurfacesList.Count - 1].transform.position;
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
		string colorTypeName = SurfacesList[SurfacesList.Count - 1].GetColorType().ToString();
		int colorNumber = 0;
		for (int i = SurfacesList.Count - 1; i >= 0; i--)
		{
			if (colorTypeName == SurfacesList[i].GetColorType().ToString() || SurfacesList[i].GetColorType()==ItemColorType.StarAll)
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
	public List<Surface> RemoveSurfaces() {
		List<Surface> surfaces = new List<Surface>();
		ItemColorType itemColor = SurfacesList[SurfacesList.Count - 1].GetColorType();
		for (int i = SurfacesList.Count - 1; i >= 0; i--)
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
		Vector3 direction = (listsurface[listsurface.Count - 1].transform.position - o3s[o3s.Count - 1]).normalized;
		for (int i = 0; i < listsurface.Count; i++)
		{
			var obj = listsurface[i];
			Vector3 controlPoint = new Vector3((o3s[i].x + obj.transform.position.x) / 2, o3s[i].y + 10, (o3s[i].z + obj.transform.position.z) / 2);
			obj.transform.SetParent(transform);
			Vector3[] path = new Vector3[] { obj.transform.position, controlPoint, o3s[i] };
			float delay = 0.03f * i;
			// 计算垂直向量（法向量）
			Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
			// 创建一个表示沿着法向量旋转180度的四元数
			Quaternion rotation = Quaternion.AngleAxis(180, normal);
			sequence.Insert(delay,
				obj.transform.DOPath(path, 0.5f, PathType.CatmullRom)
					.SetEase(Ease.Linear)
					.OnStart(() => {
						AudioManager.Instance.PlaySFX("Flip（翻转叠加时）");
						obj.transform.DORotateQuaternion(rotation, 0.45f)
								 .SetEase(Ease.Linear);
					})
					.OnComplete(() => {
						SurfacesList.Add(obj);
					})
			);
		}

		sequence.OnComplete(() =>
		{
			//SetChinderPosition();
			action?.Invoke();
		});
		sequence.Play();  // 播放序列
	}

	/// <summary>
	/// 获取当前顶端的surface颜色
	/// </summary>
	/// <returns></returns>
	public ItemColorType GetTopColor() {
		return SurfacesList[SurfacesList.Count - 1].GetColorType();
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

	public void TopTranslateColor(int x){
		int startIndex = Mathf.Max(SurfacesList.Count - x, 0);
		for (int i = SurfacesList.Count-1; i >= startIndex; i--)
		{
			//Debug.Log(string.Format("X:{0},Y:{1},开始变色：i={2}", ItemPosition.x, ItemPosition.y,i));
			if (i >= startIndex)
			{
				SurfacesList[i].TranslateColore(Color.white, () => {
					GameManager.Instance.CalculateElimination(ItemPosition.x, ItemPosition.y);
				});
			}
			else {
				SurfacesList[i].TranslateColore(Color.white);
			}
		}
	}
	/// <summary>
	/// 移除满足条件的顶端的物体
	/// </summary>
	public void RemoveTopColorObject(Action action=null)
	{
		int count = GetTopColorNumber();
		if (count >= 10/*&& GetNowColorNumber()==1*/)
		{
			List<Surface> sl = RemoveSurfaces();
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
				subSequence.Join(obj.transform.DOLocalMove(ka, 0.05f).SetEase(Ease.Linear));
				subSequence.Join(obj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.05f).SetEase(Ease.Linear));
				//subSequence.Insert()
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
				//SetChinderPosition();
				if (IsSurface()) {
					GameManager.Instance.OperationPath.Add(ItemPosition);
				}
				action!.Invoke();
			});
			sequence.Play();  // 播放序列
		}
		else
		{
			action!.Invoke();
			//Debug.Log(string.Format("坐标 {0},{1}未满足条件，当前数为{2}", x, y, count));
		}
	}

	/// <summary>
	/// 移除所有物体
	/// </summary>
	public void RemoveObject(Action action = null)
	{
		Sequence sequence = DOTween.Sequence();
		//for (int i = SurfacesList.Count-1; i>=0; i--)
		for (int i = 0; i < SurfacesList.Count; i++)
		{
			GameObject obj = SurfacesList[i].gameObject;
			Vector3 ka = obj.transform.localPosition + new Vector3(0, 50, 0);
			// 创建并播放子序列
			Tween scaleTween = obj.transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f),0.5f).SetEase(Ease.Linear);
			Tween moveTween  = obj.transform.DOLocalMove(ka, 0.5f).SetEase(Ease.Linear);

			sequence.Insert(0.08f * (SurfacesList.Count - 1 - i), scaleTween);
			sequence.Insert(0.08f * (SurfacesList.Count - 1 - i), moveTween);
			moveTween.OnComplete(() =>
			{
				PoolManager.Instance.DestoryByRecycle(obj.gameObject);
				obj.transform.localScale = Vector3.one;
			});
		}
		sequence.OnComplete(() =>
		{
			SurfacesList.Clear();
		});
		sequence.Play(); // 立即播放每个子序列

	}
}

