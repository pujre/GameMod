using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
	public virtual void ActivatePanel(bool isOn)
	{
		gameObject.SetActive(isOn);
	}


	// �����ض�����
	public abstract void CallSpecificMethod(string methodName, object[] parameters);
}
