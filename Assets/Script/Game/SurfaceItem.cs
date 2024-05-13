using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceItem : MonoBehaviour
{
	public GameObject Prefab;
	public List<Surface> Surfaces = new List<Surface>();

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
			GameObject gameObject = Instantiate(Prefab,new Vector3(0, i,0), Quaternion.identity, transform);
			gameObject.GetComponent<Surface>().SetColor(color[i]);
		}
    }


}
