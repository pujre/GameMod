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
					// �����µ������б�
					List <Vector2Int> newCoordinates = new List<Vector2Int>(coordinates);
					newCoordinates.Remove(coord1);

					// �ݹ鳢�Ժϲ�ʣ��ĵ�
					if (RecursiveMerge(newCoordinates, visited))
					{
						return true;
					}

					// ����
					newCoordinates.Add(coord1);
					GameManager.Instance.FilterLinked.Remove(instruction);
				}
			}
		}

		return false;
	}

	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		// ����GameManager.Instance.GetAroundPos�ṩ�˻�ȡ��Χ��ķ���
		// ����GameManager.Instance.IsWalkable�ṩ�˼����Ƿ���ߵķ���

		List<InstructionData> instructions = new List<InstructionData>();
		Dictionary<Vector2Int, Vector2Int> predecessors = new Dictionary<Vector2Int, Vector2Int>();

		// ѡ��һ����ʼ�㣬����򻯴���ֱ��ʹ�õ�һ����
		Vector2Int start = coordinates[0];

		// ʹ�ö��н���BFS
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
					predecessors[neighbor] = current; // ��¼ǰ���ڵ�
					queue.Enqueue(neighbor);
				}
			}
		}

		// �ؽ�·��
		foreach (Vector2Int coordinate in coordinates)
		{
			Vector2Int pathTo = new Vector2Int(2, 1); // �������е㶼�ƶ���(2, 1)
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
	//		List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y);// ��ȡ��ǰ������Χ������
	//		if (around.Count == 1)
	//		{

	//			start = around[0];
	//		}
	//		else if (around.Count > 1)
	//		{
	//			//�жϵ�ǰ�ڵ��Ƿ��Ѿ�����¼
	//			//�� ��¼�µ�ǰ��around���нڵ㣬Ȼ��ʼ���Ե�һ���ڵ�
	//			//�ǣ����Ե�ǰ��¼�ڵ��ʣ��ڵ㲢���ýڵ��Ƴ���¼��ϢList<Vector2Int>�������ǰ�ڵ��ʣ��Ϊδ���Խڵ���Ϊ0��List<Vector2Int>=0������ôɾ����ǰ�ڵ㣬���ڵ���Ϣ�µ�List<Vector2Int> 
	//			//Ȼ������һ����¼����Ϣ�Ľڵ���ݣ����û�нڵ���Ϣ�ˣ���ô���false

	//		}
	//	} 
	//	return false;
	//}
}
