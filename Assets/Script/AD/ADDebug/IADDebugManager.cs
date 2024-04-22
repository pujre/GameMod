using UnityEngine;

public class IADDebugManager
{
	public enum DebugColor
	{
		Default,
		Red,
		Green,
		Blue,
		Yellow
	}
	public static void Log(string message, DebugColor color = DebugColor.Default)
	{
		switch (color)
		{
			case DebugColor.Red:
				Debug.Log($"<color=red>{"TSDK:"+message}</color>");
				break;
			case DebugColor.Green:
				Debug.Log($"<color=green>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Blue:
				Debug.Log($"<color=blue>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Yellow:
				Debug.Log($"<color=yellow>{"TSDK:" + message}</color>");
				break;
			default:
				Debug.Log(message);
				break;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"> </param>
	/// <param name="color"> "#FFA500" </param>
	public static void Logs(string message, string color ="")
	{
		if (string.IsNullOrEmpty(color))
		{
			Debug.Log("TSDK:" + message);
		}
		else
		{
			Debug.Log($"<color={color}>{"TSDK:" + message}</color>");
		}
	}

	public static void LogWarning(string message, DebugColor color = DebugColor.Default)
	{
		switch (color)
		{
			case DebugColor.Red:
				Debug.LogWarning($"<color=red>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Green:
				Debug.LogWarning($"<color=green>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Blue:
				Debug.LogWarning($"<color=blue>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Yellow:
				Debug.LogWarning($"<color=yellow>{"TSDK:" + message}</color>");
				break;
			default:
				Debug.LogWarning("TSDK:" + message);
				break;
		}
	}

	public static void LogError(string message, DebugColor color = DebugColor.Default)
	{
		switch (color)
		{
			case DebugColor.Red:
				Debug.LogError($"<color=red>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Green:
				Debug.LogError($"<color=green>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Blue:
				Debug.LogError($"<color=blue>{"TSDK:" + message}</color>");
				break;
			case DebugColor.Yellow:
				Debug.LogError($"<color=yellow>{"TSDK:" + message}</color>");
				break;
			default:
				Debug.LogError("TSDK:" + message);
				break;
		}
	}


}
