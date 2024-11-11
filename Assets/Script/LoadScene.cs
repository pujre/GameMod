using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
	public string ScenesName;
    public Image LoadingBarImage;
    void Start()
    {
		if (!string.IsNullOrEmpty(ScenesName)) {
			StartCoroutine(MiniGameLoadScene());
		}
	}


	private IEnumerator MiniGameLoadScene() {
		AsyncOperation operation = SceneManager.LoadSceneAsync(ScenesName);

		// 禁止场景在加载完成后自动切换
		operation.allowSceneActivation = false;

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			LoadingBarImage.fillAmount = progress;
			if (operation.progress >= 0.9f)
			{
				operation.allowSceneActivation = true; // 自动切换场景
			}

			yield return null;
		}
	}
}
