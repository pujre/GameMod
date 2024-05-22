using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SurfaceItem : MonoBehaviour
{
	public bool IsOnMove = false;//�Ƿ������϶�
	public Vector3 QreVector3;//��ʼ����
	public List<Surface> Surfaces = new List<Surface>();


	public void QurStart(Vector3 pos) {
		QreVector3 = pos;
		transform.DOMove(pos, 0.3f).OnComplete(() => {
			IsOnMove = true;
		});
	}

	public void QueMoveCancel()
	{
		transform.DOMove(QreVector3, 0.3f).OnComplete(() => {
			IsOnMove = true;
		});
	}

	public void QueMoveEnd(){
		IsOnMove = false;
	}

	/// <summary>
	/// ����ָ����ɫ�������surface
	/// </summary>
	/// <param name="colorNumber"></param>
	public void CreatorSurface(int colorNumber=3) {
		List<int> color = new List<int>();
		for (int i = 0; i < 10; i++)
		{
			color.Add(Random.Range(0, colorNumber));
		}
		color.Sort();
        for (int i = 0; i < color.Count; i++)
        {
			GameObject gameObject = PoolManager.Instance.CreateGameObject("surface");
			gameObject.transform.SetParent(transform);
			gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + i, transform.position.z);
			gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
			gameObject.GetComponent<Surface>().SetColor(color[i]);
			Surfaces.Add(gameObject.GetComponent<Surface>());
		}
    }



}
