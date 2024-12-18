using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class UnitSDF : MonoBehaviour
{
	/// <summary>
	/// 获取指定坐标的周围的坐标点
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
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
	//public static void DFS(Vector2Int coord, List<Vector2Int> coordinates, HashSet<Vector2Int> visited, List<Vector2Int> group)
	//{
	//	visited.Add(coord);
	//	group.Add(coord);

	//	foreach (var neighbor in GameManager.Instance.GetAroundPos(coord.x, coord.y))
	//	{
	//		if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
	//		{
	//			DFS(neighbor, coordinates, visited, group);
	//		}
	//	}
	//}

	public static void DFS(Vector2Int coord, List<Vector2Int> vector2Ints , HashSet<Vector2Int> visited, List<Vector2Int> group)
	{
		Stack<Vector2Int> stack = new Stack<Vector2Int>();
		stack.Push(coord);
		HashSet<Vector2Int> coordinates= new HashSet<Vector2Int>(vector2Ints);
		while (stack.Count > 0)
		{
			var current = stack.Pop();
			if (visited.Contains(current)) continue;

			visited.Add(current);
			group.Add(current);

			foreach (var neighbor in GameManager.Instance.GetAroundPos(current.x, current.y))
			{
				// 如果邻居在 coordinates 中且未被访问过，则将邻居压入栈
				if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
				{
					stack.Push(neighbor);
				}
			}
		}
	}

	/// <summary>
	/// 从指定点开始搜寻该点周围所有的点
	/// </summary>
	/// <param name="startHex"></param>
	/// <returns></returns>
	public static List<Vector2Int> FindConnectedPieces(Vector2Int startHex)
	{
		List<Vector2Int> vector2Ints = new List<Vector2Int>();
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

		queue.Enqueue(startHex);
		visited.Add(startHex);
		vector2Ints.Add(startHex);
		while (queue.Count > 0)
		{
			Vector2Int currentHex = queue.Dequeue();
			vector2Ints.Add(currentHex);
			Debug.Log(string.Format("现在搜索坐标点X:{0},Y:{1}点附件的点", currentHex.x, currentHex.y));
			List<Vector2Int> aroundCan = GameManager.Instance.GetAroundCanBeOperatedPos(currentHex.x, currentHex.y);
			Debug.Log(string.Format("坐标点X:{0},Y:{1}点附件的符合要求的点的数量为：{2}", currentHex.x, currentHex.y,aroundCan.Count));
			for (int i = 0; i < aroundCan.Count; i++)
			{
				if (!visited.Contains(aroundCan[i]))
				{
					queue.Enqueue(aroundCan[i]);
					visited.Add(aroundCan[i]);
				}
			}
		}
		return vector2Ints;
	}


	/// <summary>
	/// 深度搜索
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		List<InstructionData> instructionData = new List<InstructionData>();
		Debug.Log("____________________________________");
		// 查找起点
		Vector2Int start = coordinates.FirstOrDefault(pos =>
		{
			List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(pos.x, pos.y).Where(pos => coordinates.Contains(pos)).ToList();
			Debug.Log(string.Format("查找起点--当前检查的点的坐标为{0},{1}，当前该点周围的的数组长度为：{2}", pos.x, pos.y, neighbors.Count));
			return neighbors.Count == 1;
		});
		Debug.Log(string.Format("查找起点--筛选后点的坐标为X:{0},Y:{1}", start.x, start.y));
		if (start == Vector2Int.zero) {
			start = coordinates.FirstOrDefault(pos =>
			{
				List<Vector2Int> leght = GameManager.Instance.GetAroundPos(pos.x, pos.y).Where(pos => coordinates.Contains(pos)).ToList();
				return leght.Count > 1;
			});
			Debug.Log(string.Format("查找起点--没有符合要求的点，重新筛选--筛选后点的坐标为X:{0},Y:{1}", start.x, start.y));
		}
		// 用于记录访问过的节点和未访问的邻居
		Dictionary<Vector2Int, InstDataCalculus> visitedNodes = new Dictionary<Vector2Int, InstDataCalculus>();
		bool isOn=false;
		int index = 0;
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		while(!isOn)
		{
			List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y).Where(pos => coordinates.Contains(pos)&& !visited.Contains(pos)).ToList(); // 获取当前坐标周围的坐标
			string aroundString = string.Join(", ", around.Select(pos => pos.ToString()).ToArray());
			Debug.Log(string.Format("坐标为：{0},长度为：{1},Index为：{2},已访问过的数为：{3}，当前总数为：{4}，含有以下这些点：{5}", start,around.Count,index,visited.Count, coordinates.Count, aroundString));
			if (around.Count == 0)
			{
				if (instructionData.Count == coordinates.Count - 1)
				{
					Debug.Log("没有更多节点可以访问，结束,返回的路径长度为："+ instructionData.Count);
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

				Debug.Log("回溯了" + backtrackCount + "次");
				//continue;
			}


			if (around.Count == 1)
			{
				Vector2Int next = around[0];
				instructionData.Add(new InstructionData(start, next));
				visited.Add(start);
				Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
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
					Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", around[0], start));
					visited.Add(around[0]);
				}
				else {
					index++;
					// 记录当前节点及其未访问的邻居
					UpdateVisitedNodes(visitedNodes, start, index, around);
					Vector2Int next = around[0];
					instructionData.Add(new InstructionData(start, next));
					Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
					visited.Add(start);
					start = next;
				}
			}
			else
			{
				// 没有更多节点可以访问，结束
				Debug.Log("没有更多节点可以访问，结束,返回的路径长度为：" + instructionData.Count);
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
