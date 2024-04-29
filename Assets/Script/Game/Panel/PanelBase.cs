using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
	public virtual void ActivatePanel()
	{
		gameObject.SetActive(true);
	}

	// �������
	public virtual void DeactivatePanel()
	{
		gameObject.SetActive(false);
	}

	// �����ض�����
	public abstract void CallSpecificMethod(string methodName, object[] parameters);
}
