using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Surface : MonoBehaviour
{
	private float Assign_Y=1.3f;

	void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void AssignPositions()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			child.position = new Vector3(child.position.x, i*Assign_Y, child.position.z);
		}
	}
}
