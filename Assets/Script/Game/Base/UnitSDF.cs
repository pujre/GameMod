using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeChatWASM;


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
	/// 筛选指定数组里面的点有哪些是该点相邻的点
	/// </summary>
	/// <param name="star"></param>
	/// <param name="list"></param>
	/// <returns></returns>
	public static List<Vector2Int> GetCreatorPosForList(Vector2Int star, List<Vector2Int> list) {
		List<Vector2Int> vector2s = GetCreatorPos(star.x, star.y);
		return list.Where(v => vector2s.Contains(v)).ToList();
	}

	/// <summary>
	/// 筛选指定数组里面的点有哪些是该点相邻的点且不属于list2里面
	/// </summary>
	/// <param name="star"></param>
	/// <param name="list"></param>
	/// <returns></returns>
	public static List<Vector2Int> GetCreatorPosForListOrList(Vector2Int star, List<Vector2Int> list1, List<Vector2Int> list2)
	{
		List<Vector2Int> vector2s = GetCreatorPos(star.x, star.y);
		return list1.Where(v => vector2s.Contains(v)&& !list2.Contains(v)).ToList();

	}


	/// <summary>
	/// 筛选相邻的坐标
	/// </summary>
	/// <param name="coord"></param>
	/// <param name="vector2Ints"></param>
	/// <param name="visited"></param>
	/// <param name="group"></param>
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
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		string log = "";
		queue.Enqueue(startHex);
		visited.Add(startHex);
		while (queue.Count > 0)
		{
			Vector2Int currentHex = queue.Dequeue();
			//Debug.Log(string.Format("-搜寻点- 现在搜索坐标点X:{0},Y:{1}点附件的点", currentHex.x, currentHex.y));
			List<Vector2Int> aroundCan = GameManager.Instance.GetAroundCanBeOperatedPos(currentHex.x, currentHex.y);
			//Debug.Log(string.Format("-搜寻点- 坐标点X:{0},Y:{1}点附件的符合要求的点的数量为：{2}", currentHex.x, currentHex.y,aroundCan.Count));
			for (int i = 0; i < aroundCan.Count; i++)
			{
				if (!visited.Contains(aroundCan[i]))
				{
					queue.Enqueue(aroundCan[i]);
					visited.Add(aroundCan[i]);
					log +=("X:" + aroundCan[i].x+"Y:"+aroundCan[i].y+"，");
				}
			}
		}
		LogManager.Instance.Log(string.Format("-判断该点附件是否有可翻转的点- 点X:{0}，Y：{1}周围所有的点一共有 {2} 个，为：{3}", startHex.x, startHex.y, visited.ToList().Count, log));
		if (visited.ToList().Count == 1)
		{
			return null;
		}
		else { 
			return visited.ToList();
		}
		
	}

	#region Old script
	/// <summary>
	/// 深度搜索
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	//public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	//{
	//	List<InstructionData> instructionData = new List<InstructionData>();
	//	Debug.Log("____________________________________");
	//	// 查找起点
	//	Vector2Int start = FindCountIsOne(coordinates);
	//	Debug.Log(string.Format("查找起点-筛选后点的坐标为X:{0},Y:{1},他的周围点为{2}", start.x, start.y, GameManager.Instance.GetAroundCanBeOperatedPos(start.x, start.y).Count.ToString()));

	//	// 用于记录访问过的节点和未访问的邻居
	//	Dictionary<Vector2Int, InstDataCalculus> visitedNodes = new Dictionary<Vector2Int, InstDataCalculus>();
	//	bool isOn=false;
	//	int index = 0;
	//	HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
	//	while(!isOn)
	//	{
	//		List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y).Where(pos => coordinates.Contains(pos)&& !visited.Contains(pos)).ToList(); // 获取当前坐标周围的坐标
	//		string aroundString = string.Join(", ", around.Select(pos => pos.ToString()).ToArray());
	//		Debug.Log(string.Format("坐标为：{0},长度为：{1},Index为：{2},已访问过的数为：{3}，当前总数为：{4}，含有以下这些点：{5}", start,around.Count,index,visited.Count, coordinates.Count, aroundString));
	//		if (around.Count == 0)
	//		{
	//			if (instructionData.Count == coordinates.Count - 1)
	//			{
	//				Debug.Log("没有更多节点可以访问，结束,返回的路径长度为："+ instructionData.Count);
	//				isOn = true;
	//				break;
	//			}

	//			// 回溯：当前节点没有未访问的邻居，从记录中取回上一个节点
	//			var lastEntry = visitedNodes.Last();
	//			start = lastEntry.Key;
	//			around = lastEntry.Value.VisitedNodes.Except(new List<Vector2Int> { start }).ToList();
	//			int backtrackCount = index - lastEntry.Value.Index;
	//               for (int i = 0; i < backtrackCount; i++)
	//			{
	//				visited.Remove(instructionData[instructionData.Count - 1].StarVector2);
	//			}
	//               instructionData.RemoveRange(instructionData.Count - backtrackCount, backtrackCount);
	//			index -= backtrackCount;

	//			Debug.Log("回溯了" + backtrackCount + "次");
	//			//continue;
	//		}


	//		if (around.Count == 1)
	//		{
	//			Vector2Int next = around[0];
	//			instructionData.Add(new InstructionData(start, next));
	//			visited.Add(start);
	//			Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
	//			start = next;
	//			index++;
	//			if (visitedNodes.ContainsKey(start)) {
	//				visitedNodes.Remove(start);
	//			}

	//		}
	//		else if (around.Count > 1)
	//		{
	//			if (GameManager.Instance.GetAroundPos(around[0].x, around[0].y).Where(pos => coordinates.Contains(pos)).ToList().Count==1) {
	//				index++;
	//				UpdateVisitedNodes(visitedNodes, start, index, around);
	//				instructionData.Add(new InstructionData(around[0], start));
	//				Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", around[0], start));
	//				visited.Add(around[0]);
	//			}
	//			else {
	//				index++;
	//				// 记录当前节点及其未访问的邻居
	//				UpdateVisitedNodes(visitedNodes, start, index, around);
	//				Vector2Int next = around[0];
	//				instructionData.Add(new InstructionData(start, next));
	//				Debug.Log(string.Format("添加了一个InstructionData，start为：{0},end为：{1},", start, next));
	//				visited.Add(start);
	//				start = next;
	//			}
	//		}
	//		else
	//		{
	//			// 没有更多节点可以访问，结束
	//			Debug.Log("没有更多节点可以访问，结束,返回的路径长度为：" + instructionData.Count);
	//			isOn = true;
	//			break;
	//		}
	//	}

	//	return instructionData;
	//}
	#endregion


	public static List<InstructionData> FilterLinkedCoordinates(Vector2Int currentPoint, List<Vector2Int> surroundingPoints)
	{
		// 创建用于存储InstructionData的列表
		List<InstructionData> instructions = new List<InstructionData>();
		// 使用队列进行广度优先搜索（BFS），按层级处理点
		Queue<Vector2Int> queue = new Queue<Vector2Int>();
		// 使用HashSet记录已访问的点，避免重复处理
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		// 从当前点开始
		queue.Enqueue(currentPoint);
		visited.Add(currentPoint);
		int layer = 0; // 初始化层级计数器
		// 按层级处理点
		while (queue.Count > 0)
		{
			int pointsInCurrentLayer = queue.Count;
			// 处理当前层级的所有点
			for (int i = 0; i < pointsInCurrentLayer; i++)
			{
				Vector2Int current = queue.Dequeue();
				// 获取当前点周围的点，并过滤出在surroundingPoints中且未访问的点
				List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(current.x, current.y)
					.Where(v => surroundingPoints.Contains(v) && !visited.Contains(v))
					.ToList();
				// 将每个邻居点加入队列，标记为已访问，并添加到instructions中
				foreach (Vector2Int neighbor in neighbors)
				{
					visited.Add(neighbor);
					queue.Enqueue(neighbor);
					InstructionData data = new InstructionData(neighbor, layer + 1);
					data.SetSatrtAndEnd(neighbor, current);
					instructions.Add(data);
					LogManager.Instance.Log(string.Format("添加层级点，坐标为，X：{0}，Y：{1}，层级为：{2}", neighbor.x, neighbor.y, layer + 1));
				}
			}

			layer++; // 进入下一层级
		}
		instructions = instructions.OrderByDescending(instruction => instruction.Ceng).ToList();
		return instructions;
	}

	private float GetEuclideanDistance(Vector2Int a, Vector2Int b)
	{
		return Vector2Int.Distance(a, b);
	}


	#region Test 1
	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		List<InstructionData> instructionData = new List<InstructionData>();
		LogManager.Instance.Log("___________________________深度搜索_____________________________");
		// 使用HashSet提高查找效率
		HashSet<Vector2Int> coordinateSet = new HashSet<Vector2Int>(coordinates);
		GameManager.Instance.StartPos = coordinates;
		// 查找起点：度为1的节点
		Vector2Int start = FindCountIsOne(coordinates);
		GameManager.Instance.StartPos.Remove(start);
		LogManager.Instance.Log($"找到起点：X={start.x}, Y={start.y}");
		// 初始化访问记录
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		// 初始化路径
		List<InstructionData> path = new List<InstructionData>();

		bool success = FindPath(start, coordinateSet, visited, path, null);
		if (success)
		{
			//Debug.Log($"路径搜索成功，路径长度为：{path.Count}");
			return path;
		}
		else
		{
			if (coordinates.Count == 4)
			{//针对特殊情况
				Vector2Int three = FindCountIsThree(coordinates);
				if (three.x == -1 && three.y == -1)
				{
					LogManager.Instance.Log("1:未能找到覆盖所有节点的完整路径。");
					LogManager.Instance.Log("___________________________深度搜索结束_____________________________");
					return path;
				}
				else
				{
					//普通情况
					for (int k = 0; k < coordinates.Count; k++)
					{
						if (three != coordinates[k])
						{
							InstructionData instruction = new InstructionData(coordinates[k], three);
							path.Add(instruction);
						}
					}
					return path;
				}
			}
			else
			{

				LogManager.Instance.Log("1:未能找到覆盖所有节点的完整路径。");
				LogManager.Instance.Log("___________________________深度搜索结束_____________________________");
				return path;
			}
		}
	}



	/// <summary>
	/// 递归DFS搜索路径
	/// </summary>
	/// <param name="current">当前节点</param>
	/// <param name="coordinateSet">所有棋子坐标的HashSet</param>
	/// <param name="visited">已访问节点的HashSet</param>
	/// <param name="path">当前路径的InstructionData列表</param>
	/// <param name="previous">上一个节点</param>
	/// <returns>是否找到完整路径</returns>
	private static bool FindPath(Vector2Int current, HashSet<Vector2Int> coordinateSet, HashSet<Vector2Int> visited, List<InstructionData> path, Vector2Int? previous)
	{
		visited.Add(current);
		//LogManager.Instance.Log($"访问节点：X={current.x}, Y={current.y}");

		// 如果有上一个节点，则记录路径
		if (previous.HasValue)
		{
			path.Add(new InstructionData(previous.Value, current));
			LogManager.Instance.Log($"添加路径：{previous.Value} -> {current}");
		}

		// 如果所有节点都已访问，返回成功
		if (visited.Count == coordinateSet.Count)
		{

			LogManager.Instance.Log($"——————————————————————————找到路径——————————————————————");
			return true;
		}

		// 获取所有未访问的邻居
		List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(current.x, current.y)
			.Where(pos => coordinateSet.Contains(pos) && !visited.Contains(pos))
			.ToList();

		foreach (var neighbor in neighbors)
		{
			// 尝试访问邻居
			bool found = FindPath(neighbor, coordinateSet, visited, path, current);
			if (found)
			{
				return true;
			}

			// 回溯，移除路径中的最后一步
			if (path.Count > 0)
			{
				InstructionData lastStep = path[path.Count - 1];
				if (lastStep.EndVector2 == neighbor)
				{
					path.RemoveAt(path.Count - 1);
					LogManager.Instance.Log($"回溯路径：移除 {lastStep.StarVector2} -> {lastStep.EndVector2}");
				}
			}
		}

		// 如果无法继续，移除当前节点的访问记录并返回失败
		visited.Remove(current);
		return false;
	}
	#endregion

	#region MyRegion

	#endregion

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


	public static Vector2Int FindCountIsOne(List<Vector2Int> coordinates) {
		HashSet<Vector2Int> coordinateSet = new HashSet<Vector2Int>(coordinates);
		// 预计算所有坐标的邻居数量，以减少重复计算
		Dictionary<Vector2Int, int> neighborsCountDict = new Dictionary<Vector2Int, int>();
		foreach (Vector2Int pos in coordinates)
		{
			List<Vector2Int> neighbors = GameManager.Instance.GetAroundCanBeOperatedPos(pos.x, pos.y);
			int count = neighbors.Count(n => coordinateSet.Contains(n));
			neighborsCountDict[pos] = count;
		}
		Vector2Int start = new Vector2Int(-1,-1);
		start = neighborsCountDict.Where(kvp => kvp.Value == 1)
									  .Select(kvp => kvp.Key)
									  .DefaultIfEmpty(new Vector2Int(-1, -1))
									  .First();
		if (start.x == -1&& start.y == -1)
		{
			// 没有找到则找邻居数量为3的
			start = neighborsCountDict.Where(kvp => kvp.Value == 3)
									  .Select(kvp => kvp.Key)
									  .DefaultIfEmpty(new Vector2Int(-1, -1))
									  .First();
		}
		if (start.x == -1 && start.y == -1)
		{
			// 如果依然没找到则找邻居数量大于1的
			start = neighborsCountDict.Where(kvp => kvp.Value > 1)
									  .Select(kvp => kvp.Key)
									  .DefaultIfEmpty(new Vector2Int(-1, -1))
									  .First();
		}
		return start;
	}


	public static Vector2Int FindCountIsThree(List<Vector2Int> coordinates)
	{
		HashSet<Vector2Int> coordinateSet = new HashSet<Vector2Int>(coordinates);
		// 预计算所有坐标的邻居数量，以减少重复计算
		Dictionary<Vector2Int, int> neighborsCountDict = new Dictionary<Vector2Int, int>();
		foreach (var pos in coordinates)
		{
			var neighbors = GameManager.Instance.GetAroundCanBeOperatedPos(pos.x, pos.y);
			int count = neighbors.Count(n => coordinateSet.Contains(n));
			neighborsCountDict[pos] = count;
		}
		Vector2Int start = new Vector2Int(-1,-1);
		start = neighborsCountDict.Where(kvp => kvp.Value == 3)
									  .Select(kvp => kvp.Key)
									  .DefaultIfEmpty(new Vector2Int(-1, -1))
									  .First();
		return start;
	}
}
