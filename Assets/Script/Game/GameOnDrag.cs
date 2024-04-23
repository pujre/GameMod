using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOnDrag: MonoBehaviour
{
	private bool dragging = false;

	void OnMouseDown()
	{
		dragging = true;
	}

	void OnMouseDrag()
	{
		if (dragging)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 objPos = transform.position;
			objPos.x = mousePos.x;
			objPos.y = mousePos.y;
			transform.position = objPos;
		}
	}

	void OnMouseUp()
	{
		dragging = false;
	}
}
