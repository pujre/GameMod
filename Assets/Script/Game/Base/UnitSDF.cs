using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class UnitSDF : MonoBehaviour
{
	/// <summary>
	/// ��ȡָ���������Χ�������
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
	/// ɸѡ���ڵ�����
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
				// ����ھ��� coordinates ����δ�����ʹ������ھ�ѹ��ջ
				if (coordinates.Contains(neighbor) && !visited.Contains(neighbor))
				{
					stack.Push(neighbor);
				}
			}
		}
	}

	/// <summary>
	/// ��ָ���㿪ʼ��Ѱ�õ���Χ���еĵ�
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
			//Debug.Log(string.Format("-��Ѱ��- �������������X:{0},Y:{1}�㸽���ĵ�", currentHex.x, currentHex.y));
			List<Vector2Int> aroundCan = GameManager.Instance.GetAroundCanBeOperatedPos(currentHex.x, currentHex.y);
			//Debug.Log(string.Format("-��Ѱ��- �����X:{0},Y:{1}�㸽���ķ���Ҫ��ĵ������Ϊ��{2}", currentHex.x, currentHex.y,aroundCan.Count));
			for (int i = 0; i < aroundCan.Count; i++)
			{
				if (!visited.Contains(aroundCan[i]))
				{
					queue.Enqueue(aroundCan[i]);
					visited.Add(aroundCan[i]);
					log +=("X:" + aroundCan[i].x+"Y:"+aroundCan[i].y+" ");
				}
			}
		}
		Debug.Log(string.Format("-��Ѱ��- ��X:{0}��Y��{1}��Χ���еĵ�һ����{2}����Ϊ��{3}", startHex.x, startHex.y, visited.ToList().Count, log));
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
	/// �������
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	//public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	//{
	//	List<InstructionData> instructionData = new List<InstructionData>();
	//	Debug.Log("____________________________________");
	//	// �������
	//	Vector2Int start = FindCountIsOne(coordinates);
	//	Debug.Log(string.Format("�������-ɸѡ��������ΪX:{0},Y:{1},������Χ��Ϊ{2}", start.x, start.y, GameManager.Instance.GetAroundCanBeOperatedPos(start.x, start.y).Count.ToString()));

	//	// ���ڼ�¼���ʹ��Ľڵ��δ���ʵ��ھ�
	//	Dictionary<Vector2Int, InstDataCalculus> visitedNodes = new Dictionary<Vector2Int, InstDataCalculus>();
	//	bool isOn=false;
	//	int index = 0;
	//	HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
	//	while(!isOn)
	//	{
	//		List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y).Where(pos => coordinates.Contains(pos)&& !visited.Contains(pos)).ToList(); // ��ȡ��ǰ������Χ������
	//		string aroundString = string.Join(", ", around.Select(pos => pos.ToString()).ToArray());
	//		Debug.Log(string.Format("����Ϊ��{0},����Ϊ��{1},IndexΪ��{2},�ѷ��ʹ�����Ϊ��{3}����ǰ����Ϊ��{4}������������Щ�㣺{5}", start,around.Count,index,visited.Count, coordinates.Count, aroundString));
	//		if (around.Count == 0)
	//		{
	//			if (instructionData.Count == coordinates.Count - 1)
	//			{
	//				Debug.Log("û�и���ڵ���Է��ʣ�����,���ص�·������Ϊ��"+ instructionData.Count);
	//				isOn = true;
	//				break;
	//			}

	//			// ���ݣ���ǰ�ڵ�û��δ���ʵ��ھӣ��Ӽ�¼��ȡ����һ���ڵ�
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

	//			Debug.Log("������" + backtrackCount + "��");
	//			//continue;
	//		}


	//		if (around.Count == 1)
	//		{
	//			Vector2Int next = around[0];
	//			instructionData.Add(new InstructionData(start, next));
	//			visited.Add(start);
	//			Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", start, next));
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
	//				Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", around[0], start));
	//				visited.Add(around[0]);
	//			}
	//			else {
	//				index++;
	//				// ��¼��ǰ�ڵ㼰��δ���ʵ��ھ�
	//				UpdateVisitedNodes(visitedNodes, start, index, around);
	//				Vector2Int next = around[0];
	//				instructionData.Add(new InstructionData(start, next));
	//				Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", start, next));
	//				visited.Add(start);
	//				start = next;
	//			}
	//		}
	//		else
	//		{
	//			// û�и���ڵ���Է��ʣ�����
	//			Debug.Log("û�и���ڵ���Է��ʣ�����,���ص�·������Ϊ��" + instructionData.Count);
	//			isOn = true;
	//			break;
	//		}
	//	}

	//	return instructionData;
	//}
	#endregion


	#region Test 1
	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		List<InstructionData> instructionData = new List<InstructionData>();
		Debug.Log("____________________________________");
		// ʹ��HashSet��߲���Ч��
		HashSet<Vector2Int> coordinateSet = new HashSet<Vector2Int>(coordinates);
		// ������㣺��Ϊ1�Ľڵ�
		Vector2Int start = FindCountIsOne(coordinates);
		//Debug.Log($"�ҵ���㣺X={start.x}, Y={start.y}");
		// ��ʼ�����ʼ�¼
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		// ��ʼ��·��
		List<InstructionData> path = new List<InstructionData>();
		// ��ʼ�ݹ�����
		bool success = FindPath(start, coordinateSet, visited, path, null);
		if (success)
		{
			//Debug.Log($"·�������ɹ���·������Ϊ��{path.Count}");
			return path;
		}
		else
		{
			if (coordinates.Count==4) {
				Vector2Int three = FindCountIsThree(coordinates);
				if (three == default) {
					Debug.LogWarning("0:δ���ҵ��������нڵ������·����");
					return new List<InstructionData>();
				} else {
                    for (int i = 0; i < coordinates.Count; i++)
                    {
						if (three!= coordinates[i]) {
							InstructionData instruction = new InstructionData(coordinates[i], three);
							path.Add(instruction);
						}
                    }
                    return path;
				}
			}
            else
            {
				Debug.LogWarning("1:δ���ҵ��������нڵ������·����");
				return new List<InstructionData>();
			}
		}
	}

	/// <summary>
	/// �ݹ�DFS����·��
	/// </summary>
	/// <param name="current">��ǰ�ڵ�</param>
	/// <param name="coordinateSet">�������������HashSet</param>
	/// <param name="visited">�ѷ��ʽڵ��HashSet</param>
	/// <param name="path">��ǰ·����InstructionData�б�</param>
	/// <param name="previous">��һ���ڵ�</param>
	/// <returns>�Ƿ��ҵ�����·��</returns>
	private static bool FindPath(Vector2Int current, HashSet<Vector2Int> coordinateSet, HashSet<Vector2Int> visited, List<InstructionData> path, Vector2Int? previous)
	{
		visited.Add(current);
		Debug.Log($"���ʽڵ㣺X={current.x}, Y={current.y}");

		// �������һ���ڵ㣬���¼·��
		if (previous.HasValue)
		{
			path.Add(new InstructionData(previous.Value, current));
			Debug.Log($"���·����{previous.Value} -> {current}");
		}

		// ������нڵ㶼�ѷ��ʣ����سɹ�
		if (visited.Count == coordinateSet.Count)
		{
			return true;
		}

		// ��ȡ����δ���ʵ��ھ�
		List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(current.x, current.y)
			.Where(pos => coordinateSet.Contains(pos) && !visited.Contains(pos))
			.ToList();

		foreach (var neighbor in neighbors)
		{
			// ���Է����ھ�
			bool found = FindPath(neighbor, coordinateSet, visited, path, current);
			if (found)
			{
				return true;
			}

			// ���ݣ��Ƴ�·���е����һ��
			if (path.Count > 0)
			{
				InstructionData lastStep = path[path.Count - 1];
				if (lastStep.EndVector2 == neighbor)
				{
					path.RemoveAt(path.Count - 1);
					Debug.Log($"����·�����Ƴ� {lastStep.StarVector2} -> {lastStep.EndVector2}");
				}
			}
		}

		// ����޷��������Ƴ���ǰ�ڵ�ķ��ʼ�¼������ʧ��
		visited.Remove(current);
		return false;
	}
	#endregion

	#region MyRegion

	#endregion

	/// <summary>
	/// ��¼��ǰ�ڵ㼰��δ���ʵ��ھ�
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
		// Ԥ��������������ھ��������Լ����ظ�����
		Dictionary<Vector2Int, int> neighborsCountDict = new Dictionary<Vector2Int, int>();
		foreach (var pos in coordinates)
		{
			var neighbors = GameManager.Instance.GetAroundCanBeOperatedPos(pos.x, pos.y);
			int count = neighbors.Count(n => coordinateSet.Contains(n));
			neighborsCountDict[pos] = count;
		}
		var start = neighborsCountDict.FirstOrDefault(kvp => kvp.Value == 1).Key;
		if (start == default)
		{
			// û���ҵ������ھ�����Ϊ3��
			start = neighborsCountDict.FirstOrDefault(kvp => kvp.Value == 3).Key;
		}
		if (start == default)
		{
			// �����Ȼû�ҵ������ھ���������1��
			start = neighborsCountDict.FirstOrDefault(kvp => kvp.Value > 1).Key;
		}
		return start;
	}


	public static Vector2Int FindCountIsThree(List<Vector2Int> coordinates)
	{
		HashSet<Vector2Int> coordinateSet = new HashSet<Vector2Int>(coordinates);
		// Ԥ��������������ھ��������Լ����ظ�����
		Dictionary<Vector2Int, int> neighborsCountDict = new Dictionary<Vector2Int, int>();
		foreach (var pos in coordinates)
		{
			var neighbors = GameManager.Instance.GetAroundCanBeOperatedPos(pos.x, pos.y);
			int count = neighbors.Count(n => coordinateSet.Contains(n));
			neighborsCountDict[pos] = count;
		}
		var start = neighborsCountDict.FirstOrDefault(kvp => kvp.Value == 3).Key;
		return start;
	}
}
