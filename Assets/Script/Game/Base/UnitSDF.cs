using DG.Tweening;
using Mono.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
	/// 筛选相连着得坐标
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


	public static bool RecursiveMerge(List<Vector2Int> coordinates, HashSet<Vector2Int> visited)
	{
		if (coordinates.Count == 0)
		{
			return true;
		}

		for (int i = 0; i < coordinates.Count; i++)
		{
			for (int j = i + 1; j < coordinates.Count; j++)
			{
				Vector2Int coord1 = coordinates[i];
				Vector2Int coord2 = coordinates[j];
				if (GetCreatorPos(coord1.x, coord1.y).Contains(coord2))
				{
					InstructionData instruction = new InstructionData(coord1, coord2);
					GameManager.Instance.FilterLinked.Add(instruction);
					// 创建新的坐标列表
					List <Vector2Int> newCoordinates = new List<Vector2Int>(coordinates);
					newCoordinates.Remove(coord1);

					// 递归尝试合并剩余的点
					if (RecursiveMerge(newCoordinates, visited))
					{
						return true;
					}

					// 回退
					newCoordinates.Add(coord1);
					GameManager.Instance.FilterLinked.Remove(instruction);
				}
			}
		}

		return false;
	}

	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		// 假设GameManager.Instance.GetAroundPos提供了获取周围点的方法
		// 假设GameManager.Instance.IsWalkable提供了检测点是否可走的方法

		List<InstructionData> instructions = new List<InstructionData>();
		Dictionary<Vector2Int, Vector2Int> predecessors = new Dictionary<Vector2Int, Vector2Int>();

		// 选择一个起始点，这里简化处理，直接使用第一个点
		Vector2Int start = coordinates[0];

		// 使用队列进行BFS
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		queue.Enqueue(start);
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

		while (queue.Count > 0)
		{
			Vector2Int current = queue.Dequeue();
			if (!visited.Contains(current))
			{
				visited.Add(current);
				List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(current.x, current.y).Where(pos => coordinates.Contains(pos) && !visited.Contains(pos)).ToList();

				foreach (Vector2Int neighbor in neighbors)
				{
					predecessors[neighbor] = current; // 记录前驱节点
					queue.Enqueue(neighbor);
				}
			}
		}

		// 重建路径
		foreach (Vector2Int coordinate in coordinates)
		{
			Vector2Int pathTo = new Vector2Int(2, 1); // 假设所有点都移动到(2, 1)
			Vector2Int current = coordinate;
			while (predecessors.ContainsKey(current))
			{
				instructions.Add(new InstructionData(current, pathTo));
				current = predecessors[current];
			}
		}

		return instructions;
	}










	//public static bool FilterLinkedCoordinates(Vector2Int start,List<Vector2Int> coordinates)
	//{
	//	if (coordinates == null || coordinates.Count == 0) return false;
	//	if(start==null) start = coordinates.Where(pos =>
	//	{
	//		List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(pos.x, pos.y);
	//		return neighbors.Count == 1;
	//	}).FirstOrDefault();

	//       for (int i = 0; i < coordinates.Count; i++)
	//       {
	//		List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y);// 获取当前坐标周围的坐标
	//		if (around.Count == 1)
	//		{

	//			start = around[0];
	//		}
	//		else if (around.Count > 1)
	//		{
	//			//判断当前节点是否已经被记录
	//			//否 记录下当前的around所有节点，然后开始尝试第一个节点
	//			//是，尝试当前记录节点的剩余节点并将该节点移出记录信息List<Vector2Int>，如果当前节点的剩余为未尝试节点数为0（List<Vector2Int>=0），那么删除当前节点，及节点信息下的List<Vector2Int> 
	//			//然后往上一个记录了信息的节点回溯，如果没有节点信息了，那么输出false

	//		}
	//	} 
	//	return false;
	//}
}
