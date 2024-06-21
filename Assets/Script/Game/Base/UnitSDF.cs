using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSDF : MonoBehaviour
{
    public static List<Vector2Int> GetCreatorPos(int x,int y) {
		List<Vector2Int> vector2s;
		if (!(x % 2 == 0))
		{
			vector2s = new List<Vector2Int>() {new Vector2Int(x-1,y),new Vector2Int(x-1, y-1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y-1) };
		}
		else
		{
			vector2s = new List<Vector2Int>() {
			new Vector2Int(x-1,y),new Vector2Int(x-1, y+1),new Vector2Int(x, y-1),
			new Vector2Int(x, y+1),new Vector2Int(x + 1, y),new Vector2Int(x + 1, y+1)};
		}
		return vector2s;
	}

	/// <summary>
	/// ɸѡ�����ŵ�����
	/// </summary>
	/// <param name="coord"></param>
	/// <param name="coordinates"></param>
	/// <param name="visited"></param>
	/// <param name="group"></param>
	public static void DFS(Vector2Int coord, List<Vector2Int> coordinates, HashSet<Vector2Int> visited, List<Vector2Int> group)
	{
		visited.Add(coord);
		group.Add(coord);

		foreach (var neighbor in GameManager.Instance.GetAroundPos(coord.x, coord.y))
		{
			if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
			{
				DFS(neighbor, coordinates, visited, group);
			}
		}
	}


	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates) {
		if (coordinates == null || coordinates.Count == 0) return null;
		HashSet<Vector2Int> remainingCoords = new HashSet<Vector2Int>(coordinates);
		List<InstructionData> instructionDatas = new List<InstructionData>();
		Vector2Int start = coordinates.Where(pos =>
		{
			List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(pos.x, pos.y);
			return neighbors.Count == 1;
		}).FirstOrDefault();
		//�������
		Stack<(Vector2Int current, List<Vector2Int> path)> stack = new Stack<(Vector2Int, List<Vector2Int>)>();
		while (stack.Count>0)
        {
			var (current, path) = stack.Pop();
			if (path.Count == coordinates.Count)
			{
				continue;
			}
			List<Vector2Int> around = GameManager.Instance.GetAroundPos(current.x, current.y);// ��ȡ��ǰ������Χ������
			var nextSteps = around.Where(remainingCoords.Contains).ToList();// ɸѡ�������ʵ���Χ����
			if (around.Count == 1) {
				foreach (var next in nextSteps)// �������п��е���һ��
				{
					List<Vector2Int> newPath = new List<Vector2Int>(path) { next };// ����������һ������·��
					stack.Push((next, newPath));// ����·��������ѹ��ջ
					remainingCoords.Remove(next); // �Ӵ����ʼ������Ƴ���ǰ����
				}
			}
			else
			if (around.Count>1) { 

			}else
			if (around.Count == 0 && path.Count < coordinates.Count)
			{
				continue;
			}
		}
        return instructionDatas;
	}
}
