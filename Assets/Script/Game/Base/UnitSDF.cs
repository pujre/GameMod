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

	/// <summary>
	/// 深度搜索
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		List<InstructionData> instructionData = new List<InstructionData>();
		//Debug.Log("____________________________________");
		// 查找起点
		Vector2Int start = coordinates.FirstOrDefault(pos =>
		{
			List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(pos.x, pos.y).Where(pos => coordinates.Contains(pos)).ToList();
			//Debug.Log(string.Format("当前检查的点的坐标为{0},{1}，当前该点周围的的数组长度为：{2}", pos.x, pos.y, neighbors.Count)); ;
			return neighbors.Count == 1;
		});

		// 用于记录访问过的节点和未访问的邻居
		Dictionary<Vector2Int, InstDataCalculus> visitedNodes = new Dictionary<Vector2Int, InstDataCalculus>();
		bool isOn=false;
		int index = 0;
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		//Debug.Log("Sart的坐标为："+ start.x+","+start.y);
		while(!isOn)
		{
			List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y).Where(pos => coordinates.Contains(pos)&& !visited.Contains(pos)).ToList(); // 获取当前坐标周围的坐标
			//Debug.Log(string.Format("坐标为：{0},长度为：{1},Index为：{2},已访问过的数为：{3}，当前数为：{4}", start,around.Count,index,visited.Count, coordinates.Count));
			if (around.Count == 0)
			{
				if (instructionData.Count == coordinates.Count - 1)
				{
					//Debug.Log("没有更多节点可以访问，结束,返回的路径长度为："+ instructionData.Count);
					isOn = true;
					break;
				}

				// 回溯：当前节点没有未访问的邻居，从记录中取回上一个节点
				var lastEntry = visitedNodes.Last();
				start = lastEntry.Key;
				around = lastEntry.Value.VisitedNodes.Except(new List<Vector2Int> { start }).ToList();
				int backtrackCount = index - lastEntry.Value.Index;
                for (int i = 0; i < backtrackCount; i++)
				{
					visited.Remove(instructionData[instructionData.Count - 1].StarVector2);
				}
                instructionData.RemoveRange(instructionData.Count - backtrackCount, backtrackCount);
				index -= backtrackCount;

				//Debug.Log("回溯了" + backtrackCount + "次");
				//continue;
			}


			if (around.Count == 1)
			{
				Vector2Int next = around[0];
				instructionData.Add(new InstructionData(start, next));
				visited.Add(start);
				//Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
				start = next;
				index++;
				if (visitedNodes.ContainsKey(start)) {
					visitedNodes.Remove(start);
				}

			}
			else if (around.Count > 1)
			{
				if (GameManager.Instance.GetAroundPos(around[0].x, around[0].y).Where(pos => coordinates.Contains(pos)).ToList().Count==1) {
					index++;
					UpdateVisitedNodes(visitedNodes, start, index, around);
					instructionData.Add(new InstructionData(around[0], start));
					//Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", around[0], start));
					visited.Add(around[0]);
				}
				else {
					index++;
					// 记录当前节点及其未访问的邻居
					UpdateVisitedNodes(visitedNodes, start, index, around);
					Vector2Int next = around[0];
					instructionData.Add(new InstructionData(start, next));
					//Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
					visited.Add(start);
					start = next;
				}
			}
			else
			{
				// 没有更多节点可以访问，结束
				//Debug.Log("没有更多节点可以访问，结束,返回的路径长度为：" + instructionData.Count);
				isOn = true;
				break;
			}
		}

		return instructionData;
	}

	/// <summary>
	/// 记录当前节点及其未访问的邻居
	/// </summary>
	/// <param name="visitedNodes"></param>
	/// <param name="start"></param>
	/// <param name="index"></param>
	/// <param name="around"></param>
	private static void UpdateVisitedNodes(Dictionary<Vector2Int, InstDataCalculus> visitedNodes, Vector2Int start, int index, List<Vector2Int> around)
	{
		if (!visitedNodes.ContainsKey(start))
		{
			visitedNodes[start] = new InstDataCalculus(index, around);
		}
		else
		{
			visitedNodes[start].Index = index;
		}
		visitedNodes[start].SetFoldNodes(around[0]);
	}

}
