using UnityEngine;

public class InstructionData
{
	public Vector2Int StarVector2;
	public Vector2Int EndVector2;

	public Vector2Int NowVector2;//当前点坐标
	public int Ceng = 0;//距离目标点的层级
	public InstructionData() {
	}
	public InstructionData(Vector2Int star, Vector2Int end) {
		StarVector2=star;
		EndVector2=end;
	}

	public void SetSatrtAndEnd(Vector2Int star, Vector2Int end) {
		StarVector2 = star;
		EndVector2 = end;
	}

	public InstructionData(Vector2Int star,int ceng) {
		StarVector2=star;
		Ceng = ceng;
	}

	public override string ToString()
	{
		return $"Start: {StarVector2}, End: {EndVector2}";
	}
}
