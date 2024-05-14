using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SurfaceItem : MonoBehaviour
{
	public bool IsOnMove = false;//是否允许拖动
	public Vector3 QreVector3;//初始坐标
	public GameObject Prefab;
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
	/// 生成指定颜色数得随机surface
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
			GameObject gameObject = Instantiate(Prefab,new Vector3(transform.position.x, transform.position.y+i, transform.position.z),/* Quaternion.Euler(90, 0, 0)*/ Quaternion.identity, transform);
			gameObject.GetComponent<Surface>().SetColor(color[i]);
			Surfaces.Add(gameObject.GetComponent<Surface>());
		}
    }



}
