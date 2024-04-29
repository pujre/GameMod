using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
	public virtual void ActivatePanel()
	{
		gameObject.SetActive(true);
	}

	// 隐藏面板
	public virtual void DeactivatePanel()
	{
		gameObject.SetActive(false);
	}

	// 调用特定方法
	public abstract void CallSpecificMethod(string methodName, object[] parameters);
}
