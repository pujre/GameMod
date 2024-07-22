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

	/// <summary>
	/// �������
	/// </summary>
	/// <param name="coordinates"></param>
	/// <returns></returns>
	public static List<InstructionData> FilterLinkedCoordinates(List<Vector2Int> coordinates)
	{
		List<InstructionData> instructionData = new List<InstructionData>();
		//Debug.Log("____________________________________");
		// �������
		Vector2Int start = coordinates.FirstOrDefault(pos =>
		{
			List<Vector2Int> neighbors = GameManager.Instance.GetAroundPos(pos.x, pos.y).Where(pos => coordinates.Contains(pos)).ToList();
			//Debug.Log(string.Format("��ǰ���ĵ������Ϊ{0},{1}����ǰ�õ���Χ�ĵ����鳤��Ϊ��{2}", pos.x, pos.y, neighbors.Count)); ;
			return neighbors.Count == 1;
		});

		// ���ڼ�¼���ʹ��Ľڵ��δ���ʵ��ھ�
		Dictionary<Vector2Int, InstDataCalculus> visitedNodes = new Dictionary<Vector2Int, InstDataCalculus>();
		bool isOn=false;
		int index = 0;
		HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
		//Debug.Log("Sart������Ϊ��"+ start.x+","+start.y);
		while(!isOn)
		{
			List<Vector2Int> around = GameManager.Instance.GetAroundPos(start.x, start.y).Where(pos => coordinates.Contains(pos)&& !visited.Contains(pos)).ToList(); // ��ȡ��ǰ������Χ������
			//Debug.Log(string.Format("����Ϊ��{0},����Ϊ��{1},IndexΪ��{2},�ѷ��ʹ�����Ϊ��{3}����ǰ��Ϊ��{4}", start,around.Count,index,visited.Count, coordinates.Count));
			if (around.Count == 0)
			{
				if (instructionData.Count == coordinates.Count - 1)
				{
					//Debug.Log("û�и���ڵ���Է��ʣ�����,���ص�·������Ϊ��"+ instructionData.Count);
					isOn = true;
					break;
				}

				// ���ݣ���ǰ�ڵ�û��δ���ʵ��ھӣ��Ӽ�¼��ȡ����һ���ڵ�
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

				//Debug.Log("������" + backtrackCount + "��");
				//continue;
			}


			if (around.Count == 1)
			{
				Vector2Int next = around[0];
				instructionData.Add(new InstructionData(start, next));
				visited.Add(start);
				//Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", start, next));
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
					//Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", around[0], start));
					visited.Add(around[0]);
				}
				else {
					index++;
					// ��¼��ǰ�ڵ㼰��δ���ʵ��ھ�
					UpdateVisitedNodes(visitedNodes, start, index, around);
					Vector2Int next = around[0];
					instructionData.Add(new InstructionData(start, next));
					//Debug.Log(string.Format("�����һ��InstructionData��startΪ��{0},endΪ��{1},", start, next));
					visited.Add(start);
					start = next;
				}
			}
			else
			{
				// û�и���ڵ���Է��ʣ�����
				//Debug.Log("û�и���ڵ���Է��ʣ�����,���ص�·������Ϊ��" + instructionData.Count);
				isOn = true;
				break;
			}
		}

		return instructionData;
	}

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

}
