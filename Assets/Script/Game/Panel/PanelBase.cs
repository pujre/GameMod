using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
	public virtual void ActivatePanel(bool isOn)
	{
		gameObject.SetActive(isOn);
	}


	// 调用特定方法
	public abstract void CallSpecificMethod(string methodName, object[] parameters);
}
