using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOnDrag: MonoBehaviour
{
	Ray ray;
	RaycastHit hit;
	private bool isDragging = false;
	private Vector3 screenPoint;
	private Vector3 offset;
	void Update()
	{
		if (isDragging) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);
			//if (Physics.Raycast(ray, out hit))
			//{
			//	// 如果射线击中了物体，将物体的颜色改变为红色
			//	Renderer rend = hit.transform.GetComponent<Renderer>();
			//	if (rend != null)
			//	{
			//		rend.material.color = Color.red;
			//	}
			//}
		}
	}
	void OnMouseDown()
	{
		isDragging = true;
		screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
		transform.position = curPosition;
	}

	void OnMouseUp() {
		isDragging = false;
	}
}
