using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GoundBackItem : MonoBehaviour
{
	public float delayBetweenMoves = 0.35f;  // ÿ�������ƶ�֮����ӳ�

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
		DelegateManager.Instance.AddEvent(OnEventKey.OnCalculate.ToString(), DelegateCallback);
	}
	void DelegateCallback(object[] args)
	{
		if (args.Length >= 2 && args[0] is int x && args[1] is int y) {
			RemoveTopColorObject(x, y);
		}
		
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
	/// ��ȡ��ǰ����ͬ��ɫ�Ľڵ���
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
				Vector3 ka = new Vector3((o3s[i].x + obj.transform.localPosition.x) / 2, o3s[i].y + 5, (o3s[i].z + obj.transform.localPosition.z) / 2);

				// Ϊÿ���������һ���ƶ����յ�Ķ���������ǰһ������������ʼ
				sequence.Append(obj.transform.DOLocalMove(ka, 0.1f).SetEase(Ease.Linear));
				sequence.Append(obj.transform.DOLocalMove(o3s[i], 0.1f).SetEase(Ease.Linear));
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
	/// �Ƴ����������Ķ��˵�����
	/// </summary>
	public void RemoveTopColorObject(int x,int y)
	{
		Debug.Log("�Ƴ�����");
		int count = GetTopColorNumber();
		if (count >= 8)
		{
			List<Surface> sl = RemoveSurfaces(GetTopColor());
			Debug.Log("�Ƴ����������Ķ��˵�����");
			Sequence sequence = DOTween.Sequence();  // ����һ��DoTween����
			for (int i = 0; i < sl.Count; i++)
			{
				var obj = sl[i];
				Vector3 ka = obj.transform.localPosition + new Vector3(0,5, 0);

				// ����һ����������ͬʱ�����ƶ���͸���ȱ仯
				Sequence subSequence = DOTween.Sequence();
				subSequence.Join(obj.transform.DOLocalMove(ka, 0.1f).SetEase(Ease.Linear));
				subSequence.Join(obj.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.1f).SetEase(Ease.Linear));

				// �����������ʱִ�лص�
				subSequence.OnComplete(() =>
				{
					GameManager.Instance.ReturnObject(obj.gameObject);
					obj.transform.localScale = Vector3.one;
				});

				// ����������ӵ���������
				sequence.Append(subSequence);
				sequence.AppendInterval(delayBetweenMoves);  // ��ÿ�������ƶ�������ӳ�
			}
			sequence.OnComplete(() =>
			{
				SetChinderPosition();
				GameManager.Instance.CalculateElimination(x, y);
			});
			sequence.Play();  // ��������
		}
		else
		{
			Debug.Log(string.Format("���� {0},{1}δ������������ǰ��Ϊ{2}", x, y, count));
		}
	}
}

