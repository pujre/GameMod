using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager : SingletonMono<LogManager>
{
	public bool IsDebugLog=false;
	public void Log(object message) {
		if (IsDebugLog) {
			Debug.Log(message);
		}
	}

	
}
