using UnityEngine;

public class Test : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width / 2, 50, 100 * 2, 100), "清除日志"))
		{
			GUI.depth = -100;
			Debug.Log("--:" + GUI.depth);
		}

	}

	void Button()
	{
	}
}
