using UnityEngine;
namespace TYQ
{
	public abstract class PanelBase : MonoBehaviour
	{
		public virtual void ActivatePanel(bool isOn)
		{
			gameObject.SetActive(isOn);
		}

		// �����ض�����
		public abstract void CallSpecificMethod(string methodName, object[] parameters);
	}
}