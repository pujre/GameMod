using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class GameManager : MonoBehaviour
{
    public GoundBackItem[,] GoundBackItemArray2D;


    public void SetGoundBack(int x,int y){
        GoundBackItemArray2D= new GoundBackItem[x, y];
	}

    public void EnterSurface(int x,int y)
    {

    }

	void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
