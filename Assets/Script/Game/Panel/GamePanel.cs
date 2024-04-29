using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    void Start()
    {
        var buts = transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < buts.Length; i++)
        {
            Button button = buts[i];
            button.onClick.AddListener(() => { OnClickEvent(button.gameObject);});
        }
    }

    void Update()
    {
        
    }

    void OnClickEvent(GameObject but) {
        switch (but.name) {
            case "Prop_1Btn":
                break;
            case "Prop_2Btn":
                break;
            case "Prop_3Btn":
                break;
            case "StopBtn":
				DelegateManager.Instance.Broadcast(OnEventKey.OnStop.ToString());
				break;
        }

    }
}