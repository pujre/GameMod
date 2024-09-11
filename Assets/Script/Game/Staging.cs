using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staging : MonoBehaviour
{
	public GameObject StagingFace;

	public bool IsStaging()
	{
		if (StagingFace)
		{
			return false;
		}else
		{
			return true;
		}
	}
	public void AddAndRemoveStaging(GameObject stagingFace=null){
		StagingFace = stagingFace;
	}
}
