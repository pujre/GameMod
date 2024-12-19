using UnityEngine;
namespace TYQ
{
	public abstract class PanelBase : MonoBehaviour
	{
		public virtual void ActivatePanel(bool isOn)
		{
			gameObject.SetActive(isOn);
		}

		// 调用特定方法
		public abstract void CallSpecificMethod(string methodName, object[] parameters);
	}
}