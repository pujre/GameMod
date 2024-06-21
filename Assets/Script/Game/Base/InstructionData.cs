using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionData : MonoBehaviour
{
	public Vector2Int StarVector2;
	public Vector2Int EndVerctor2;
	public InstructionData(Vector2Int star, Vector2Int end) {
		StarVector2=star;
		EndVerctor2=end;
	}
}
