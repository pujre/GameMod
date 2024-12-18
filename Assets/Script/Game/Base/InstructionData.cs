using UnityEngine;

public class InstructionData
{
	public Vector2Int StarVector2;
	public Vector2Int EndVector2;
	public InstructionData() {
	}
	public InstructionData(Vector2Int star, Vector2Int end) {
		StarVector2=star;
		EndVector2=end;
	}
}
